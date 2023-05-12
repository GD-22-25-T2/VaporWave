namespace UDK.API.Features.Entity.Stats.DamageHandlers
{
    using UDK.API.Features.Core.Generic;

    /// <summary>
    /// The base implementation of damage handlers.
    /// <para>
    /// A <see cref="DamageHandlerBase"/> affects entities and stats.
    /// <br>It provides a poor API as is, due to how the <see cref="Stats"/> works.</br>
    /// <br>The damage handlers can be implemented to affect entities and stats in a specific way.</br>
    /// <br>A ready-to-play version exists as <seealso cref="StandardDamageHandler"/>, it's being used to implement value based handlers.</br>
    /// <br>They carry data used to directly affect entities and stats, they can also be processed in/out.</br>
    /// </para>
    /// <para>
    /// The current implementation is not network replicated.
    /// </para>
    /// </summary>
    public abstract class DamageHandlerBase : TypeCastObject<DamageHandlerBase>
    {
        /// <summary>
        /// The handler output.
        /// </summary>
        public enum HandlerOutput : byte
        {
            /// <summary>
            /// Means the output is none.
            /// </summary>
            None,

            /// <summary>
            /// Means the output is damaged.
            /// </summary>
            Damaged,

            /// <summary>
            /// Means the output is dead.
            /// </summary>
            Dead,
        }

        /// <summary>
        /// Applies damage to the specified entity.
        /// </summary>
        /// <param name="entity">The entity to apply damage to.</param>
        /// <returns>The relative <see cref="HandlerOutput"/>.</returns>
        public abstract HandlerOutput ApplyDamage(EntityBase entity);
    }
}
