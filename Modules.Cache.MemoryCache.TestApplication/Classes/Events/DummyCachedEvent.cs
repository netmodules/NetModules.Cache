using System;
using System.Collections.Generic;
using NetModules;
using NetModules.Interfaces;

namespace Modules.Cache.MemoryCache.TestApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class DummyCachedEvent : IEvent<DummyCachedEventInput, DummyCachedEventOutput>
    {
        /// <summary>
        /// 
        /// </summary>
        public DummyCachedEventInput Input { get; set; } = new DummyCachedEventInput();


        /// <summary>
        /// 
        /// </summary>
        public DummyCachedEventOutput Output { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public EventName Name { get { return "NetModules.Events.DummyCachedEvent"; } }


        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> Meta { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool Handled { get; set; }


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to IModuleEvent.Input where available.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done with <see cref="System.Reflection"/> or more efficiently using dynamic casting.
        /// Eg. return ((dynamic)this).Input as IEventInput;
        /// </summary>
        public IEventInput GetEventInput()
        {
            return ((dynamic)this).Input as IEventInput;
        }


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to IModuleEvent.Output where available.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done with <see cref="System.Reflection"/> or more efficiently using dynamic casting.
        /// Eg. return ((dynamic)this).Output as IEventOutput;
        /// </summary>
        public IEventOutput GetEventOutput()
        {
            return ((dynamic)this).Output as IEventOutput;
        }


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to the setter for IModuleEvent.Output.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done with <see cref="System.Reflection"/>.
        /// </summary>
        public void SetEventOutput(IEventOutput output)
        {
            // This purposely assumes that output exists on the event type and that typeof(output) is equal to typeof(this.Output).
            // It will throw an exception where any of the above conditions fail. This is by design.
            GetType().GetProperty("Output").SetValue(this, output);
        }
    }
}
