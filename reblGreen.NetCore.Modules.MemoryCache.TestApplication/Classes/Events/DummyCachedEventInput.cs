using System;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.MemoryCache.TestApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class DummyCachedEventInput : IEventInput
    {
        public DateTime CachedTime { get; set; }
    }
}
