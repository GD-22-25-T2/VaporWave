namespace UDK.API.Features.Enums
{
    /// <summary>
    /// A set of default movement states.
    /// </summary>
    public enum EMovementState : sbyte
    {
        /// <summary>
        /// Means the pawn's currently idling.
        /// </summary>
        None,

        /// <summary>
        /// Means the pawn's currently jogging.
        /// </summary>
        Jogging,

        /// <summary>
        /// Means the pawn's currently sprinting.
        /// </summary>
        Sprinting,

        /// <summary>
        /// Means the pawn's currently crouching.
        /// </summary>
        Crouching,

        /// <summary>
        /// Means the pawn's currently airborne.
        /// </summary>
        Airborne,

        /// <summary>
        /// Means the pawn's currently vaulting.
        /// </summary>
        Vaulting,

        /// <summary>
        /// Means the pawn's currently crawling.
        /// </summary>
        Crawling,

        /// <summary>
        /// Means the pawn's currently buoyant.
        /// </summary>
        Buoyant,

        /// <summary>
        /// Means the pawn's currently walking.
        /// </summary>
        Walking,

        /// <summary>
        /// Means the pawn's currently sliding.
        /// </summary>
        Sliding,

        /// <summary>
        /// Means the pawn's currently dashing.
        /// </summary>
        Dashing,

        /// <summary>
        /// Means the pawn's currently dodging.
        /// </summary>
        Dodging,

        /// <summary>
        /// Means the pawn's currently rolling.
        /// </summary>
        Rolling,

        /// <see cref="Airborne"/>
        MAX = Rolling,
    }
}
