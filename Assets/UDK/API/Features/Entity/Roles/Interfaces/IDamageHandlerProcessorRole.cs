namespace UDK.API.Features.Entity.Roles.Interfaces
{
    using UDK.API.Features.Entity.Stats.DamageHandlers;

    /// <summary>
    /// Defines the base contract for roles with custom damage handler proccessors.
    /// </summary>
    public interface IDamageHandlerProcessorRole
    {
        /// <summary>
        /// Processes the given <see cref="DamageHandlerBase"/>.
        /// </summary>
        /// <param name="damageHandlerBase">The damage handler to process.</param>
        /// <returns>The processed <see cref="DamageHandlerBase"/>.</returns>
        public DamageHandlerBase ProcessDamageHandler(DamageHandlerBase damageHandlerBase);
    }
}
