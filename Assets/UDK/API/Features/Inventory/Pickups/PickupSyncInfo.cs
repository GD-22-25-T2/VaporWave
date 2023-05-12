namespace UDK.API.Features.Inventory.Pickups
{
    using System;
    using UDK.API.Features.Inventory.Items;
    using UnityEngine;

    /// <summary>
    /// A struct which handles information regarding pickups.
    /// </summary>
    [Serializable]
    public struct PickupSyncInfo : IEquatable<PickupSyncInfo>
    {
        /// <summary>
        /// Represents the default pickup.
        /// </summary>
        public static readonly PickupSyncInfo None = new(0, default, default, 0f, 0);

        #region Editor

        [SerializeField]
        private sbyte itemId;

        [SerializeField]
        private ushort serial;

        [SerializeField]
        private float weight;

        [SerializeField]
        private Vector3 position;

        [SerializeField]
        private Quaternion rotation;

        [SerializeField]
        private PickupFlags flags;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PickupSyncInfo"/> struct.
        /// </summary>
        /// <param name="id"><inheritdoc cref="ItemId"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="rotation"><inheritdoc cref="Rotation"/></param>
        /// <param name="weight"><inheritdoc cref="Weight"/></param>
        /// <param name="serial"><inheritdoc cref="Serial"/></param>
        public PickupSyncInfo(sbyte id, Vector3 position_t, Quaternion rotation_t, float weight_t, ushort serial_t = 0)
        {
            itemId = id;
            weight = weight_t;
            flags = 0;
            position = position_t;
            rotation = rotation_t;
            serial = ((serial_t == 0) ? ItemSerialGenerator.GenerateNext() : serial_t);
        }

        /// <summary>
        /// Gets or sets the type of the pickup.
        /// </summary>
        public sbyte ItemId
        {
            get => itemId;
            set => itemId = value;
        }

        /// <summary>
        /// Gets or sets the pickup's serial.
        /// </summary>
        public ushort Serial
        {
            get => serial;
            set => serial = value;
        }

        /// <summary>
        /// Gets or sets the pickup's weight.
        /// </summary>
        public float Weight
        {
            get => weight;
            set => weight = value;
        }

        /// <summary>
        /// Gets or sets the pickup's position.
        /// </summary>
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }
        
        /// <summary>
        /// Gets or sets the pickup's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="PickupFlags"/>.
        /// </summary>
        public PickupFlags Flags
        {
            get => flags;
            set => flags = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is locked.
        /// </summary>
        public bool Locked
        {
            get => (Flags & PickupFlags.Locked) == PickupFlags.Locked;
            set => Flags = (value ? (Flags | PickupFlags.Locked) : (Flags & ~PickupFlags.Locked));
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is in use.
        /// </summary>
        public bool InUse
        {
            get => (Flags & PickupFlags.InUse) == PickupFlags.InUse;
            set => Flags = (value ? (Flags | PickupFlags.InUse) : (Flags & ~PickupFlags.InUse));
        }

        /// <summary>
        /// Sets the position and rotation of the pickup.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="PickupSyncInfo"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="PickupSyncInfo"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the <see cref="PickupSyncInfo"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public bool Equals(PickupSyncInfo other) =>
            other.ItemId == ItemId && other.Weight == Weight &&
            other.Position == Position && other.Rotation == Rotation
            && other.Flags == Flags;

        /// <summary>
        /// Compares two operands: <see cref="PickupSyncInfo"/> and <see cref="PickupSyncInfo"/>.
        /// </summary>
        /// <param name="left">The right-hand <see cref="PickupSyncInfo"/> to compare.</param>
        /// <param name="right">The left-hand <see cref="PickupSyncInfo"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(PickupSyncInfo left, PickupSyncInfo right) => left.Equals(right);

        /// <summary>
        /// Compares two operands: <see cref="PickupSyncInfo"/> and <see cref="PickupSyncInfo"/>.
        /// </summary>
        /// <param name="left">The right-hand <see cref="PickupSyncInfo"/> to compare.</param>
        /// <param name="right">The left-hand <see cref="PickupSyncInfo"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(PickupSyncInfo left, PickupSyncInfo right) => !left.Equals(right);

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="object"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="object"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the <see cref="object"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object other)
        {
            if (other is PickupSyncInfo)
            {
                PickupSyncInfo obj = (PickupSyncInfo)other;
                return Equals(obj);
            }

            return false;
        }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer representing the hash code for this instance.</returns>
        public override int GetHashCode() => Serial == 0 ? ((ItemId * 398 ^ Position.GetHashCode()) * 397 ^ Rotation.GetHashCode()) * 397 ^ (int)Flags : Serial;
    }
}
