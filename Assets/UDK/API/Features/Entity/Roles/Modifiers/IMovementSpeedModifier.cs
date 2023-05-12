namespace UDK.API.Features.Entity.Roles.Modifiers
{
    /// <summary>
    /// Defines the contract for basic speed modifier features.
    /// </summary>
    public interface IMovementSpeedModifier : IRoleModifier
    {
        /// <summary>
        /// Gets or sets a value indicating whether the movement speed modifier is active.
        /// </summary>
        bool MovementModifierActive { get; }

        /// <summary>
        /// Gets or sets the speed multiplier.
        /// </summary>
        float MovementSpeedMultiplier { get; }

        /// <summary>
        /// Gets or sets the speed limit.
        /// </summary>
        float MovementSpeedLimit { get; }

        /// <summary>
        /// Gets or sets the acceleration multiplier.
        /// </summary>
        float MovementAccelerationMultiplier { get; }
    }
}
