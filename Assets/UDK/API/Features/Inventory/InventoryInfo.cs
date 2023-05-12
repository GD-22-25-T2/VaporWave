namespace UDK.API.Features.Inventory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UDK.API.Features.Inventory.Items;
    using UnityEngine;

    [Serializable]
    /// <summary>
    /// A struct which handles information regarding inventory.
    /// </summary>
    public class InventoryInfo
    {
        [SerializeField]
        private List<ItemBase> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryInfo"/> struct.
        /// </summary>
        /// <param name="maxSlots"><inheritdoc cref="MaxSlots"/></param>
        public InventoryInfo(uint maxSlots)
        {
            MaxSlots = maxSlots;
            _items = new();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IEnumerable<ItemBase> Items
        {
            get => _items;
            set => _items = value.Distinct().ToList();
        }

        /// <summary>
        /// Gets the maximum slots.
        /// </summary>
        public uint MaxSlots { get; }

        /// <summary>
        /// Adds an item from user's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <returns><see langword="true"/> if the item was added successfully; otherwise, <see langword="false"/>.</returns>
        public bool AddItem(ItemBase item)
        {
            if (!item || _items.Contains(item))
                return false;

            _items.Add(item);
            return true;
        }

        /// <summary>
        /// Removes an item from user's inventory.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns><see langword="true"/> if the item was removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveItem(ItemBase item)
        {
            if (!item || !_items.Contains(item))
                return false;

            _items.Remove(item);
            return true;
        }

        /// <summary>
        /// Removes an item from user's inventory.
        /// </summary>
        /// <param name="serial">The item's serial to be removed.</param>
        /// <returns><see langword="true"/> if the item was removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveItem(ushort serial) => RemoveItem(Items.FirstOrDefault(i => i.Serial == serial));
    }
}
