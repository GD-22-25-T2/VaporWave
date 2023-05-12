namespace UDK.API.Features.Core
{
    using UDK.API.Features.Core.Interfaces;
    using UnityEngine;

    /// <summary>
    /// A component which is being handled by an <see cref="Actor"/>.
    /// <br>As this is the base component implementation, <see cref="Owner"/> has to be manually initialized on <see cref="Actor.OnPostInitialize"/>.</br>
    /// </summary>
    [AddComponentMenu("UDK/Core/ActorComponent")]
    [ExecuteAlways]
    public abstract class ActorComponent : Actor, IActorComponent<Actor>
    {
        /// <summary>
        /// Gets the owner.
        /// </summary>
        public Actor Owner { get; protected set; }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <typeparam name="T">The type of the pawn.</typeparam>
        /// <returns>The owner.</returns>
        public T GetOwner<T>()
            where T : Actor => Owner.Cast<T>();
    }
}
