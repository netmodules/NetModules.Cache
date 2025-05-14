using System;
using NetModules;
using NetModules.Interfaces;

namespace NetModules.Cache.MemoryCache.TestApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class DummyCachedEventOutput : IEventOutput
    {
        public DateTime CachedTime { get; set; }
    }
}
