namespace UDK.API.Features.Inventory.Items
{
    using System;

    /// <summary>
    /// A struct which identifies <see cref="ItemBase"/> instances.
    /// </summary>
    [Serializable]
    public readonly struct ItemIdentifier : IEquatable<ItemIdentifier>
    {
        public static readonly ItemIdentifier None = new(0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemIdentifier"/> struct.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="serial">The item's serial.</param>
        public ItemIdentifier(sbyte type, ushort serial)
        {
            TypeId = type;
            Serial = serial;
        }

        /// <summary>
        /// Gets the item type.
        /// </summary>
        public readonly sbyte TypeId { get; }

        /// <summary>
        /// Gets the item's serial.
        /// </summary>
        public readonly ushort Serial { get; }

        /// <inheritdoc cref="Serial"/>
        public override int GetHashCode() => (int)Serial;

        /// <summary>
        /// Compares two operands: <see cref="ItemIdentifier"/> and <see cref="ItemIdentifier"/>.
        /// </summary>
        /// <param name="left">The right-hand <see cref="ItemIdentifier"/> to compare.</param>
        /// <param name="right">The left-hand <see cref="ItemIdentifier"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(ItemIdentifier left, ItemIdentifier right) => left.Equals(right);

        /// <summary>
        /// Compares two operands: <see cref="ItemIdentifier"/> and <see cref="ItemIdentifier"/>.
        /// </summary>
        /// <param name="left">The right-hand <see cref="ItemIdentifier"/> to compare.</param>
        /// <param name="right">The left-hand <see cref="ItemIdentifier"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(ItemIdentifier left, ItemIdentifier right) => !left.Equals(right);

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="ItemIdentifier"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="ItemIdentifier"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the <see cref="ItemIdentifier"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public bool Equals(ItemIdentifier other) => Serial == other.Serial && TypeId == other.TypeId;

        /// <summary>
        /// Indicates whether this instance and a specified <see cref="object"/> are equal.
        /// </summary>
        /// <param name="other">The <see cref="object"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if the <see cref="object"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is ItemIdentifier && Equals((ItemIdentifier)obj);
    }
}
