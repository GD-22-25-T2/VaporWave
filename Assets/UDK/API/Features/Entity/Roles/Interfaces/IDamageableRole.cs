namespace UDK.API.Features.Entity.Roles.Interfaces
{
    using UDK.API.Features.Entity.Stats;

    /// <summary>
    /// Defines the base contract for damageable roles.
    /// </summary>
    public interface IDamageableRole
    {
        /// <summary>
        /// Gets the max health.
        /// </summary>
        public float MaxHealth { get; }

        /// <summary>
        /// Gets the stats.
        /// </summary>
        public StatsBase TargetStats { get; }
    }
}
