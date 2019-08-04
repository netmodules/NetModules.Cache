using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using reblGreen.Cache;
using reblGreen.Serialization;
using reblGreen.NetCore.Modules.Interfaces;
using reblGreen.NetCore.Modules.Events;

namespace reblGreen.NetCore.Modules.MemoryCache.Classes
{
    [Serializable]
    internal class CacheHandler
    {
        Module Module;
        MD5 Crypto;
        TimeSpan DefaultExpires;
        Dictionary<string, int> Expires;

        /// <summary>
        /// 
        /// </summary>
        internal CacheHandler(Module module, uint defaultExpires, Dictionary<string, object> expires)
        {
            Module = module;
            Crypto = MD5.Create();

            DefaultExpires = TimeSpan.FromSeconds(defaultExpires);
            Expires = new Dictionary<string, int>(expires.Select(x => new KeyValuePair<string, int>(x.Key, (int)x.Value)));
        }


        /// <summary>
        /// 
        /// </summary>
        internal void GetCachedEvent(IEvent @event)
        {
            if (@event.GetMetaValue("noCache", false))
            {
                return;
            }

            if (Expires.TryGetValue(@event.Name, out int seconds))
            {
                if (seconds == 0)
                {
                    return;
                }
            }

            var input = @event.GetEventInput();

            if (input == null)
            {
                return;
            }

            var cached = GetCached(@event.Name, input);

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
                @event.SetEventOutput(cached);
                @event.SetMetaValue("fromCache", true, true);
                @event.Handled = true;
            }
            catch { }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void GetCachedEventEvent(GetCachedEvent @event)
        {
            var cached = GetCached(@event.Input.EventName, @event.Input.EventInput);

            if(cached == null)
            {
                return;
            }

            @event.Output = new GetCachedEventOutput()
            {
                EventOutput = cached
            };

            @event.Handled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        internal IEventOutput GetCached(EventName name, IEventInput input)
        {
            var key = GenerateInputHash(name, input);
            return DataStore.GetCache(key, null as IEventOutput);
        }


        /// <summary>
        /// 
        /// </summary>
        internal void SetCachedEvent(IEvent @event)
        {
            if (@event.GetMetaValue("fromCache", false))
            {
                return;
            }

            TimeSpan expires;

            if (Expires.TryGetValue(@event.Name, out int seconds))
            {
                if (seconds == 0)
                {
                    return;
                }

                expires = TimeSpan.FromSeconds(seconds);
            }
            else
            {
                expires = DefaultExpires;
            }

            var input = @event.GetEventInput();
            var output = @event.GetEventOutput();

            if (input == null || output == null)
            {
                return;
            }

            var key = GenerateInputHash(@event.Name, input);
            DataStore.SetCache(output, key, expires);
        }


        /// <summary>
        /// This helper method generates an MD5 hash using the event name and a JSON string of the input.
        /// </summary>
        string GenerateInputHash(EventName name, IEventInput input)
        {
            var seed = name + input.ToJson();

            // Use input string to calculate MD5 hash.
            var sb = new StringBuilder();
            var inputBytes = Encoding.ASCII.GetBytes(seed);
            var hashBytes = Crypto.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string.
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
