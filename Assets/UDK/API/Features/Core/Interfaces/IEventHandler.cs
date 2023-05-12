namespace UDK.API.Features.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for basic event handler features.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// Subscribes to all the events.
        /// </summary>
        void SubscribeEvents();

        /// <summary>
        /// Unsubscribes from all the events.
        /// </summary>
        void UnsubscribeEvents();
    }
}
