namespace UDK.API.Features.Inventory.Items.Modifiers
{
    /// <summary>
    /// Defines the contract for basic equip and dequip features.
    /// </summary>
    public interface IEquipDequipModifier
    {
        /// <summary>
        /// Gets a value indicating whether the item can be holstered.
        /// </summary>
        bool AllowHolster { get; }

        /// <summary>
        /// Gets a value indicating whether the item can be equipped.
        /// </summary>
        bool AllowEquip { get; }
    }
}
