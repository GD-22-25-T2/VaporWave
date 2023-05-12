namespace UDK.API.Features.InteractableObjects.Verification
{
    using UDK.API.Features.Entity;
    using UnityEngine;

    /// <summary>
    /// Classifies the interaction verification ruleset.
    /// </summary>
    public interface IVerificationRule
    {
        /// <summary>
        /// Checks whether the interaction can be performed.
        /// </summary>
        /// <param name="entity">The target entity.</param>
        /// <param name="collider">The target collider.</param>
        /// <param name="hit">The target hit.</param>
        /// <returns><see langword="true"/> if the interaction can be performed; otherwise <see langword="false"/>.</returns>
        bool CanInteract(EntityBase entity, InteractableCollider collider, RaycastHit hit);
    }
}
