namespace UDK.API.Features.Inventory.Pickups
{
    using System;

    /// <summary>
    /// All the pickup flags.
    /// </summary>
    [Flags]
    public enum PickupFlags : byte
    {
        /// <summary>
        /// Means the pickup is locked.
        /// </summary>
        Locked = 1,

        /// <summary>
        /// Means the pickup is in use.
        /// </summary>
        InUse = 2,
    }
}
