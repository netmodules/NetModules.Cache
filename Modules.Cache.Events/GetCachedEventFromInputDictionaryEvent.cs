using System;
using System.Collections.Generic;
using NetModules;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;

namespace Modules.Cache.Events
{
    /// <summary>
    /// This event can be handled by event caching modules to return a cached event output.
    /// Event caching modules should handle event output caching automatically but should also handle this event
    /// directly if the event is raised by another module.
    /// </summary>
    [JsonSchemaTitle("Get Cached Event From Input Dictionary Event")]
    [JsonSchemaDescription("This event can be handled by various event caching modules.")]
    public class GetCachedEventFromInputDictionaryEvent : Event<GetCachedEventFromInputDictionaryEventInput, GetCachedEventOutput>
    {
        /// <summary>
        /// A unique name which can be used to identify the event type where the concrete type of the IEvent object
        /// is unknown.
        /// </summary>
        public override EventName Name { get { return "Cache.GetCachedEventFromInputDictionary"; } }
    }
}