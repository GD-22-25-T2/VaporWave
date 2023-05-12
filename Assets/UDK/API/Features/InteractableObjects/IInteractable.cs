namespace UDK.API.Features.InteractableObjects
{
    using UDK.API.Features.Entity;
    using UDK.API.Features.InteractableObjects.Verification;

    /// <summary>
    /// Defines an interactable object.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Gets the <see cref="IVerificationRule"/>.
        /// </summary>
        IVerificationRule VerificationRule { get; }

        /// <summary>
        /// Performs an interaction.
        /// </summary>
        /// <param name="entity">The entity who's trying to perform the interaction.</param>
        /// <param name="colliderId">The target collider id.</param>
        void Interact(EntityBase entity, byte colliderId);
    }
}
