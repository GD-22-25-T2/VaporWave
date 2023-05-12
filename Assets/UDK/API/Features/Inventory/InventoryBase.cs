namespace UDK.API.Features.Inventory
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using UDK.API.Features.Core;
    using UDK.API.Features.Entity;
    using UDK.API.Features.Entity.Roles.Modifiers;
    using UDK.API.Features.Inventory.Items;
    using UDK.API.Features.Inventory.Pickups;
    using UnityEngine;
    using static Constants;
    using UDK.MEC;

    /// <summary>
    /// The base inventory component which handles all the items and pickups.
    /// </summary>
    [AddComponentMenu("UDK/Inventory/InventoryBase")]
    [DisallowMultipleComponent]
    public class InventoryBase : PawnComponent, IStaminaModifier, IMovementSpeedModifier
    {
        private readonly Stopwatch lastEquipSw = Stopwatch.StartNew();
        private ItemBase curInstance;

        /// <summary>
        /// Invoked when the current item is changed.
        /// </summary>
        public static event Action<EntityBase, ItemIdentifier, ItemIdentifier> OnCurrentItemChanged;

        /// <summary>
        /// Invoked when an item is added. 
        /// </summary>
        public static event Action<EntityBase, ItemBase, ItemPickupBase> OnItemAdded;

        /// <summary>
        /// Invoked when an item is removed.
        /// </summary>
        public static event Action<EntityBase, ItemBase, ItemPickupBase> OnItemRemoved;

        static InventoryBase()
        {
            OnCurrentItemChanged = (EntityBase, PreviousItem, NewItem) => { };
            OnItemAdded = (EntityBase, PreviousItem, NewItem) => { };
            OnItemRemoved = (EntityBase, PreviousItem, NewItem) => { };
        }

        /// <summary>
        /// Gets or sets the current <see cref="ItemBase"/> instance.
        /// </summary>
        [HideInInspector]
        public ItemBase BaseInstance
        {
            get => curInstance;
            set
            {
                if (value == curInstance)
                    return;

                ItemBase curInst = curInstance;
                curInstance = value;

                if (curInst != null)
                {
                    curInst.HolsteredDispatcher.InvokeAll();
                    curInst.IsEquipped = false;
                }

                if (curInstance != null)
                {
                    curInstance.EquippedDispatcher.InvokeAll();
                    curInstance.IsEquipped = true;
                }

                SelectItem(curInstance.Serial);
            }
        }

        /// <summary>
        /// Gets the time in seconds since last swap.
        /// </summary>
        public float LastItemSwap => (float)lastEquipSw.Elapsed.TotalSeconds;

        [field: SerializeField]
        /// <summary>
        /// Gets or sets the <see cref="InventoryInfo"/>.
        /// </summary>
        public InventoryInfo UserInventory { get; protected set; }

        /// <summary>
        /// Gets or sets the previous item.
        /// </summary>
        public ItemIdentifier PreviousItem { get; protected set; }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        public ItemIdentifier CurrentItem { get; set; }

        /// <inheritdoc/>
        public bool StaminaModifierActive { get; set; }

        /// <inheritdoc/>
        public bool MovementModifierActive { get; set; }

        /// <inheritdoc/>
        public float StaminaUsageMultiplier { get; set; }

        /// <inheritdoc/>
        public float StaminaRegenMultiplier { get; set; } = 1f;

        /// <inheritdoc/>
        public bool SprintingDisabled { get; set; }

        /// <inheritdoc/>
        public float MovementSpeedMultiplier { get; set; }

        /// <inheritdoc/>
        public float MovementSpeedLimit { get; set; }

        /// <inheritdoc/>
        public float MovementAccelerationMultiplier { get; set; }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            UserInventory = new(64);
        }

        /// <inheritdoc/>
        protected override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (PreviousItem != CurrentItem)
            {
                if (OnCurrentItemChanged is not null)
                    OnCurrentItemChanged(Owner.Cast<EntityBase>(), PreviousItem, CurrentItem);

                PreviousItem = new(CurrentItem.TypeId, CurrentItem.Serial);
            }

            foreach (ItemBase ib in UserInventory.Items)
                ib.AlwaysUpdate();
        }

        /// <summary>
        /// Fired when an item is updated.
        /// </summary>
        /// <param name="prev">The previous item.</param>
        /// <param name="cur">The current item.</param>
        public virtual void OnItemUpdated(ItemIdentifier prev, ItemIdentifier cur)
        {
            if (prev != cur)
                lastEquipSw.Restart();
        }

        /// <summary>
        /// Refreshes all modifiers.
        /// </summary>
        public virtual void RefreshModifiers()
        {
            StaminaUsageMultiplier = 1f;
            MovementSpeedLimit = BIG_NUMBER;
            MovementSpeedMultiplier = 1f;
            SprintingDisabled = false;

            foreach (ItemBase kvp in UserInventory.Items)
            {
                if (!kvp.Cast(out IRoleModifier roleModifier) || roleModifier == null)
                    continue;

                if (roleModifier is IStaminaModifier staminaModifier && staminaModifier.StaminaModifierActive)
                {
                    StaminaUsageMultiplier *= staminaModifier.StaminaUsageMultiplier;
                    SprintingDisabled |= staminaModifier.SprintingDisabled;
                }

                if (roleModifier is IMovementSpeedModifier movementSpeedModifier && movementSpeedModifier.MovementModifierActive)
                {
                    MovementSpeedLimit = Mathf.Min(MovementSpeedLimit, movementSpeedModifier.MovementSpeedLimit);
                    MovementSpeedMultiplier *= movementSpeedModifier.MovementSpeedMultiplier;
                }
            }
        }

        /// <summary>
        /// Selects an item.
        /// </summary>
        /// <param name="serial">The item serial.</param>
        public virtual void SelectItem(ushort serial)
        {
            if (serial == CurrentItem.Serial)
                return;

            ItemBase ib = UserInventory.Items.FirstOrDefault(item => item.Serial == CurrentItem.Serial);
            ItemBase itemBase = UserInventory.Items.FirstOrDefault(item => item.Serial == serial);
            bool flag = CurrentItem.Serial == 0 || ib && BaseInstance;

            if (serial == 0 || itemBase)
            {
                if (CurrentItem.Serial > 0 && flag && !ib.CanHolster || serial != 0 && !itemBase.CanEquip)
                    return;

                if (serial == 0)
                {
                    CurrentItem = ItemIdentifier.None;
                    BaseInstance = null;
                    return;
                }

                CurrentItem = new(itemBase.ItemTypeId, serial);
                BaseInstance = itemBase;
                return;
            }

            if (!flag)
            {
                CurrentItem = ItemIdentifier.None;
                BaseInstance = null;
            }
        }

        /// <summary>
        /// Destroys an item instance.
        /// </summary>
        /// <param name="serial">The item's serial.</param>
        /// <param name="pickup">The corresponding pickup.</param>
        /// <param name="foundItem">The found <see cref="ItemBase"/> instance.</param>
        /// <returns><see langword="true"/> if the item was destroyed successfully; otherwise, <see langword="false"/>.</returns>
        public virtual bool DestroyItem(ushort serial, ItemPickupBase pickup, out ItemBase foundItem)
        {
            foundItem = UserInventory.Items.FirstOrDefault(ib => ib.Serial == serial);
            if (!foundItem)
                return false;

            foundItem.RemovedDispatcher.InvokeAll(pickup);
            if (BaseInstance == foundItem)
                BaseInstance = null;

            Destroy(foundItem.GameObject);
            return true;
        }

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="ib">The item to be added.</param>
        /// <param name="pickup">The corresponding pickup.</param>
        /// <returns>An <see cref="ItemBase"/> instance, or <see langword="null"/> if not found.</returns>
        public virtual ItemBase AddItem(ItemBase ib, ItemPickupBase pickup = null)
        {
            ib.Serial = ib.Serial == 0 ? ItemSerialGenerator.GenerateNext() : ib.Serial;
            UserInventory.AddItem(ib);

            if (pickup)
                Timing.CallDelayed(Timing.WaitForOneFrame, () => ib.AddedDispatcher.InvokeAll(pickup));

            if (OnItemAdded is not null)
                OnItemAdded(Owner.Cast<EntityBase>(), ib, pickup);

            return ib;
        }

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="type">The item type.</param>
        /// <param name="serial">The item's serial.</param>
        /// <param name="pickup">The corresponding pickup.</param>
        /// <returns>An <see cref="ItemBase"/> instance, or <see langword="null"/> if not found.</returns>
        public virtual ItemBase AddItem(sbyte type, ushort serial = 0, ItemPickupBase pickup = null)
        {
            ItemBase ib = ItemBase.Create(type, Owner.Cast<EntityBase>());
            if (!ib)
                return null;

            ib.Serial = serial == 0 ? ItemSerialGenerator.GenerateNext() : serial;
            UserInventory.AddItem(ib);

            if (pickup)
                Timing.CallDelayed(Timing.WaitForOneFrame, () => ib.AddedDispatcher.InvokeAll(pickup));

            if (OnItemAdded is not null)
                OnItemAdded(Owner.Cast<EntityBase>(), ib, pickup);

            return ib;
        }

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="serial">The item's serial.</param>
        /// <param name="pickup">The corresponding pickup.</param>
        public virtual void RemoveItem(ushort serial, ItemPickupBase pickup)
        {
            if (!DestroyItem(serial, pickup, out ItemBase foundItem))
                return;

            if (serial == CurrentItem.Serial)
                CurrentItem = ItemIdentifier.None;

            if (OnItemRemoved is not null)
                OnItemRemoved(Owner.Cast<EntityBase>(), foundItem, pickup);

            UserInventory.RemoveItem(serial);
        }

        /// <summary>
        /// Drops an item from the inventory.
        /// </summary>
        /// <param name="serial">The item's serial.</param>
        /// <returns>The corresponding <see cref="ItemPickupBase"/>, or <see langword="null"/> if not found.</returns>
        public virtual ItemPickupBase DropItem(ItemBase item)
        {
            ItemBase ib = UserInventory.Items.FirstOrDefault(x => x.Serial == item.Serial);
            return ib.DropItem();
        }

        /// <summary>
        /// Drops everything from the inventory.
        /// </summary>
        public virtual void DropEverything()
        {
            while (UserInventory.Items.Count() > 0)
                DropItem(UserInventory.Items.FirstOrDefault());
        }
    }
}
