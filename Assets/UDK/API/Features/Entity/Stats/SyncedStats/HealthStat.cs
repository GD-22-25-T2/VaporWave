namespace UDK.API.Features.Entity.Stats.SyncedStats
{
    using UDK.API.Features.Entity.Roles.Interfaces;

    /// <summary>
    /// Defines a stat module which implements a health layer.
    /// </summary>
    public class HealthStat : SyncedStatBase
    {
        /// <inheritdoc/>
        public override float MinValue => 0f;

        /// <inheritdoc/>
        public override float MaxValue => CustomMaxValue > 0f ? CustomMaxValue : Owner.Cast<EntityBase>().RoleManager.CurrentRole is IDamageableRole damageableRole ? damageableRole.MaxHealth : 0f;
    }
}
