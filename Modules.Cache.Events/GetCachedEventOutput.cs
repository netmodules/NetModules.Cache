using System;
using NetModules;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;

namespace Modules.Cache.Events
{
    /// <summary>
    /// If a cached object is found for the values entered in <see cref="GetCachedEventInput"/> it is
    /// returned to the requester via the <see cref="EventOutput"/> property.
    /// </summary>
    [JsonSchemaTitle("Get Cached Event Output")]
    [JsonSchemaDescription("")]
    public struct GetCachedEventOutput : IEventOutput
    {
        /// <summary>
        /// If a cached object is found for the values entered in <see cref="GetCachedEventInput"/> it is
        /// returned to the requester via this property.
        /// </summary>
        [JsonSchemaTitle("")]
        [JsonSchemaDescription("")]
        public IEventOutput EventOutput { get; set; }
    }
}
