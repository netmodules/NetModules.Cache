using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using NetTools.Cache;
using NetTools.Serialization;
using NetModules;
using NetModules.Interfaces;
using NetModules.Events;
using NetModules.Cache.Events;

namespace NetModules.Cache.MemoryCache.Classes
{
    [Serializable]
    internal class CacheHandler
    {
        Module Module;
        
        TimeSpan DefaultExpires;
        Dictionary<string, int> Expires;
        
        bool CacheWithMeta;
        List<string> ExcludeMetaKeys;

        bool CacheUniqueKeys;

        // Used to start checking cache, this field is set by MemoryCacheModule
        // when OnAllModulesLoaded() is invoked by ModuleHost.
        internal bool Loaded = false;


        /// <summary>
        /// Creates a new instance of the CacheHandler class. This is used to handle any incoming events
        /// and return cached data if available.
        /// </summary>
        internal CacheHandler(Module module, uint defaultExpires, Dictionary<string, object> expires, bool cacheWithMeta, List<object> excludeMetaKeys, bool cacheUniqueKeys)
        {
            Module = module;
            CacheWithMeta = cacheWithMeta;
            CacheUniqueKeys = cacheUniqueKeys;
            DefaultExpires = TimeSpan.FromSeconds(defaultExpires);
            ExcludeMetaKeys = new List<string>(excludeMetaKeys.Where(x => x != null && !string.IsNullOrWhiteSpace(x.ToString())).Select(x => x.ToString()));
            Expires = new Dictionary<string, int>(expires.Select(x => new KeyValuePair<string, int>(x.Key, int.TryParse(x.Value.ToString(), out var expire)
                ? expire
                : (int)DefaultExpires.TotalSeconds)));

        }


        /// <summary>
        /// 
        /// </summary>
        internal void GetCachedEvent(IEvent e)
        {
            if (!Loaded)
            {
                return;
            }

            // Check to see if the event has already been returned from cache, and if it has we shouldn't handle it again.
            // We also check here to see if the event is marked as "noCache" (do not cache), and if it is we shouldn't
            // handle it.
            if (e.GetMetaValue(Constants.FromCache, false) || e.GetMetaValue(Constants.NoCache, false))
            {
                return;
            }

            // If the event is included in the expires dictionary and the value is 0 then we don't want to return a cached
            // output for the event.
            if (Expires.TryGetValue(e.Name, out int seconds))
            {
                if (seconds == 0)
                {
                    return;
                }
            }

            var input = e.GetEventInput();

            if (input == null)
            {
                return;
            }

            var meta = e.Meta;
            var cached = GetCached(e.Name, input, meta);

            if (cached == null)
            {
                return;
            }

            // We can only set the output from cache if the cached IEventOutput is the same type
            // as @event.Output, rather than type checking we wrap try/catch as it's less expensive.
            // It's possible with a probability of 2^64, due to the 'birthday paradox', that 2 hashes
            // can collide so we must do this for the 1 in 340 undecillion chance that this may
            // happen and throw an exception.
            try
            {
                e.SetEventOutput(cached);
                e.SetMetaValue(Constants.FromCache, true, true);
                e.Handled = true;
            }
            catch { }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void GetCachedEventEvent(GetCachedEvent e)
        {
            if (string.IsNullOrEmpty(e.Input.EventName) || e.Input.EventInput == null)
            {
                e.SetMetaValue(Constants.Message, Constants.MessageMustBeValid);
                e.Handled = false;
                return;
            }

            var cached = GetCached(e.Input.EventName, e.Input.EventInput, e.Input.EventMeta);

            if (cached == null)
            {
                e.SetMetaValue(Constants.Message, Constants.MessageNotCached);
                e.Handled = false;
                return;
            }

            e.Output = new GetCachedEventOutput()
            {
                EventOutput = cached
            };

            e.Handled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        internal void GetCachedEventEvent(GetCachedEventFromInputDictionaryEvent e)
        {
            if (string.IsNullOrEmpty(e.Input.EventName) || e.Input.EventInput == null)
            {
                e.SetMetaValue(Constants.Message, Constants.MessageMustBeValid);
                e.Handled = false;
                return;
            }

            var @event = Module.Host.Events.GetSolidEventFromName(e.Input.EventName);

            if (@event == null)
            {
                e.SetMetaValue(Constants.Message, Constants.MessageEventNotFound);
                e.Handled = false;
                return;
            }

            @event = @event.ObjectFromDictionary(new Dictionary<string, object>
            {
                { "name", e.Input.EventName },
                { "input", e.Input.EventInput },
                { "meta", e.Input.EventMeta }
            }) as IEvent;

            var cached = GetCached(@event.Name, @event.GetEventInput(), @event.Meta);

            if (cached == null)
            {
                e.SetMetaValue(Constants.Message, Constants.MessageNotCached);
                e.Handled = false;
                return;
            }

            e.Output = new GetCachedEventOutput()
            {
                EventOutput = cached
            };

            e.Handled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        internal IEventOutput GetCached(EventName name, IEventInput input, Dictionary<string, object> meta)
        {
            string key = null;

            if (CacheUniqueKeys && meta != null && meta.TryGetValue("id", out var id) && id != null)
            {
                var keyId = '_' + id.ToString();

                key = DataStore.GetCache(keyId
                    , DataStore.SetCache(
                        GenerateInputHash(name, input, meta), keyId, TimeSpan.FromSeconds(2)
                      ));
            }
            else
            {
                key = GenerateInputHash(name, input, meta);
            }

            return DataStore.GetCache(key, null as IEventOutput);
        }


        /// <summary>
        /// 
        /// </summary>
        internal void SetCachedEvent(IEvent e)
        {
            // Check to see if the event has already been cached and if it has we shouldn't cache it again.
            // We also check here to see if the event is marked as "noCache" (do not cache), and if it is we shouldn't
            // handle it.
            if (e.GetMetaValue(Constants.FromCache, false) || e.GetMetaValue(Constants.NoCache, false))
            {
                return;
            }

            TimeSpan expires;

            // Check to see if the event has the cacheExpires meta key and if it does then this overrides
            // any other settings.
            var metaCache = e.GetMetaValue(Constants.CacheExpires, -1);

            if (metaCache == -1)
            {
                // "cacheExpires" meta value may be a TimeSpan...
                var timespanCache = e.GetMetaValue(Constants.CacheExpires, default(TimeSpan));

                if (timespanCache.TotalSeconds > 0)
                {
                    metaCache = (int)timespanCache.TotalSeconds;
                }
            }

            // Check to see if the event has the cache meta key and TimeSpan and if it does then this
            // overrides any other settings.
            if (metaCache == -1)
            {
                var timespanCache = e.GetMetaValue(Constants.Cache, default(TimeSpan));

                if (timespanCache.TotalSeconds > 0)
                {
                    metaCache = (int)timespanCache.TotalSeconds;
                }
            }

            // "cache" meta value may be a int (seconds)...
            if (metaCache == -1)
            {
                metaCache = e.GetMetaValue(Constants.Cache, -1);
            }

            if (metaCache > -1)
            {
                expires = TimeSpan.FromSeconds(metaCache);
            }
            else if (Expires.TryGetValue(e.Name, out int seconds))
            {
                // Is the event named in the expires dictionary? If it is then this overrides the default
                // expiry time.
                expires = TimeSpan.FromSeconds(seconds);
            }
            else
            {
                expires = DefaultExpires;
            }

            // If any of the expiry times resulted in 0 seconds then we don't want to cache the object.
            if (expires == TimeSpan.Zero)
            {
                return;
            }

            // We use the IEvent.Name and IEvent.Input to generate a unique key to cache the event output with.
            // If the input or output are null we can't cache the output object.
            var input = e.GetEventInput();
            var output = e.GetEventOutput();

            if (input == null || output == null)
            {
                return;
            }

            var key = GenerateInputHash(e.Name, input, e.Meta);

            if (!string.IsNullOrEmpty(key))
            {
                DataStore.SetCache(output, key, expires);
            }
        }


        /// <summary>
        /// This helper method generates an MD5 hash using the event name and a JSON string of the input.
        /// </summary>
        string GenerateInputHash(EventName name, IEventInput input,Dictionary<string, object> meta)
        {
            var sb = new StringBuilder();

            try
            {
                var seed = name + input.ToJson();
                
                if (CacheWithMeta && meta != null)
                {
                    foreach (var kv in meta)
                    {
                        if (kv.Value == null || ExcludeMetaKeys.Contains(kv.Key))
                        {
                            continue;
                        }

                        seed += kv.Key + kv.Value.ToString();
                    }
                }

                // Use input string to calculate MD5 hash.
                var inputBytes = Encoding.UTF8.GetBytes(seed);
                //var hashBytes = Crypto.ComputeHash(inputBytes);

                using (var md5 = MD5.Create())
                {
                    var hashBytes = md5.ComputeHash(inputBytes);

                    // Convert the byte array to hexadecimal string.
                    for (var i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }
                }
            }
            catch(Exception ex)
            {
                Module.Log(LoggingEvent.Severity.Error, $"{nameof(MemoryCacheModule)} is unable to genterate an input hash key for this event.", name, ex);
            }

            return sb.ToString();
        }
    }
}
