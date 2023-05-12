namespace UDK.API.Features.Entity.Stats.DamageHandlers
{
    using UDK.API.Features.Entity.Stats.SyncedStats;
    using UDK.API.Features.Entity.Roles.Interfaces;

    /// <summary>
    /// <para>
    /// A ready-to-play version of <seealso cref="DamageHandlerBase"/>, it's being used to implement value based handlers.
    /// <br>They carry data used to directly affect entities and stats, they can also be processed in/out.</br>
    /// </para>
    /// </summary>
    public abstract class StandardDamageHandler : DamageHandlerBase
    {
        /// <summary>
        /// The kill value.
        /// </summary>
        public const float KILL_VALUE = -1f;

        /// <summary>
        /// The damage amount.
        /// </summary>
        public abstract float Damage { get; set; }

        /// <summary>
        /// The amount of damage affecting the health.
        /// </summary>
        public float DealtHealthDamage { get; set; }

        /// <summary>
        /// The amount of damage affecting the artificial health.
        /// <br>It has priority over </br>
        /// </summary>
        public float AbsorbedAhpDamage { get; set; }

        /// <inheritdoc/>
        public override HandlerOutput ApplyDamage(EntityBase entity)
        {
            StatsBase entityStats = entity.Stats;

            if (entity.Role is IDamageableRole damageableRole && damageableRole.TargetStats != entityStats)
                return HandlerOutput.None;

            ArtificialHealthStat ahpModule = entityStats.GetModule<ArtificialHealthStat>();
            HealthStat healthModule = entityStats.GetModule<HealthStat>();

            if (Damage == -1f)
            {
                ahpModule.CurrentValue = 0f;
                healthModule.CurrentValue = 0f;
            }

            ProcessDamage(entity);

            if (Damage <= 0f)
                return HandlerOutput.None;

            float curHealthValue = healthModule.CurrentValue;
            float ahpDamage = ahpModule.ProcessDamage(Damage);
            AbsorbedAhpDamage = Damage - ahpDamage;
            DealtHealthDamage = curHealthValue - healthModule.CurrentValue;
            return healthModule.CurrentValue > 0f ? HandlerOutput.Damaged : HandlerOutput.Dead;
        }

        /// <summary>
        /// Processes the damage before dealing the specified damage amount to the target entity.
        /// </summary>
        /// <param name="entity">The target entity.</param>
        public virtual void ProcessDamage(EntityBase entity)
        {
        }
    }
}
