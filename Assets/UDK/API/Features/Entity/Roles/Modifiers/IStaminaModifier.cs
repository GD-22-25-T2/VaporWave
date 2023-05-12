namespace UDK.API.Features.Entity.Roles.Modifiers
{
    /// <summary>
    /// Defines the contract for basic stamina modifier features.
    /// </summary>
    public interface IStaminaModifier : IRoleModifier
    {
        /// <summary>
        /// Gets or sets a value indicating whether the stamina modifier is active.
        /// </summary>
        bool StaminaModifierActive { get; }

        /// <summary>
        /// Gets or sets the usage multiplier.
        /// </summary>
        float StaminaUsageMultiplier { get; }

        /// <summary>
        /// Gets or sets the regeneration multiplier.
        /// </summary>
        float StaminaRegenMultiplier { get; }

        /// <summary>
        /// Gets or sets a value indicating whether sprinting is disabled.
        /// </summary>
        bool SprintingDisabled { get; }
    }
}
