using System;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Events
{
    /// <summary>
    /// The properties in this object are used to create an identifier which is used to lookup a cached output.
    /// </summary>
    public struct GetCachedEventInput : IEventInput
    {
        /// <summary>
        /// This property identifies the event type for which the <see cref="EventInput"/> belongs. Set this to
        /// the name of the event you wish to check the cache for.
        /// </summary>
        public EventName EventName { get; set; }

        /// <summary>
        /// The <see cref="EventName"/> and <see cref="EventInput"/> are used to generate a "unique" key
        /// when caching an IEventOutput. This unique key is later used to retrieve the cached data when
        /// required. Set this to the event input object for the event you wish to check the cache for.
        /// </summary>
        public IEventInput EventInput { get; set; }
    }
}
