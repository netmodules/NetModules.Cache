using System;
using System.Collections.Generic;
using NetModules;
using NetModules.Events;
using NetModules.Interfaces;
using Modules.Cache.MemoryCache.Classes;
using NetTools.Serialization;
using Modules.Cache.Events;

namespace Modules.Cache.MemoryCache
{
    /// <summary>
    /// A basic cache module that uses an in-memory dictionary to store event output to cache using the event name and
    /// input as a storage and lookup identifier
    /// </summary>
    [Serializable]
    [Module(
        LoadPriority = short.MinValue,
        HandlePriority = short.MaxValue,
        Description = "A basic cache module that uses an in-memory dictionary to store event output to cache using the "
        + "event name and input as a storage and lookup identifier.",
        AdditionalInformation = new string[]
        {
            "Adding a \"noCache\" key with a bool value of true will tell the caching module not to fetch data from the "
            + "cache. any IEvent with this meta value will be returned unhandled by this module so it can continue to be "
            + "handled by other modules.",
            "Adding a \"cacheExpires\" key with a integer value in seconds to cache an event output object for before "
            + "expiring the cached data will override the default cache expiry and the per event expiry object in the "
            + "module settings. This can be used to control cache on a per-event basis.",
            "When data is returned from cache this module will add the \"fromCache\" meta key with a bool value of true. "
            + "This meta value can be checked to see if the data is cache data or fresh data. If fresh data the key will "
            + "not exist in the event meta."
        }
    )]
    public class MemoryCacheModule : Module, IEventPostHandler<IEvent>
    {
        CacheHandler CacheHandler;


        /// <summary>
        /// 
        /// </summary>
        public override bool CanHandle(IEvent e)
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Handle(IEvent e)
        {
            if (CacheHandler == null)
            {
                return;
            }

            if (e is GetCachedEvent getCachedEvent)
            {
                CacheHandler.GetCachedEventEvent(getCachedEvent);
                return;
            }

            CacheHandler.GetCachedEvent(e);
        }


        /// <summary>
        /// 
        /// </summary>
        public override void OnLoading()
        {
            var defaultExpire = GetSetting("defaultExpire", 3600);
            var expire = GetSetting("expire", new Dictionary<string, object>());

            CacheHandler = new CacheHandler(this, (uint)defaultExpire, expire);
            base.OnLoading();
        }


        /// <summary>
        /// 
        /// </summary>
        public override void OnLoaded()
        {
            base.OnLoaded();
        }


        /// <summary>
        /// 
        /// </summary>
        public override void OnUnloading()
        {
            base.OnUnloading();
        }


        /// <summary>
        /// 
        /// </summary>
        public void OnHandled(IEvent e)
        {
            if (CacheHandler == null)
            {
                return;
            }

            if (e is GetCachedEvent)
            {
                return;
            }

            CacheHandler.SetCachedEvent(e);
        }
    }
}
