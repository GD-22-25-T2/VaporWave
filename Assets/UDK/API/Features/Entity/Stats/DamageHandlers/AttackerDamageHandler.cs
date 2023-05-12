namespace UDK.API.Features.Entity.Stats.DamageHandlers
{
    /// <summary>
    /// The damage handler which handles an entity acting as damage dealer.
    /// </summary>
    public abstract class AttackerDamageHandler : StandardDamageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackerDamageHandler"/> class.
        /// </summary>
        /// <param name="entityBase">The damage dealer.</param>
        /// <param name="damage">The dealt damage.</param>
        public AttackerDamageHandler(EntityBase entityBase, float damage)
        {
            Attacker = entityBase;
            Damage = damage;
        }

        /// <inheritdoc/>
        public override float Damage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the damage handler is self damageable.
        /// </summary>
        public abstract bool IsSelfDamageable { get; set; }

        /// <summary>
        /// Gets or sets the damage dealer.
        /// </summary>
        public EntityBase Attacker { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the damage handler is handling a suicide.
        /// </summary>
        public bool IsSuicide { get; set; }

        /// <inheritdoc/>
        public override void ProcessDamage(EntityBase entity)
        {
            base.ProcessDamage(entity);

            if (entity == Attacker)
            {
                if (!IsSelfDamageable)
                {
                    Damage = 0f;
                    return;
                }

                IsSuicide = true;
            }

            base.ProcessDamage(entity);
        }
    }
}
