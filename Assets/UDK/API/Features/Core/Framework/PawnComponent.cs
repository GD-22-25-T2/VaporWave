namespace UDK.API.Features.Core
{
    using UnityEngine;
    using UDK.API.Features.Core.Framework;

    /// <summary>
    /// A component which is being handled by a <see cref="Pawn"/>.
    /// <br>A <see cref="Pawn"/> will be automatically found, if possible, and assigned to the corresponding <see cref="Owner"/>.</br>
    /// </summary>
    [AddComponentMenu("UDK/Core/PawnComponent")]
    [ExecuteAlways]
    public abstract class PawnComponent : ActorComponent
    {
        /// <summary>
        /// Gets the owner.
        /// </summary>
        public new Pawn Owner { get; protected set; }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <typeparam name="T">The type of the pawn.</typeparam>
        /// <returns>The owner.</returns>
        public new T GetOwner<T>()
            where T : Pawn => Owner.Cast<T>();

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            Owner = GetComponentSafe<Pawn>();
        }
    }
}
