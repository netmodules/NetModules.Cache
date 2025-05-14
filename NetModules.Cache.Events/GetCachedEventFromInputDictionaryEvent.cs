using System;
using System.Collections.Generic;
using NetModules;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;

namespace NetModules.Cache.Events
{
    /// <summary>
    /// This event can be handled by event caching modules to return a cached event output.
    /// Event caching modules can handle event caching silently and automatically but can also handle
    /// this event directly if the event is raised by another module.
    /// </summary>
    [JsonSchemaTitle("Get Cached Event From Input Dictionary Event")]
    [JsonSchemaDescription("This event can be handled by event caching modules to return a cached event output. Event caching modules can handle event caching silently and automatically but can also handle this event directly if the event is raised by another module.")]
    public class GetCachedEventFromInputDictionaryEvent : Event<GetCachedEventFromInputDictionaryEventInput, GetCachedEventOutput>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override EventName Name { get { return "NetModules.Cache.GetCachedEventFromInputDictionary"; } }
    }
}