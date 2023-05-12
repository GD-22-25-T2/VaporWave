namespace UDK.API.Features.InteractableObjects
{
    using System.Collections.Generic;
    using UDK.API.Features.Core;
    using UnityEngine;

    /// <summary>
    /// The behavior of the interactable colliders.
    /// </summary>
    public class InteractableCollider : Actor
    {
        private static readonly Dictionary<IInteractable, Dictionary<byte, InteractableCollider>> _allInstances = new();

        /// <summary>
        /// Gets a <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing all the active instances.
        /// </summary>
        public static IReadOnlyDictionary<IInteractable, Dictionary<byte, InteractableCollider>> AllInstances => _allInstances;

        /// <summary>
        /// Gets or sets the interactable target.
        /// </summary>
        public Actor Target { get; set; }

        /// <summary>
        /// Gets or sets the collider id.
        /// </summary>
        public byte ColliderId { get; set; }

        /// <summary>
        /// Gets or sets the verification offset position.
        /// </summary>
        public Vector3 VerificationOffset { get; set; }

        /// <summary>
        /// Tries to get a collider given the specified parameters.
        /// </summary>
        /// <param name="target">The interactable target.</param>
        /// <param name="colliderId">The collider id.</param>
        /// <param name="result">The found <see cref="InteractableCollider"/> object.</param>
        /// <returns><see langword="true"/> if the collider was found successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetCollider(IInteractable target, byte colliderId, out InteractableCollider result)
        {
            result = null;
            return AllInstances.TryGetValue(target, out Dictionary<byte, InteractableCollider> dict) && dict.TryGetValue(colliderId, out result);
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            if (Target.Cast(out IInteractable key))
            {
                if (_allInstances.ContainsKey(key))
                    _allInstances[key] = new();

                _allInstances[key][ColliderId] = this;
                return;
            }
        }
    }
}
