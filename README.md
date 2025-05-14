# NetModules.Cache

**NetModules.Cache** is a basic event caching module that is built using the [NetModules](https://github.com/netmodules/NetModules) architecture.

This repository consists of two libraries:

- **NetModules.Cache.MemoryCache**: A library that contains a basic in-memory caching module.
- **NetModules.Cache.Events**: A library that contains helper events for testing cached events, and requesting cached data on demand.

It enables a simple in-memory caching solution for **NetModules** Events and can be configured to cache any event type that implements the `IEvent` interface, with the exception of an event that has assigned a strict handler using the `EventHandlerAttribute`.

> [!NOTE]
> A Module doesn't need to worry about how event caching is handled unless it's an event caching Module!

Applications that frequently process identical event requests benefit from caching to **reduce redundant processing** and **speed up response times**. MemoryCacheModule provides an efficient caching layer by:
- Storing **event output** based on event names and input parameters.
- Allowing modules to control caching behavior dynamically.
- Providing fine-grained **cache expiration management**.

## Features

- **Event-Based Caching**: Stores event output using input parameters as cache keys.
- **Dynamic Expiry Control**: Set custom expiration times per event.
- **Metadata-Driven Behavior**: Control caching at runtime using event metadata.
- **Efficient Handling**: Automatically retrieves cached data when available.

## Getting Started

### Installation

To use **NetModules.Cache**, `MemoryCacheModule`, add the library to your `ModuleHost` project via NuGet Package Manager:

```bash
Install-Package NetModules.Cache.MemoryCache
```

To use **NetModules.Cache.Events**, and request cache data on demand from another module, add the events library to your `Module` project via NuGet Package Manager:

```bash
Install-Package NetModules.Cache.Events
```

### Setup and Configuration

When you install `MemoryCacheModule`, it will automatically register itself with the `ModuleHost` and start caching events using the default configuration (see included file: `NetModules.Cache.MemoryCache.MemoryCacheModule.settings.default.json`). You can configure the caching behavior by creating your own settings file `"NetModules.Cache.MemoryCache.MemoryCacheModule.settings.json"` and modifying/adding settings accordingly.

> [!IMPORTANT]  
> To add your own configuration to **NetModules.Cache.MemoryCache** `MemoryCacheModule`, a module that can handle a [GetSettingEvent](https://github.com/netmodules/NetModules/blob/main/NetModules/Events/GetSettingEvent.cs) is required in your `ModuleHost` project.
> See: [NetModules.Settings.LocalSettings](https://github.com/netmodules/NetModules.Settings.LocalSettings/) for more information.

Here is an example configuration file that will cache all events for 1 hour, but only cache `MyCustomModuleEvents.MyExampleEventName` event for 1 second...

```json
{
  "defaultExpire": 3600,
  "expire": {
    "NetModules.Events.LoggingEvent": 0,
    "MyCustomModuleEvents.MyExampleEventName": 1
  },
  "cacheWithMeta": true,
  "excludeMetaKeys": ["id", "handlers"],
  "cacheUniqueKeys": true
}
```

>[!TIP]
>For more advanced event caching, consider creating/using a module that implements a dedicated caching service, a dedicated framework such as [Memcached](https://www.memcached.org/), or reading/writing cache to dedicated [Redis Enterprise cache](https://redis.io/solutions/caching/) designed for caching at scale. This module can be used as a reference.

## Contributing

We welcome contributions! To get involved:
1. Fork [NetModules.Cache](https://github.com/netmodules/NetModules.Cache), make improvements, and submit a pull request.
2. Code will be reviewed upon submission.
3. Join discussions via the [issues board](https://github.com/netmodules/NetModules.Cache/issues).

## License

NetModules.Cache is licensed under the [MIT License](https://tldrlegal.com/license/mit-license), allowing unrestricted use, modification, and distribution. If you use NetModules.Cache in your own project, we’d love to hear about your experience, and possibly feature you on our website!

Full documentation coming soon!

This project references [NetTools.Cache](https://github.com/netmodules/NetTools.Cache) for strict [ICachedObject](https://github.com/netmodules/NetTools.Cache/blob/master/NetTools.Cache/Interfaces/ICachedObject.cs) and [ICacheController](https://github.com/netmodules/NetTools.Cache/blob/master/NetTools.Cache/Interfaces/ICacheController.cs) implementation.

[NetModules Foundation](https://netmodules.net/)
