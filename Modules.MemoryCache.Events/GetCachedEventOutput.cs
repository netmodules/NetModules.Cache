using System;
using reblGreen.NetCore.Modules.Interfaces;

namespace Modules.MemoryCache.Events
{
    /// <summary>
    /// If a cached object is found for the values entered in <see cref="GetCachedEventInput"/> it is
    /// returned to the requester via the <see cref="EventOutput"/> property.
    /// </summary>
    public struct GetCachedEventOutput : IEventOutput
    {
        /// <summary>
        /// If a cached object is found for the values entered in <see cref="GetCachedEventInput"/> it is
        /// returned to the requester via this property.
        /// </summary>
        public IEventOutput EventOutput { get; set; }
    }
}
