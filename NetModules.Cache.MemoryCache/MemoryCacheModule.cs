using System;
using System.Collections.Generic;
using NetModules;
using NetModules.Events;
using NetModules.Interfaces;
using NetModules.Cache.MemoryCache.Classes;
using NetTools.Serialization;
using NetModules.Cache.Events;
using System.Linq;

namespace NetModules.Cache.MemoryCache
{
    /// <summary>
    /// A basic event caching module that uses an in-memory dictionary to store event output to cache using the event name and
    /// the event input as a storage and lookup identifier.
    /// </summary>
    [Serializable]
    [Module(
        LoadPriority = short.MinValue,
        HandlePriority = short.MinValue + 1000,
        Description = "A basic event caching module that uses an in-memory dictionary to store event output to cache using the "
        + "event name, and the event input as a storage and lookup identifier.",
        AdditionalInformation = new string[]
        {
            "Adding a \"noCache\" key with a bool value of true will tell the caching module not to fetch data from the "
            + "cache. any IEvent with this meta value will be returned unhandled by this module so it can continue to be "
            + "handled by other modules.",
            "Adding a \"cacheExpires\" key with a integer value in seconds to cache an event output object for before "
            + "expiring the cached data will override the default cache expiry and the per event expiry object in this "
            + "module's settings. This can be used to control cache on a per-event basis.",
            "When data is returned from cache, this module will add the \"fromCache\" meta key with a bool value of true. "
            + "This meta value can be checked to see if the data is cache data or fresh data. If fresh data, the key will "
            + "not exist in the event metadata."
        }
    )]
    public class MemoryCacheModule : Module, IEventPostHandler<IEvent>
    {
        CacheHandler CacheHandler;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanHandle(IEvent e)
        {
            /*
             * This caching module returns true for all events so that it can handle any caching depending on
             * the configuration in NetModules.Cache.MemoryCache.MemoryCacheModule.settings.default.json and/or
             * NetModules.Cache.MemoryCache.MemoryCacheModule.settings.json
             */
            return true;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Handle(IEvent e)
        {
            // Don't try reading from cache if CacheHandler is not ready.
            if (CacheHandler == null)
            {
                return;
            }

            /*
             * These events are located in the NetModules.Cache.Events namespace and can
             * be used to retrieve a cached event output on demand. These events are also
             * used for testing in the TestApplication project.
             */
            if (e is GetCachedEvent getCachedEvent)
            {
                CacheHandler.GetCachedEventEvent(getCachedEvent);
                return;
            }

            if (e is GetCachedEventFromInputDictionaryEvent getFromDic)
            {
                CacheHandler.GetCachedEventEvent(getFromDic);
                return;
            }
            
            /*
             * Because this module returns true for CanHandle indefinitely, all IEvent
             * instances will be set to this method (unless the event specifies its own
             * handler using a NetModules.Attributes.EventHandler attribute). Here we
             * attempt to get the event from cache and return it, this is how caching
             * events works!!!
             */
            CacheHandler.GetCachedEvent(e);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnLoading()
        {
            /*
             * The default expiry time in seconds for the event name if it does not exist in
             * the expire dictionary below or the object does not include a "cacheExpire"
             * value in the IEvent.Meta dictionary. This defaults to 1 hour (3600 seconds).
             */
            var defaultExpire = GetSetting("defaultExpire", 3600);

            var expireDic = new Dictionary<string, object>() { { "NetModules.Events.LoggingEvent", 0 } };
            var expire = GetSetting("expire", expireDic);

            if (expire == null)
            {
                expire = expireDic;
            }

            // Force LoggingEvent to have 0 cache expire to prevent possible StackOverflowException...
            foreach (var kv in expireDic)
            {
                expire[kv.Key] = kv.Value;
            }

            // Force excluding the "id" and "handlers" keys from the generated cache key...
            var cacheWithMeta = GetSetting("cacheWithMeta", true);
            var excludeMetaKeysList = new List<object>() { "id", "handlers" };
            var excludeMetaKeys = GetSetting("excludeMetaKeys", excludeMetaKeysList);

            if (excludeMetaKeys == null)
            {
                excludeMetaKeys = excludeMetaKeysList;
            }

            excludeMetaKeys.AddRange(excludeMetaKeysList);
            excludeMetaKeys = excludeMetaKeys.Distinct().ToList();

            var cacheUniqueKeys = GetSetting("cacheUniqueKeys", true);

            CacheHandler = new CacheHandler(this, (uint)Math.Clamp(defaultExpire, 0, short.MaxValue)
                , expire, cacheWithMeta, excludeMetaKeys, cacheUniqueKeys);
            
            base.OnLoading();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnAllModulesLoaded()
        {
            CacheHandler.Loaded = true;
            base.OnAllModulesLoaded();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void OnHandled(IEvent e)
        {
            /*
             * This module implements the IEventPostHandler interface and is called after
             * an event has been handled. This is where we can set the event output to
             * cache if required.
             */

            // Don't try to cache if CacheHandler is not ready...
            if (CacheHandler == null)
            {
                return;
            }

            /*
             * We don't want to cache these events as they are located in the
             * NetModules.Cache.Events namespace and are used to retrieve a cached event
             * output on demand.
             */
            if (e is GetCachedEvent
                || e is GetCachedEventFromInputDictionaryEvent)
            {
                return;
            }

            CacheHandler.SetCachedEvent(e);
        }
    }
}
