﻿using System;
using System.Collections.Generic;
using System.Threading;
using NetModules;
using NetModules.Interfaces;
using NetModules.Cache.Events;
using NetModules.Events;
using NetTools.Serialization;

namespace NetModules.Cache.MemoryCache.TestApplication
{
    class Program
    {
        static EventWaitHandle BlockingHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        static void Main(string[] args)
        {
            ModuleHost host = new BasicModuleHost();
            host.Modules.LoadModules();

            var myModule = host.Modules.GetModulesByType<MemoryCacheModule>();

            if (myModule.Count > 0)
            {
                myModule[0].Log(LoggingEvent.Severity.Debug, "DebugLogTest");

                var now = DateTime.UtcNow;
                var dummy = new DummyCachedEvent()
                {
                    Output = new DummyCachedEventOutput()
                    {
                        CachedTime = now
                    }
                };

                dummy.Input.CachedTime = now;
                host.Handle(dummy);
                var then = DateTime.UtcNow;

                var getCached = new GetCachedEvent()
                {
                    Input = new GetCachedEventInput()
                    {
                        EventName = dummy.Name,
                        EventInput = dummy.Input
                    }
                };
                host.Handle(getCached);

                var thenAgain = DateTime.UtcNow;

                Console.WriteLine("Testing dummy cached event...");
                Console.WriteLine($"Name: {dummy.Name}");
                Console.WriteLine($"Input: {dummy.Input.ToJson()}");
                Console.WriteLine($"Metadata: {dummy.Meta.ToJson()}");
                Console.WriteLine();
                Console.WriteLine((getCached.Output.EventOutput as DummyCachedEventOutput).CachedTime);
                Console.WriteLine("Set in: " + then.Subtract(now));
                Console.WriteLine("Retrieved in: " + thenAgain.Subtract(then));
            }


            var getSetting1 = new GetSettingEvent()
            {
                Input = new GetSettingEventInput()
                {
                    ModuleName = myModule[0].ModuleAttributes.Name,
                    SettingName = "defaultExpire"
                }
            };

            host.Handle(getSetting1);


            var getSetting2 = new GetSettingEvent()
            {
                Input = new GetSettingEventInput()
                {
                    ModuleName = myModule[0].ModuleAttributes.Name,
                    SettingName = "defaultExpire"
                }
            };

            host.Handle(getSetting2);

            BlockingHandle.WaitOne();
        }
    }
}
