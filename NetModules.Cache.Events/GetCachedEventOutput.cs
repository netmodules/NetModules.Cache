using System;
using NetModules;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;
using NetTools.Serialization.JsonSchemaEnums;

namespace NetModules.Cache.Events
{
    /// <summary>
    /// Returns a cached event output, if a cached object is found for the values provided in the event input.
    /// </summary>
    [JsonSchemaTitle("Get Cached Event Output")]
    [JsonSchemaDescription("Returns a cached event output, if a cached object is found for the values provided in the event input.")]
    public struct GetCachedEventOutput : IEventOutput
    {
        /// <summary>
        /// If a cached object is found for the values provided in <see cref="GetCachedEventInput"/> it is
        /// returned to the requester via this property.
        /// </summary>
        [JsonSchemaTitle("Event Output")]
        [JsonSchemaDescription("If a cached object is found for the values provided in the event input, it is returned to the requester via this property.")]
        [JsonSchemaType(BasicType.Object)]
        public IEventOutput EventOutput { get; set; }
    }
}
