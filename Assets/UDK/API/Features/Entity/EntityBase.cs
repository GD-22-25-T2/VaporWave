namespace UDK.API.Features.Entity
{
    using UDK.API.Features.Entity.Stats;
    using UDK.API.Features.Entity.Roles;
    using UDK.API.Features.Entity.Stats.SyncedStats;
    using UDK.API.Features.Entity.Stats.DamageHandlers;
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;
    using UDK.API.Features.Inventory;
    using UDK.API.Features.Inventory.Items;
    using UDK.API.Features.Inventory.Pickups;
    using System;

    using static UDK.API.Features.Entity.Stats.SyncedStats.ArtificialHealthStat;
    using UDK.API.Features.StatusEffects;
    using UDK.API.Features.CharacterMovement;
    using UDK.API.Features.Core.Framework;

    /// <summary>
    /// The base implementation of a game entity.
    /// <para>
    /// The current implementation may be used to define non-organic behaviors, such as AI entities.
    /// <br>All the existing modules can be extended in a way to handle many situations where organic and non-organic entities coexist.</br>
    /// <br>The built-in modules are already compatible with organic and non-organic entities, as they process and evaluate data based on the entity object.</br>
    /// </para>
    /// <para>
    /// A <see cref="Roles.RoleManager"/> exists, in order to easily manage in-game roles.
    /// <br>It provides an API to extend the relative subsystem, if exists, and/or to interact with the base one.</br>
    /// </para>
    /// <para>
    /// The <see cref="StatsBase"/> tracks and manages the current stats (<i>i.e. health, artificial health, [...]</i>).
    /// <br>The built-in damage system is based on the <see cref="StatsBase"/>, providing an API to easily manage different types of damage sources.</br>
    /// <br>Processes and performs evaultations on <see cref="DamageHandlerBase"/> objects, in order to affect the entity's in-game stats.</br>
    /// </para>
    /// <para>
    /// The current implementation refers to a network replicated code base, although it's not network replicated.
    /// </para>
    /// </summary>
    [AddComponentMenu("UDK/EntitySystem/EntityBase")]
    [RequireComponent(typeof(StatsBase), typeof(RoleManager))]
    [DisallowMultipleComponent]
    public abstract class EntityBase : Pawn
    {
        protected HealthStat healthStat;
        protected ArtificialHealthStat artificialHealthStat;

        /// <summary>
        /// Gets or sets the entity's <see cref="GenCameraComponent"/>.
        /// </summary>
        public GenCameraComponent Camera { get; protected set; }

        /// <summary>
        /// Gets the entity's <see cref="GenMovementComponent"/>.
        /// </summary>
        public GenMovementComponent Movement => GetPlayerController<GenMovementComponent>();

        /// <summary>
        /// Gets or sets the entity's <see cref="GenIKComponent"/>.
        /// </summary>
        public GenIKComponent IKComponent { get; protected set; }

        /// <summary>
        /// Gets or sets the entity's <see cref="GenFootstepComponent"/>.
        /// </summary>
        public GenFootstepComponent FootstepComponent { get; protected set; }

        /// <summary>
        /// Gets or sets the entity's <see cref="StatsBase"/>.
        /// </summary>
        public StatsBase Stats { get; protected set; }

        /// <summary>
        /// Gets or sets the entity's <see cref="Roles.RoleManager"/>.
        /// </summary>
        public RoleManager RoleManager { get; protected set; }

        /// <summary>
        /// Gets or sets the entity's <see cref="InventoryBase"/>.
        /// </summary>
        public InventoryBase Inventory { get; protected set; }

        /// <summary>
        /// Gets or sets the entity's <see cref="StatusEffects.EffectsController"/>
        /// </summary>
        public EffectsController EffectsController { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Inventory.InventoryInfo"/>
        /// </summary>
        public InventoryInfo InventoryInfo => Inventory.UserInventory;

        /// <summary>
        /// Gets the entity's items.
        /// </summary>
        public IEnumerable<ItemBase> Items => InventoryInfo.Items;

        /// <summary>
        /// Gets the entity's role.
        /// </summary>
        public RoleBase Role => RoleManager.CurrentRole;

        /// <summary>
        /// Gets the entity's stat modules.
        /// </summary>
        public SyncedStatBase[] StatModules => Stats.Modules;

        /// <summary>
        /// Gets the entity's <see cref="Entity.Stats.SyncedStats.HealthStat"/>
        /// </summary>
        public HealthStat HealthStat => healthStat ?? Stats.GetModule<HealthStat>();

        /// <summary>
        /// Gets the entity's <see cref="Entity.Stats.SyncedStats.ArtificialHealthStat"/>
        /// </summary>
        public ArtificialHealthStat ArtificialHealthStat => artificialHealthStat ?? Stats.GetModule<ArtificialHealthStat>();

        /// <summary>
        /// Gets all the active artificial health processes.
        /// </summary>
        public IEnumerable<ArtificialHealthProcess> ActiveArtificialHealthProcesses => ArtificialHealthStat?.ActiveProcesses;

        /// <summary>
        /// Gets the entity's <see cref="UnityEngine.Rigidbody"/>.
        /// </summary>
        public Rigidbody Rigidbody => Movement.Rigidbody;

        /// <summary>
        /// Gets the current entity's movement state.
        /// </summary>
        public sbyte MovementState => Movement.MovementState;

        /// <summary>
        /// Gets the previous entity's movement state.
        /// </summary>
        public sbyte PreviousMovementState => Movement.PreviousMovementState;

        /// <summary>
        /// Gets the current entity's <see cref="GenRootCollisionComponent"/>.
        /// </summary>
        public GenRootCollisionComponent RootCollision => Movement.RootCollision;

        /// <summary>
        /// Gets the time in seconds since last bounce.
        /// </summary>
        public float TimeSinceLastBounce => Movement.TimeSinceLastBounce;

        /// <summary>
        /// Gets the current entity's acceleration.
        /// </summary>
        public Vector3 Acceleration => Movement.Acceleration;

        /// <summary>
        /// Gets the current floor the entity's on.
        /// </summary>
        public RaycastHit CurrentFloor => Movement.FloorHit;

        /// <summary>
        /// Gets the current floor distance.
        /// </summary>
        public float FloorDistance => Movement.FloorDistance;

        /// <summary>
        /// Gets the current entity's speed.
        /// </summary>
        public float Speed => Movement.Speed;

        /// <summary>
        /// Gets or sets the entity's maximum movement speed.
        /// </summary>
        public float MaxSpeed
        {
            get => Movement.MaxSpeed;
            set => Movement.MaxSpeed = value;
        }

        /// <summary>
        /// Gets or sets a value indicating the gravity affects the entity.
        /// </summary>
        public bool UseGravity
        {
            get => Movement.UseGravity;
            set => Movement.UseGravity = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="LayerMask"/> to evaluate objects which make the entity bounce when they collide.
        /// </summary>
        public LayerMask BouncingCollisionMask
        {
            get => Movement.BouncingCollisionMask;
            set => Movement.BouncingCollisionMask = value;
        }

        /// <summary>
        /// Gets or sets the bouncing collision force.
        /// </summary>
        public float BouncingCollisionForce
        {
            get => Movement.BouncingCollisionForce;
            set => Movement.BouncingCollisionForce = value;
        }

        /// <summary>
        /// Gets or sets the entity's walkable <see cref="LayerMask"/>.
        /// </summary>
        public LayerMask WalkableMask
        {
            get => Movement.WalkableMask;
            set => Movement.WalkableMask = value;
        }

        /// <summary>
        /// Gets or sets the max floor distance tolerance before the entity will be considered in air.
        /// </summary>
        public float MaxFloorDistance
        {
            get => Movement.MaxFloorDistance;
            set => Movement.MaxFloorDistance = value;
        }

        /// <summary>
        /// Gets or sets the entity's maximum acceleration.
        /// </summary>
        public float MaxAcceleration
        {
            get => Movement.MaxAcceleration;
            set => Movement.MaxAcceleration = value;
        }

        /// <summary>
        /// Gets or sets the entity's health.
        /// </summary>
        public float Health
        {
            get => HealthStat.CurrentValue;
            set => HealthStat.CurrentValue = value;
        }

        /// <summary>
        /// Gets or sets the entity's maximum health.
        /// </summary>
        public float MaxHealth
        {
            get => HealthStat.MaxValue;
            set => HealthStat.CustomMaxValue = value;
        }

        /// <summary>
        /// Gets or sets the entity's artificial health.
        /// </summary>
        public float ArtificialHealth
        {
            get => ArtificialHealthStat.CurrentValue;
            set
            {
                if (value > MaxArtificialHealth)
                    MaxArtificialHealth = value;

                ArtificialHealthProcess artificialHealthProcess = ActiveArtificialHealthProcesses.FirstOrDefault();
                if (artificialHealthProcess is not null)
                    artificialHealthProcess.CurrentAmount = value;
            }
        }

        public float MaxArtificialHealth
        {
            get => ArtificialHealthStat.ActiveProcesses.FirstOrDefault()?.Limit ?? 0f;
            set
            {
                if (!ActiveArtificialHealthProcesses.Any())
                    AddArtificialHealthProcess(value);

                ArtificialHealthProcess artificialHealthProcess = ActiveArtificialHealthProcesses.FirstOrDefault();
                if (artificialHealthProcess is not null)
                    artificialHealthProcess.Limit = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity's id.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the entity's nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has god mode enabled.
        /// </summary>
        public bool IsGodModeEnabled
        {
            get => Stats.IsDamageIgnored;
            set => Stats.IsDamageIgnored = value;
        }

        /// <summary>
        /// Gets or sets the current <see cref="ItemBase"/> instance.
        /// </summary>
        public ItemBase CurrentItem
        {
            get => Inventory.BaseInstance;
            set => Inventory.BaseInstance = value;
        }

        /// <summary>
        /// Gets or sets the current item type.
        /// </summary>
        public sbyte CurrentItemType
        {
            get => CurrentItem.ItemTypeId;
            set => CurrentItem = ItemBase.Get(value);
        }

        /// <summary>
        /// Gets or sets the current <see cref="ItemIdentifier"/>.
        /// </summary>
        public ItemIdentifier CurrentItemId
        {
            get => Inventory.CurrentItem;
            set => Inventory.CurrentItem = value;
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            Stats = GetComponentSafe<StatsBase>();
            RoleManager = GetComponentSafe<RoleManager>();
            Inventory = GetComponentSafe<InventoryBase>();
            Camera = GetComponentSafe<GenCameraComponent>();
            IKComponent = GetComponentSafe<GenIKComponent>();
            FootstepComponent = GetComponentSafe<GenFootstepComponent>();
            EffectsController = GetComponentSafe<EffectsController>();
        }

#if UNITY_EDITOR
        /// <inheritdoc/>
        protected override void OnValidating()
        {
            base.OnValidating();

            Camera = Camera ?? GetComponentSafe<GenCameraComponent>();
            IKComponent = IKComponent ?? GetComponentSafe<GenIKComponent>();
            FootstepComponent = FootstepComponent ?? GetComponentSafe<GenFootstepComponent>();
            Stats = Stats ?? GetComponentSafe<StatsBase>();
            RoleManager = RoleManager ?? GetComponentSafe<RoleManager>();
            Inventory = Inventory ?? GetComponentSafe<InventoryBase>();
            EffectsController = EffectsController ?? GetComponentSafe<EffectsController>();
        }
#endif

        /// <summary>
        /// Hurts the entity.
        /// </summary>
        /// <param name="damageHandlerBase">The damage handler.</param>
        public void Hurt(DamageHandlerBase damageHandlerBase) => Stats.DealDamage(damageHandlerBase);

        /// <summary>
        /// Hurts the entity.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="damage">The damage amount.</param>
        public void Hurt(EntityBase attacker, float damage) => Stats.DealDamage(new UniversalDamageHandler(attacker, damage));

        /// <summary>
        /// Hurts the entity.
        /// </summary>
        /// <param name="damage">The damage amount.</param>
        public void Hurt(float damage) => Stats.DealDamage(new UniversalDamageHandler(this, damage));

        /// <summary>
        /// Adds an <see cref="ArtificialHealthProcess"/> to the active processes.
        /// </summary>
        /// <param name="amount">The amount of artificial health.</param>
        /// <param name="limit">The process cap.</param>
        /// <param name="decay">The decay amount.</param>
        /// <param name="efficacy">The efficacy of the process.</param>
        /// <param name="sustain">The sustain of the process.</param>
        /// <param name="persistant">Whether the process persists during the game.</param>
        /// <returns>The added <see cref="ArtificialHealthProcess"/>.</returns>
        public ArtificialHealthProcess AddProcess(float amount, float limit, float decay, float efficacy, float sustain, bool persistant) =>
            Stats.GetModule<ArtificialHealthStat>().AddProcess(amount, limit, decay, efficacy, sustain, persistant);

        /// <summary>
        /// Adds an <see cref="ArtificialHealthProcess"/> to the active processes.
        /// </summary>
        /// <param name="artificialHealthAmount">The amount of the artificial health.</param>
        /// <returns>The added <see cref="ArtificialHealthProcess"/>.</returns>
        public ArtificialHealthProcess AddArtificialHealthProcess(float artificialHealthAmount) =>
            Stats.GetModule<ArtificialHealthStat>().AddProcess(artificialHealthAmount);

        /// <summary>
        /// Sets the role.
        /// </summary>
        /// <param name="id">The role id.</param>
        public void SetRole(uint id) => RoleManager.InitializeNewRole(id);

        /// <summary>
        /// Adds an item to the entity's inventory.
        /// </summary>
        /// <param name="pickup">The pickup as item to be added.</param>
        public void AddItem(ItemPickupBase pickup) => Inventory.AddItem(pickup.Info.ItemId, pickup.Info.Serial, pickup);

        /// <summary>
        /// Adds an item to the entity's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddItem(ItemBase item) => Inventory.AddItem(item);

        /// <summary>
        /// Adds an item to the entity's inventory.
        /// </summary>
        /// <param name="typeId">The type of the item.</param>
        public void AddItem(sbyte typeId) => AddItem(ItemBase.Get(typeId));

        /// <summary>
        /// Adds multiple items to the entity's inventory.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        public void AddItem(IEnumerable<sbyte> items)
        {
            foreach (sbyte item in items)
                AddItem(item);
        }

        /// <summary>
        /// Adds multiple items to the entity's inventory.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        public void AddItem(IEnumerable<ItemBase> items)
        {
            foreach (ItemBase item in items)
                AddItem(item);
        }

        /// <summary>
        /// Adds multiple items to the entity's inventory.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        public void AddItem(params sbyte[] items) => AddItem(items);

        /// <summary>
        /// Adds multiple items to the entity's inventory.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        public void AddItem(params ItemBase[] items) => AddItem(items);

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The serial of the item to be removed.</param>
        public void RemoveItem(ushort serial)
        {
            if (!ItemBase.TryGet(serial, out ItemBase item) || item.Owner != this)
                return;

            Inventory.RemoveItem(item.Serial, item.PickupDropModel);
        }

        /// <summary>
        /// Removes the first item matching the specified item id.
        /// </summary>
        /// <param name="id">The item type.</param>
        public void RemoveItem(sbyte id)
        {
            Log.Debug("Remove Item Format");
            ItemBase jumpItem = GetItem(id);
            Inventory.RemoveItem(jumpItem.Serial, jumpItem.PickupDropModel);
            //RemoveItem(jumpItem);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void RemoveItem(ItemBase item)
        {
            if (item)
                Inventory.RemoveItem(item.Serial, item.PickupDropModel);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void RemoveItem(ItemIdentifier item) => RemoveItem(item.Serial);

        /// <summary>
        /// Removes the specified items.
        /// </summary>
        /// <param name="items">The items to be removed.</param>
        public void RemoveItem(IEnumerable<ItemBase> items)
        {
            foreach (ItemBase item in items)
                RemoveItem(item);
        }

        /// <summary>
        /// Removes the specified items.
        /// </summary>
        /// <param name="items">The items to be removed.</param>
        public void RemoveItem(IEnumerable<ItemIdentifier> items)
        {
            foreach (ItemIdentifier item in items)
                RemoveItem(item);
        }

        /// <summary>
        /// Removes the specified items.
        /// </summary>
        /// <param name="items">The items to be removed.</param>
        public void RemoveItem(params ItemBase[] items) => RemoveItem(items);

        /// <summary>
        /// Removes the specified items.
        /// </summary>
        /// <param name="items">The items to be removed.</param>
        public void RemoveItem(params ItemIdentifier[] items) => RemoveItem(items);

        /// <summary>
        /// Drops the current item.
        /// </summary>
        public void DropCurrentItem() => Inventory.DropItem(CurrentItem);

        /// <summary>
        /// Drops the specified item.
        /// </summary>
        /// <param name="serial">The serial of the item to be dropped.</param>
        public void DropItem(ushort serial) => Inventory.DropItem(ItemBase.Get(serial));

        /// <summary>
        /// Drops the specified item.
        /// </summary>
        /// <param name="item">The item to be dropped.</param>
        public void DropItem(ItemBase item) => DropItem(item.Serial);

        /// <summary>
        /// Drops the specified item.
        /// </summary>
        /// <param name="item">The item to be dropped.</param>
        public void DropItem(ItemIdentifier item) => DropItem(item.Serial);

        /// <summary>
        /// Drops the specified items.
        /// </summary>
        /// <param name="items">The items to be dropped.</param>
        public void DropItem(IEnumerable<ItemBase> items)
        {
            foreach (ItemBase item in items)
                DropItem(item);
        }

        /// <summary>
        /// Drops the specified items.
        /// </summary>
        /// <param name="items">The items to be dropped.</param>
        public void DropItem(IEnumerable<ItemIdentifier> items)
        {
            foreach (ItemIdentifier item in items)
                DropItem(item);
        }

        /// <summary>
        /// Drops the specified items.
        /// </summary>
        /// <param name="items">The items to be dropped.</param>
        public void DropItem(params ItemBase[] items) => DropItem(items);

        /// <summary>
        /// Drops the specified items.
        /// </summary>
        /// <param name="items">The items to be dropped.</param>
        public void DropItem(params ItemIdentifier[] items) => DropItem(items);

        /// <summary>
        /// Drops everything from the inventory.
        /// </summary>
        public void DropEverything() => Inventory.DropEverything();

        /// <summary>
        /// Resets the entity's inventory.
        /// </summary>
        /// <param name="items">The new items to be given.</param>
        public void ResetInventory(IEnumerable<sbyte> items = null)
        {
            RemoveItem(Items);

            if (items is null || !items.Any())
                return;

            AddItem(items);
        }

        /// <summary>
        /// Resets the entity's inventory.
        /// </summary>
        /// <param name="items">The new items to be given.</param>
        public void ResetInventory(IEnumerable<ItemBase> items = null)
        {
            RemoveItem(Items);

            if (items is null || !items.Any())
                return;

            AddItem(items);
        }

        /// <summary>
        /// Resets the entity's inventory.
        /// </summary>
        /// <param name="items">The new items to be given.</param>
        public void ResetInventory(params sbyte[] items) => ResetInventory(items);

        /// <summary>
        /// Resets the entity's inventory.
        /// </summary>
        /// <param name="items">The new items to be given.</param>
        public void ResetInventory(params ItemBase[] items) => ResetInventory(items);

        /// <summary>
        /// Gets a value indicating whether the entity has the specified item.
        /// </summary>
        /// <param name="serial">The item's serial to look for.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise, <see langword="false"/>.</returns>
        public bool HasItem(ushort serial) => Items.Any(item => item.Serial == serial);

        /// <summary>
        /// Gets a value indicating whether the entity has the specified item.
        /// </summary>
        /// <param name="serial">The item to look for.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise, <see langword="false"/>.</returns>
        public bool HasItem(ItemBase item) => HasItem(item.Serial);

        /// <summary>
        /// Gets a value indicating whether the entity has the specified item.
        /// </summary>
        /// <param name="serial">The item type to look for.</param>
        /// <returns><see langword="true"/> if the item was found; otherwise, <see langword="false"/>.</returns>
        public bool HasItem(sbyte id) => Items.Any(i => i.ItemTypeId == id);

        /// <summary>
        /// Gets the specified item from the entity's inventory.
        /// </summary>
        /// <param name="serial">The item type to look for.</param>
        /// <returns>The corresponding <see cref="ItemBase"/>, or <see langword="null"/> if not found.</returns>
        public ItemBase GetItem(sbyte id) => Items.FirstOrDefault(item => item.ItemTypeId == id);

        /// <summary>
        /// Gets the specified item from the entity's inventory.
        /// </summary>
        /// <param name="serial">The item's serial to look for.</param>
        /// <returns>The corresponding <see cref="ItemBase"/>, or <see langword="null"/> if not found.</returns>
        public ItemBase GetItem(ushort serial) => Items.FirstOrDefault(item => item.Serial == serial);

        /// <summary>
        /// Gets the specified item from the entity's inventory.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>The corresponding <see cref="ItemBase"/>, or <see langword="null"/> if not found.</returns>
        public void UseItem(ItemBase item) => Inventory.SelectItem(item.Serial);

        /// <summary>
        /// Gets the current item type.
        /// </summary>
        /// <typeparam name="T">The item type enum.</typeparam>
        /// <returns>The current item type as the enum value.</returns>
        public T GetItemType<T>()
            where T : Enum => (T)Enum.ToObject(typeof(T), CurrentItem.ItemTypeId);

        /// <summary>
        /// Sets the current item type.
        /// </summary>
        /// <typeparam name="T">The item type enum.</typeparam>
        /// <param name="param">The item type to be set.</param>
        public void SetItemType<T>(T param)
            where T : Enum
        {
            if (param is not sbyte type)
                return;

            CurrentItemType = type;
        }

        /// <summary>
        /// Gets the current movement state.
        /// </summary>
        /// <typeparam name="T">The movement state enum.</typeparam>
        /// <returns>The current movement state as the enum value.</returns>
        public T GetMovementState<T>()
            where T : Enum => (T)Enum.ToObject(typeof(T), MovementState);

        /// <summary>
        /// Gets the <see cref="RootCollision"/> as the specified collision shape.
        /// </summary>
        /// <typeparam name="T">The type of the collision shape.</typeparam>
        /// <returns>The cast <see cref="RootCollision"/> or a <see cref="InvalidCastException"/> if the specified cast is not valid.</returns>
        /// <exception cref="InvalidCastException">Thrown when the specified cast is not valid.</exception>
        public T GetRootCollision<T>()
            where T : Collider => Movement.GetRootCollision<T>();


        /// <summary>
        /// Gets all currently active <see cref="StatusEffectBase">effects</see>.
        /// </summary>
        /// <seealso cref="EnableEffect(sbyte, float, bool)"/>
        /// <seealso cref="EnableEffect(StatusEffectBase, float, bool)"/>
        /// <seealso cref="EnableEffect(string, float, bool)"/>
        /// <seealso cref="EnableEffect{T}(float, bool)"/>
        /// <seealso cref="EnableEffects(IEnumerable{sbyte}, float, bool)"/>
        public IEnumerable<StatusEffectBase> ActiveEffects => EffectsController.AllEffects.Where(effect => effect.Intensity > 0);

        /// <summary>
        /// Gets a <see cref="bool"/> describing whether or not the given <see cref="StatusEffectBase">status effect</see> is currently enabled.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to check.</typeparam>
        /// <returns>A <see cref="bool"/> determining whether or not the entity effect is active.</returns>
        public bool IsEffectActive<T>()
            where T : StatusEffectBase
        {
            T effect = EffectsController.GetEffect<T>();
            return effect && effect.IsEnabled;
        }

        /// <summary>
        /// Disables all currently active <see cref="StatusEffectBase">status effects</see>.
        /// </summary>
        /// <seealso cref="DisableEffects(IEnumerable{sbyte})"/>
        public void DisableAllEffects()
        {
            foreach (StatusEffectBase effect in EffectsController.AllEffects)
                effect.IsEnabled = false;
        }

        /// <summary>
        /// Disables a specific <see cref="StatusEffectBase">status effect</see> on the entity.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to disable.</typeparam>
        public void DisableEffect<T>()
            where T : StatusEffectBase => EffectsController.DisableEffect<T>();

        /// <summary>
        /// Disables a specific effect type on the entity.
        /// </summary>
        /// <param name="effect">The effect type to disable.</param>
        public void DisableEffect(sbyte effect)
        {
            if (TryGetEffect(effect, out StatusEffectBase playerEffect))
                playerEffect.IsEnabled = false;
        }

        /// <summary>
        /// Disables a <see cref="IEnumerable{T}"/> of effect types on the entity.
        /// </summary>
        /// <param name="effects">The <see cref="IEnumerable{T}"/> of effect types to disable.</param>
        public void DisableEffects(IEnumerable<sbyte> effects)
        {
            foreach (sbyte effect in effects)
                DisableEffect(effect);
        }

        /// <summary>
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the entity.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to enable.</typeparam>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A bool indicating whether or not the effect was valid and successfully enabled.</returns>
        public bool EnableEffect<T>(float duration = 0f, bool addDurationIfActive = false)
            where T : StatusEffectBase => EffectsController.EnableEffect<T>(duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the entity.
        /// </summary>
        /// <param name="statusEffect">The name of the <see cref="StatusEffectBase"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A bool indicating whether or not the effect was valid and successfully enabled.</returns>
        public bool EnableEffect(StatusEffectBase statusEffect, float duration = 0f, bool addDurationIfActive = false) =>
            EnableEffect(statusEffect.GetType().Name, duration, addDurationIfActive).IsEnabled;

        /// <summary>
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the entity.
        /// </summary>
        /// <param name="effectName">The name of the <see cref="StatusEffectBase"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>The <see cref="StatusEffectBase"/> instance of the activated effect.</returns>
        public StatusEffectBase EnableEffect(string effectName, float duration = 0f, bool addDurationIfActive = false) =>
            EffectsController.ChangeState(effectName, 1, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="sbyte">status effect</see> on the entity.
        /// </summary>
        /// <param name="type">The effect type to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        public void EnableEffect(sbyte type, float duration = 0f, bool addDurationIfActive = false)
        {
            if (TryGetEffect(type, out StatusEffectBase statusEffect))
                EnableEffect(statusEffect, duration, addDurationIfActive);
        }

        /// <summary>
        /// Enables a <see cref="IEnumerable{T}"/> of effect type on the entity.
        /// </summary>
        /// <param name="types">The <see cref="IEnumerable{T}"/> of effect types to enable.</param>
        /// <param name="duration">The amount of time the effects will be active for.</param>
        /// <param name="addDurationIfActive">If an effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        public void EnableEffects(IEnumerable<sbyte> types, float duration = 0f, bool addDurationIfActive = false)
        {
            foreach (sbyte type in types)
            {
                if (TryGetEffect(type, out StatusEffectBase statusEffect))
                    EnableEffect(statusEffect, duration, addDurationIfActive);
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="StatusEffectBase"/> by effect type.
        /// </summary>
        /// <param name="type">The effect type.</param>
        /// <returns>The <see cref="StatusEffectBase"/>.</returns>
        public StatusEffectBase GetEffect(sbyte type)
        {
            EffectsController.EffectsByType.TryGetValue(type.GetType(), out StatusEffectBase playerEffect);
            return playerEffect;
        }

        /// <summary>
        /// Tries to get an instance of <see cref="StatusEffectBase"/> by effect type.
        /// </summary>
        /// <param name="type">The effect type.</param>
        /// <param name="statusEffect">The <see cref="StatusEffectBase"/>.</param>
        /// <returns>A bool indicating whether or not the <paramref name="statusEffect"/> was successfully gotten.</returns>
        public bool TryGetEffect(sbyte type, out StatusEffectBase statusEffect) => (statusEffect = GetEffect(type)) is not null;

        /// <summary>
        /// Tries to get an instance of <see cref="StatusEffectBase"/> by effect type.
        /// </summary>
        /// <param name="statusEffect">The <see cref="StatusEffectBase"/>.</param>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to get.</typeparam>
        /// <returns>A bool indicating whether or not the <paramref name="statusEffect"/> was successfully gotten.</returns>
        public bool TryGetEffect<T>(out T statusEffect)
            where T : StatusEffectBase => EffectsController.TryGetEffect(out statusEffect);

        /// <summary>
        /// Gets a <see cref="byte"/> indicating the intensity of the given <see cref="StatusEffectBase"></see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to check.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the given type is not a valid <see cref="StatusEffectBase"/>.</exception>
        /// <returns>The intensity of the effect.</returns>
        public byte GetEffectIntensity<T>()
            where T : StatusEffectBase
        {
            if (EffectsController.EffectsByType.TryGetValue(typeof(T), out StatusEffectBase statusEffect))
                return statusEffect.Intensity;

            throw new ArgumentException("The given type is invalid.");
        }

        /// <summary>
        /// Changes the intensity of a <see cref="StatusEffectBase">status effect</see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to change the intensity of.</typeparam>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="duration">The new duration to add to the effect.</param>
        public void ChangeEffectIntensity<T>(byte intensity, float duration = 0)
            where T : StatusEffectBase
        {
            if (EffectsController.TryGetEffect(out T statusEffect))
            {
                statusEffect.Intensity = intensity;
                statusEffect.ChangeDuration(duration, true);
            }
        }

        /// <summary>
        /// Changes the intensity of a <see cref="StatusEffectBase"/>.
        /// </summary>
        /// <param name="type">The effect type to change.</param>
        /// <param name="intensity">The new intensity to use.</param>
        /// <param name="duration">The new duration to add to the effect.</param>
        public void ChangeEffectIntensity(sbyte type, byte intensity, float duration = 0)
        {
            if (TryGetEffect(type, out StatusEffectBase statusEffect))
            {
                statusEffect.Intensity = intensity;
                statusEffect.ChangeDuration(duration, true);
            }
        }

        /// <summary>
        /// Changes the intensity of a <see cref="StatusEffectBase">status effect</see>.
        /// </summary>
        /// <param name="effectName">The name of the <see cref="StatusEffectBase"/> to enable.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="duration">The new length of the effect. Defaults to infinite length.</param>
        public void ChangeEffectIntensity(string effectName, byte intensity, float duration = 0)
        {
            if (Enum.TryParse(effectName, out sbyte type))
                ChangeEffectIntensity(type, intensity, duration);
        }

        /// <summary>
        /// Converts the player in a human-readable format.
        /// </summary>
        /// <returns>A string containing entity-related data.</returns>
        public override string ToString() => $"{Id} ({Nickname}) *{(Role is null ? "No role" : Role)}*";
    }
}
