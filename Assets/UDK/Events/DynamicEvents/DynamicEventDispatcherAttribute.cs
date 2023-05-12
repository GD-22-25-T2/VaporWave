namespace UDK.Events.DynamicEvents
{
    using System;

    /// <summary>
    /// An attribute to easily manage <see cref="DynamicEventDispatcher"/> initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DynamicEventDispatcherAttribute : Attribute
    {
    }
}