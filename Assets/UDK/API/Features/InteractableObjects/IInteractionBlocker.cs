namespace UDK.API.Features.InteractableObjects
{
    /// <summary>
    /// Defines an interaction blocker behavior.
    /// </summary>
    public interface IInteractionBlocker
    {
        /// <summary>
        /// Gets the blocked interactions.
        /// </summary>
        sbyte BlockedInteractions { get; }

        /// <summary>
        /// Gets a value indicating whether the interaction can be cleared.
        /// </summary>
        bool CanBeCleared { get; }
    }
}
