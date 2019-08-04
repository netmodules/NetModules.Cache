using reblGreen.NetCore.Modules.Events;
using System;
using System.Threading;

namespace reblGreen.NetCore.Modules.MemoryCache.TestApplication
{
    class Program
    {
        static EventWaitHandle BlockingHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        static void Main(string[] args)
        {
            ModuleHost host = new BasicModuleHost();
            host.Modules.LoadModules();

            var myModule = host.Modules.GetModulesByType<CacheModule>();

            if (myModule.Count > 0)
            {
                myModule[0].GetSetting("testInt", 0);
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

                var getCached = new GetCachedEvent();
                getCached.Input.EventName = dummy.Name;
                getCached.Input.EventInput = dummy.Input;
                host.Handle(getCached);

                var thenAgain = DateTime.UtcNow;

                Console.WriteLine((getCached.Output.EventOutput as DummyCachedEventOutput).CachedTime);
                Console.WriteLine("Set in: " + then.Subtract(now));
                Console.WriteLine("Retrieved in: " + thenAgain.Subtract(then));
            }

            BlockingHandle.WaitOne();
        }
    }
}
