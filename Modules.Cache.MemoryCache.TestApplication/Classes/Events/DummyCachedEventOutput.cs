using System;
using NetModules;
using NetModules.Interfaces;

namespace Modules.Cache.MemoryCache.TestApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class DummyCachedEventOutput : IEventOutput
    {
        public DateTime CachedTime { get; set; }
    }
}
