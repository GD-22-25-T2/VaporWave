namespace UDK.API.Features.Enums
{
    /// <summary>
    /// All the collision shapes.
    /// </summary>
    public enum ECollisionShape
    {
        /// <summary>
        /// Refers the box collision shape.
        /// </summary>
        Box,

        /// <summary>
        /// Refers the box vertical capsule collision.
        /// </summary>
        VerticalCapsule,

        /// <summary>
        /// Refers the sphere collision.
        /// </summary>
        Sphere,

        /// <summary>
        /// Refers an invalid collision.
        /// </summary>
        Invalid,

        /// <see cref="Invalid"/>
        MAX = Invalid,
    }
}
