using System;
using NetModules;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;
using NetTools.Serialization.JsonSchemaEnums;

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
        [JsonSchemaType(BasicType.Object)]
        public IEventOutput EventOutput { get; set; }
    }
}
