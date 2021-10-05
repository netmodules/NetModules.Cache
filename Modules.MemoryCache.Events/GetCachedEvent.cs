using System;
using System.Collections.Generic;
using reblGreen.NetCore.Modules;
using reblGreen.NetCore.Modules.Interfaces;

namespace Modules.MemoryCache.Events
{
    /// <summary>
    /// This event can be handled by event caching modules to return a cached event output.
    /// Event caching modules should handle event output caching automatically but should also handle this event
    /// directly if the event is raised by another module.
    /// </summary>
    public class GetCachedEvent : IEvent<GetCachedEventInput, GetCachedEventOutput>
    {
        /// <summary>
        /// The properties in this object are used to create an identifier which is used to lookup a cached output.
        /// </summary>
        public GetCachedEventInput Input { get; set; } = new GetCachedEventInput();


        /// <summary>
        /// If a cached object is found for the values entered in <see cref="GetCachedEventInput"/> it is
        /// returned to the requester via the <see cref="GetCachedEventOutput.EventOutput"/> property.
        /// </summary>
        public GetCachedEventOutput Output { get; set; }


        /// <summary>
        /// A unique name which can be used to identify the event type where the concrete type of the IEvent object
        /// is unknown.
        /// </summary>
        public EventName Name { get { return "MemoryCache.GetCachedEvent"; } }


        /// <summary>
        /// The Meta dictionary can be used to hold and transfer any generic or event specific data between modules.
        /// </summary>
        public Dictionary<string, object> Meta { get; set; }


        /// <summary>
        /// returns true if the event was handled sucessfully.
        /// </summary>
        public bool Handled { get; set; }


        /// <summary>
        /// It has been required by modules which are designed to handle generic type of IEvent and need access to IModuleEvent.Input when the generic
        /// type definition of the IEvent{} may be unknown at runtime and strict casting is unavailable. We must expose Input and Output objects via
        /// non-generic IEvent interface.
        /// </summary>
        public IEventInput GetEventInput()
        {
            return Input;
        }


        /// <summary>
        /// It has been required by modules which are designed to handle generic type of IEvent and need access to IModuleEvent.Output when the generic
        /// type definition of the IEvent{} may be unknown at runtime and strict casting is unavailable. We must expose Input and Output objects via
        /// non-generic IEvent interface.
        /// </summary>
        public IEventOutput GetEventOutput()
        {
            return Output;
        }


        /// <summary>
        /// It has been required by modules which are designed to handle generic type of IEvent and need access to set IModuleEvent.Output when the generic
        /// type definition of IEvent{} may be unknown at runtime and strict casting is unavailable. We must expose a method to set Output object via the
        /// non-generic IEvent interface.
        /// </summary>
        public void SetEventOutput(IEventOutput output)
        {
            if (output is GetCachedEventOutput o)
            {
                Output = o;
            }
        }
    }
}