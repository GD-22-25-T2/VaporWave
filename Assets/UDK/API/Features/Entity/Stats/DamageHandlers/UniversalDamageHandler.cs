namespace UDK.API.Features.Entity.Stats.DamageHandlers
{
    /// <summary>
    /// The universal damage handler used to handle generic damage actions.
    /// </summary>
    public class UniversalDamageHandler : AttackerDamageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalDamageHandler"/> class.
        /// </summary>
        /// <param name="entityBase">The damage dealer.</param>
        /// <param name="damage">The dealt damage.</param>
        public UniversalDamageHandler(EntityBase entityBase, float damage)
            : base(entityBase, damage)
        {
        }

        /// <inheritdoc/>
        public override bool IsSelfDamageable { get; set; } = true;

        /// <inheritdoc/>
        public override float Damage { get; set; }
    }
}
