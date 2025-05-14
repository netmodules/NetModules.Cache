using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModules.Cache.MemoryCache.Classes
{
    internal class Constants
    {
        /// <summary>
        /// The key used to set the cache expiry time in the event metadata.
        /// This can be added by other modules.
        /// </summary>
        internal const string Cache = "cache";

        /// <summary>
        /// The key used to set the cache expiry time in the event metadata.
        /// This can be added as an alternative by other modules.
        /// </summary>
        internal const string CacheExpires = "cacheExpires";

        /// <summary>
        /// The key used to specify that the event should not be cached in
        /// the event metadata. This can be added by other modules.
        /// </summary>
        internal const string NoCache = "noCache";

        /// <summary>
        /// The key used to tell other modules that the data was returned
        /// from cache. This is added to an event by the caching module.
        /// </summary>
        internal const string FromCache = "fromCache";

        /// <summary>
        /// 
        /// </summary>
        internal const string Message = "message";

        /// <summary>
        /// 
        /// </summary>
        internal const string MessageMustBeValid = "Input.EventName and Input.EventInput must be set to valid objects.";

        /// <summary>
        /// 
        /// </summary>
        internal const string MessageNotCached = "This event has not been cached.";

        /// <summary>
        /// 
        /// </summary>
        internal const string MessageEventNotFound = "Unknown event, the event could not be found in Module.Host.Events";
    }
}
