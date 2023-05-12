namespace UDK.API.Features.Inventory
{
    using System;
    using System.Collections.Generic;
    using UDK.API.Features.Inventory.Items;
    using UnityEngine;

    /// <summary>
    /// The helper class which loads all items.
    /// </summary>
    public static class InventoryItemLoader 
    {
        /// <summary>
        /// The resource directory.
        /// </summary>
        public const string ITEMS_DIRECTORY_NAME = "Items";

        private static bool _isLoaded;
        private static Dictionary<sbyte, ItemBase> _loadedItems;

        /// <summary>
        /// Gets the available items.
        /// </summary>
        public static Dictionary<sbyte, ItemBase> AvailableItems
        {
            get
            {
                if (!_isLoaded)
                    ForceReload();

                return _loadedItems;
            }
        }

        /// <summary>
        /// Forces the resources reload.
        /// </summary>
        public static void ForceReload()
        {
            try
            {
                _loadedItems = new();

                ItemBase[] items = Resources.LoadAll<ItemBase>(ITEMS_DIRECTORY_NAME);
                Array.Sort<ItemBase>(items, delegate (ItemBase x, ItemBase y)
                {
                    sbyte typeId = x.ItemTypeId;
                    return typeId.CompareTo(y.ItemTypeId);
                });

                foreach (ItemBase item in items)
                {
                    _loadedItems[item.ItemTypeId] = item;
                    item.TemplateReloadedDispatcher.InvokeAll(_isLoaded);
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error whilst loading items from the resources folder: {ex.Message}");
                _isLoaded = false;
            }
        }
    }
}
