using System;
using System.Collections.Generic;
using NetModules;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;
using NetTools.Serialization.JsonSchemaEnums;

namespace NetModules.Cache.Events
{
    /// <summary>
    /// The properties in this object are used to create an identifier that is used to lookup a cached output.
    /// </summary>
    [JsonSchemaTitle("Get Cached Event Input")]
    [JsonSchemaDescription("The properties in this object are used to create an identifier that is used to lookup a cached output.")]
    public struct GetCachedEventInput : IEventInput
    {
        /// <summary>
        /// This property identifies the event type for that the <see cref="EventInput"/> belongs. Set this to
        /// the name of the event you wish to check the cache for.
        /// </summary>
        [JsonSchemaTitle("Event Name")]
        [JsonSchemaDescription("This property identifies the event type for that the Event Input belongs. Set this to the name of the event you wish to check the cache for.")]
        public EventName EventName { get; set; }

        /// <summary>
        /// The <see cref="EventName"/> and <see cref="EventInput"/> are used to generate a "unique" key
        /// when caching an IEventOutput. This unique key is later used to retrieve the cached data when
        /// required. Set this to the event input object for the event you wish to check the cache for.
        /// </summary>
        [JsonSchemaTitle("Event Input")]
        [JsonSchemaDescription("The Event Name and Event Input are used to generate a \"unique\" key when caching an IEventOutput. This unique key is later used to retrieve the cached data when required. Set this to the event input object for the event you wish to check the cache for.")]
        [JsonSchemaType(BasicType.Object)]
        public IEventInput EventInput { get; set; }

        /// <summary>
        /// The <see cref="IEvent.Meta"/> may be required to generate a "unique" key if caching module is
        /// configured to require metadata for caching when caching an IEventOutput. Set this to the event
        /// metadata object for the event you wish to check the cache for.
        /// </summary>
        [JsonSchemaTitle("Event Metadata")]
        [JsonSchemaDescription("The Event Metadata may be required to generate a \"unique\" key if caching module is configured to require metadata for caching when caching an IEventOutput. Set this to the event metadata object for the event you wish to check the cache for.")]
        [JsonSchemaType(BasicType.Object)]
        public Dictionary<string, object> EventMeta { get; set; }
    }
}
