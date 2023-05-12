namespace UDK.API.Features.InteractableObjects.Verification
{
    using UDK.API.Features.Entity;
    using UnityEngine;

    /// <summary>
    /// Defines the standard verification ruleset distance-based.
    /// </summary>
    public class StandardDistanceVerification : IVerificationRule
    {
        /// <summary>
        /// The default max distance.
        /// </summary>
        public const float DEFAULT_MAX_DISTANCE = 2.42f;

        /// <summary>
        /// The interact lag compensation.
        /// </summary>
        public const float INTERACT_LAG_COMPENSATION = 1.4f;

        private readonly float _maxDistance;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardDistanceVerification"/> class.
        /// </summary>
        /// <param name="maxDistance"></param>
        public StandardDistanceVerification(float maxDistance = DEFAULT_MAX_DISTANCE) => _maxDistance = maxDistance;

        /// <summary>
        /// Gets the default <see cref="StandardDistanceVerification"/>.
        /// </summary>

        public static StandardDistanceVerification Default { get; } = new(DEFAULT_MAX_DISTANCE);

        /// <inheritdoc/>
        public virtual bool CanInteract(EntityBase entity, InteractableCollider collider, RaycastHit hit)
        {
            if (hit.distance >= _maxDistance || !entity.TryGetComponent(out InteractionCoordinator interactionCoordinator) ||
                interactionCoordinator.AnyBlocker((IInteractionBlocker blocker) => blocker.BlockedInteractions > 0))
                return false;

            Transform transform = collider.transform;
            return !(Vector3.Distance(entity.Position, transform.position + transform.TransformDirection(collider.VerificationOffset)) > _maxDistance * 1.4f);
        }
    }
}
