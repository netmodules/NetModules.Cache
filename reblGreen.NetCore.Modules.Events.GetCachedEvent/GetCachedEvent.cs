using System;
using System.Collections.Generic;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Events
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
        public EventName Name { get { return "reblGreen.NetCore.Modules.Events.GetCachedEvent"; } }

        
        /// <summary>
        /// The Meta dictionary can be used to hold and transfer any generic or event specific data between modules.
        /// </summary>
        public Dictionary<string, object> Meta { get; set; }

        
        /// <summary>
        /// returns true if the event was handled sucessfully.
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
