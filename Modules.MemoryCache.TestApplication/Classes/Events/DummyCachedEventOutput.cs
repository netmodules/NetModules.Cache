using reblGreen;
using reblGreen.NetCore.Modules;
using System;
using reblGreen.NetCore.Modules.Interfaces;

namespace Modules.MemoryCache.TestApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class DummyCachedEventOutput : IEventOutput
    {
        public DateTime CachedTime { get; set; }
    }
}
