using System;
using System.Collections.Generic;
using reblGreen.NetCore.Modules.Events;
using reblGreen.NetCore.Modules.Interfaces;
using reblGreen.NetCore.Modules.MemoryCache.Classes;
using reblGreen.Serialization;


namespace reblGreen.NetCore.Modules.MemoryCache
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    [Module(
        HandlePriority = short.MaxValue,
        Description = ""
    )]
    public class CacheModule : Module, IEventPostHandler<IEvent>
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
