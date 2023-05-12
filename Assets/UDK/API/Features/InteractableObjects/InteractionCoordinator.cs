namespace UDK.API.Features.InteractableObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UDK.API.Features.Core;
    using UDK.API.Features.Entity;
    using UDK.API.Features.InteractableObjects.Verification;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// The behavior of the interactable colliders.
    /// </summary>
    public class InteractionCoordinator : PawnComponent
    {
        private readonly HashSet<IInteractionBlocker> _blockers = new();

        /// <summary>
        /// Safely gets the verification rule out of an <see cref="IInteractable"/> object.
        /// </summary>
        /// <param name="interactable">The target interactable.</param>
        /// <returns>The corresponding verification rule, or <see cref="StandardDistanceVerification.Default"/> if not valid.</returns>
        public static IVerificationRule GetRule_Safe(IInteractable interactable) => interactable.VerificationRule ?? StandardDistanceVerification.Default;

        /// <summary>
        /// Adds an <see cref="IInteractionBlocker"/>.
        /// </summary>
        /// <param name="blocker">The blocker to be added.</param>
        public void AddBlocker(IInteractionBlocker blocker) => _blockers.Add(blocker);

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="interactions">A function parameter to test each element for a condition.</param>
        /// <returns><see langword="true"/> if any elements in the source sequence pass the test in the specified predicate; otherwise, <see langword="false"/>.</returns>
        public bool AnyBlocker(sbyte interactions) => AnyBlocker((IInteractionBlocker blocker) => (blocker.BlockedInteractions & interactions) == interactions);

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="func">A function to test each element for a condition.</param>
        /// <returns><see langword="true"/> if any elements in the source sequence pass the test in the specified predicate; otherwise, <see langword="false"/>.</returns>
        public bool AnyBlocker(Func<IInteractionBlocker, bool> func)
        {
            _blockers.RemoveWhere((IInteractionBlocker entry) =>
            {
                Object @object;
                return ((@object = (entry as Object)) && !@object) || entry == null || entry.CanBeCleared;
            });

            return _blockers.Any(func);
        }

        /// <summary>
        /// Performs an interaction when possible.
        /// </summary>
        /// <param name="targetInteractable">The target interactable object.</param>
        /// <param name="colliderId">The target collider id.</param>
        /// <param name="hit">The interaction hit.</param>
        public void Interact(IInteractable targetInteractable, byte colliderId, RaycastHit hit)
        {
            if (!InteractableCollider.TryGetCollider(targetInteractable, colliderId, out InteractableCollider result) ||
                !GetRule_Safe(targetInteractable).CanInteract(Owner.Cast<EntityBase>(), result, hit))
                return;

            targetInteractable.Interact(Owner.Cast<EntityBase>(), colliderId);
        }
    }
}
