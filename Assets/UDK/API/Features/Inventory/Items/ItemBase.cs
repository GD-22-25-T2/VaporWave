namespace UDK.API.Features.Inventory.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using UDK.API.Features.CharacterMovement;
    using UDK.API.Features.Core;
    using UDK.API.Features.Entity;
    using UDK.API.Features.Inventory.Items.Modifiers;
    using UDK.API.Features.Inventory.Pickups;
    using UDK.Events.DynamicEvents;
    using UnityEngine;
    using Quaternion = UnityEngine.Quaternion;
    using Vector3 = UnityEngine.Vector3;

    /// <summary>
    /// The base class which handles in-game items.
    /// </summary>
    [AddComponentMenu("UDK/Inventory/Items/ItemBase")]
    public abstract class ItemBase : Actor
    {
        #region BackingFields

        protected static readonly List<ItemBase> itemsValue = new();

        #endregion

        #region Editor

        [SerializeField]
        protected ItemPickupBase itemPickupBase;

        #endregion

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="ItemBase"/> containing all the registered items.
        /// </summary>
        public static IEnumerable<ItemBase> List => InventoryItemLoader.AvailableItems.Values.Concat(itemsValue);

        /// <summary>
        /// Gets the item's weight.
        /// </summary>
        public abstract float Weight { get; }

        /// <summary>
        /// Gets the description type.
        /// </summary>
        public virtual sbyte DescriptionType { get; }

        /// <summary>
        /// Gets the item's category.
        /// </summary>
        public virtual sbyte Category { get; }

        /// <summary>
        /// Gets the item's tier flags.
        /// </summary>
        public virtual sbyte TierFlags { get; }

        /// <summary>
        /// Gets or sets the item's type id.
        /// </summary>
        public virtual sbyte ItemTypeId { get; }

        /// <summary>
        /// Gets or sets the item's owner.
        /// </summary>
        public EntityBase Owner { get; set; }

        /// <summary>
        /// Gets or sets the item's serial number.
        /// </summary>
        public ushort Serial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is equipped.
        /// </summary>
        public bool IsEquipped { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ItemPickupBase"/>.
        /// </summary>
        public ItemPickupBase PickupDropModel
        {
            get => itemPickupBase;
            set => itemPickupBase = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="DynamicEventDispatcher"/> which handles all delegates fired when the item holstered.
        /// </summary>
        [DynamicEventDispatcher]
        public DynamicEventDispatcher HolsteredDispatcher { get; protected set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="DynamicEventDispatcher"/> which handles all delegates fired when the item is equipped.
        /// </summary>
        [DynamicEventDispatcher]
        public DynamicEventDispatcher EquippedDispatcher { get; protected set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired when the item is added.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ItemPickupBase> AddedDispatcher { get; protected set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired when the item is removed.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ItemPickupBase> RemovedDispatcher { get; protected set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired when the item template is reloaded.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<bool> TemplateReloadedDispatcher { get; protected set; } = new();

        /// <inheritdoc/>
        public virtual bool CanHolster => Cast(out IEquipDequipModifier modifier) && modifier is null || modifier.AllowHolster;

        /// <inheritdoc/>
        public virtual bool CanEquip => Cast(out IEquipDequipModifier modifier) && modifier is null || modifier.AllowEquip;

        /// <summary>
        /// Gets an <see cref="ItemBase"/> given the specified item type.
        /// </summary>
        /// <param name="typeId">The type id to look for.</param>
        /// <returns>The corresponding <see cref="ItemBase"/>, or <see langword="null"/> if not found.</returns>
        public static ItemBase Get(sbyte typeId) => List.FirstOrDefault(item => item.ItemTypeId == typeId);

        /// <summary>
        /// Gets an <see cref="ItemBase"/> given the specified serial number.
        /// </summary>
        /// <param name="serial">The serial to look for.</param>
        /// <returns>The corresponding <see cref="ItemBase"/>, or <see langword="null"/> if not found.</returns>
        public static ItemBase Get(ushort serial) => List.FirstOrDefault(item => item.Serial == serial);

        /// <summary>
        /// Tries to get an <see cref="ItemBase"/> given the specified item type.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="typeId">The type id to look for.</param>
        /// <param name="value">The found <see cref="ItemBase"/>.</param>
        /// <returns><see langword="true"/> if the item was found successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet<T>(sbyte typeId, out T value)
            where T : ItemBase
        {
            T t;
            if (!InventoryItemLoader.AvailableItems.TryGetValue(typeId, out ItemBase item) || !(t = (item.Cast<T>())))
            {
                value = default(T);
                return false;
            }

            value = t;
            return true;
        }

        /// <summary>
        /// Tries to get an <see cref="ItemBase"/> given the specified item type.
        /// </summary>
        /// <param name="typeId">The type id to look for.</param>
        /// <param name="item">The found <see cref="ItemBase"/>.</param>
        /// <returns><see langword="true"/> if the item was found successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(sbyte typeId, out ItemBase item) => item = Get(typeId);

        /// <summary>
        /// Tries to get an <see cref="EntityBase"/> given the specified serial number.
        /// </summary>
        /// <param name="serial">The serial to look for.</param>
        /// <param name="entity">The found <see cref="EntityBase"/>.</param>
        /// <returns><see langword="true"/> if the entity was found successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(ushort serial, out EntityBase entity) => entity = Get(serial)?.Owner;

        /// <summary>
        /// Tries to get an <see cref="ItemBase"/> given the specified serial number.
        /// </summary>
        /// <param name="serial">The serial to look for.</param>
        /// <param name="item">The found <see cref="ItemBase"/>.</param>
        /// <returns><see langword="true"/> if the item was found successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(ushort serial, out ItemBase item) => item = Get(serial);

        /// <summary>
        /// Creates an item instance given the specified item type, if exists.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <returns>An <see cref="ItemBase"/> instance, or <see langword="null"/> if not found.</returns>
        public static ItemBase Create(sbyte itemType)
        {
            ItemBase original = ItemBase.Get(itemType);
            if (!original)
                return null;

            ItemBase ib = UnityEngine.Object.Instantiate<ItemBase>(original);
            ib.Transform.localPosition = Vector3.zero;
            ib.Transform.localRotation = Quaternion.identity;
            return ib;
        }

        /// <summary>
        /// Creates an item instance given the specified item type, if exists.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <param name="owner">The item's owner.</param>
        /// <returns>An <see cref="ItemBase"/> instance, or <see langword="null"/> if not found.</returns>
        public static ItemBase Create(sbyte itemType, EntityBase owner)
        {
            ItemBase original = ItemBase.Get(itemType);
            if (!original)
                return null;

            ItemBase ib = UnityEngine.Object.Instantiate<ItemBase>(original, owner.Transform);
            ib.Transform.localPosition = Vector3.zero;
            ib.Transform.localRotation = Quaternion.identity;
            ib.Owner = owner;
            return ib;
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            itemsValue.Add(this);
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            itemsValue.Remove(this);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            HolsteredDispatcher.Bind(this, OnHolstered);
            EquippedDispatcher.Bind(this, OnEquipped);
            AddedDispatcher.Bind(this, OnAdded);
            RemovedDispatcher.Bind(this, OnRemoved);
            TemplateReloadedDispatcher.Bind(this, OnTemplateReloaded);
        }

        /// <summary>
        /// Fired every tick the item is equipped.
        /// </summary>
        public virtual void EquipUpdate()
        {
        }

        /// <summary>
        /// Fired every tick.
        /// </summary>
        public virtual void AlwaysUpdate()
        {
        }

        /// <summary>
        /// Fired when the item is holstered.
        /// </summary>
        protected virtual void OnHolstered()
        {
        }

        /// <summary>
        /// Fired when the item is equipped.
        /// </summary>
        protected virtual void OnEquipped()
        {
        }

        /// <summary>
        /// Fired when the item is added.
        /// </summary>
        /// <param name="pickup">The corresponding pickup.</param>
        protected virtual void OnAdded(ItemPickupBase pickup)
        {
        }

        /// <summary>
        /// Fired when the item is removed.
        /// </summary>
        /// <param name="pickup">The corresponding pickup.</param>
        protected virtual void OnRemoved(ItemPickupBase pickup)
        {
        }

        /// <summary>
        /// Fired when the item template is reloaded.
        /// </summary>
        /// <param name="wasEverLoaded">Whether the item was ever loaded.</param>
        protected virtual void OnTemplateReloaded(bool wasEverLoaded)
        {
        }

        /// <summary>
        /// Creates a pickup given the specified <see cref="PickupSyncInfo"/>.
        /// </summary>
        /// <param name="psi">The <see cref="PickupSyncInfo"/> from which create the pickup.</param>
        /// <returns>The corresponding <see cref="ItemPickupBase"/>.</returns>
        public virtual ItemPickupBase CreatePickup(PickupSyncInfo psi)
        {
            ItemPickupBase ipb = UnityEngine.Object.Instantiate<ItemPickupBase>(PickupDropModel, Owner.Position,
                Owner.GameObject.TryGetComponent(out GenCameraComponent camera) ? camera.Rotation * PickupDropModel.Rotation : default);

            ipb.SendInfo(default, psi);
            return ipb;
        }

        /// <summary>
        /// Drops the item.
        /// </summary>
        /// <returns>The corresponding <see cref="ItemPickupBase"/>.</returns>
        public virtual ItemPickupBase DropItem()
        {
            if (!PickupDropModel)
            {
                Log.Error($"Couldn't drop the item due to {nameof(PickupDropModel)} being null.");
                return null;
            }

            PickupSyncInfo psi = new(ItemTypeId, Owner.Position, Quaternion.identity, Weight, Serial);
            ItemPickupBase ipb = CreatePickup(psi);
            Owner.Inventory.RemoveItem(psi.Serial, ipb);
            ipb.PreviousOwner = Owner;
            return ipb;
        }
    }
}
