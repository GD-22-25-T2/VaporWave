namespace UDK.API.Features.Inventory.Pickups
{
    using System;
    using System.Collections.Generic;
    using UDK.API.Features.Core;
    using UDK.API.Features.Core.Framework;
    using UDK.API.Features.Inventory.Items;
    using UDK.API.Features.Inventory.Pickups.PhysicsModules;
    using UnityEngine;

    using static Constants;

    /// <summary>
    /// The base class which handles in-game pickups.
    /// </summary>
    [AddComponentMenu("UDK/Inventory/Pickups/ItemPickupBase")]
    [DisallowMultipleComponent]
    public abstract class ItemPickupBase : Actor
    {
        /// <summary>
        /// Represents the height at which the pickups will be deleted.
        /// </summary>
        public const float AUTO_DELETE_HEIGHT = -2500f;

        /// <summary>
        /// Represents the minimum weight of pickups.
        /// </summary>
        public const float MIN_WEIGHT = 0.001f;

        /// <summary>
        /// Represents the minimum pickup time of pickups.
        /// </summary>
        public const float MIN_PICKUP_TIME = 0.245f;

        /// <summary>
        /// Represents the weight to time of pickups.
        /// </summary>
        public const float WEIGHT_TO_TIME = 0.175f;

        /// <summary>
        /// Invoked when a pickup is added.
        /// </summary>
        public static event Action<ItemPickupBase> OnPickupAdded;

        /// <summary>
        /// Invoked when a pickup is destroyed.
        /// </summary>
        public static event Action<ItemPickupBase> OnPickupDestroyed;

        #region Editor

        [SerializeField]
        protected IPickupPhysicsModule physicsModule;

        [SerializeField]
        protected PickupSyncInfo info = PickupSyncInfo.None;

        #endregion

        #region BackingFields

        protected static readonly List<ItemPickupBase> pickupValues = new();

        #endregion

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="ItemPickupBase"/> containing all pickups.
        /// </summary>
        public static IEnumerable<ItemPickupBase> List => pickupValues;

        /// <summary>
        /// Get or sets the pickup's <see cref="UnityEngine.Rigidbody"/>.
        /// </summary>
        public Rigidbody Rigidbody { get; protected set; }

        /// <summary>
        /// Gets or sets the pickup's previous owner.
        /// </summary>
        public Pawn PreviousOwner { get; set; }

        /// <summary>
        /// Gets or sets the pickup's info.
        /// </summary>
        public PickupSyncInfo Info
        {
            get => info;
            set => info = value;
        }

        /// <summary>
        /// Gets or sets the physics module.
        /// </summary>
        protected IPickupPhysicsModule PhysicsModule
        {
            get => physicsModule;
            set => physicsModule = value;
        }

        /// <summary>
        /// Spawns the specified pickup.
        /// </summary>
        /// <param name="ipb">The pickup to be spawned.</param>
        /// <param name="parent">The parent <see cref="Transform"/>.</param>
        public static GameObject Spawn(ItemPickupBase ipb, Transform parent = null)
        {
            if (!ipb)
                return null;

            if (parent)
            {
                ipb.Transform.SetParent(parent);
                UnityEngine.Object.Instantiate(ipb.GameObject, parent);
                return Instantiate(ipb.GameObject, parent);
            }

            UnityEngine.Object.Instantiate(ipb.GameObject);
            return Instantiate(ipb.GameObject);
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            base.OnPostInitialize();

            try
            {
                Rigidbody = GetComponentSafe<Rigidbody>();
                Rigidbody.ResetCenterOfMass();
            }
            catch
            {
            }
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            if (OnPickupAdded is not null)
                OnPickupAdded(this);

            if (Info.Serial == 0)
            {
                PickupSyncInfo psi = Info;
                psi.Serial = ItemSerialGenerator.GenerateNext();
                Info = psi;
            }

            pickupValues.Add(this);
        }

        /// <inheritdoc/>
        protected override void FixedTick(float fixedDeltaTime)
        {
            base.FixedTick(fixedDeltaTime);

            if (PhysicsModule is not null)
                PhysicsModule.Update();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            PhysicsModule?.FinalizePhysics();

            if (OnPickupDestroyed is not null)
                OnPickupDestroyed(this);

            pickupValues.Remove(this);
        }

        /// <summary>
        /// Sends information to the <see cref="PhysicsModule"/>/
        /// </summary>
        /// <param name="oldInfo">The old <see cref="PickupSyncInfo"/>.</param>
        /// <param name="newInfo">The new <see cref="PickupSyncInfo"/>.</param>
        public virtual void SendInfo(PickupSyncInfo oldInfo, PickupSyncInfo newInfo)
        {
            if (PhysicsModule is null)
                return;

            PhysicsModule.ReceiveInfo(newInfo);
            Rigidbody.mass = Mathf.Max(KINDA_SMALL_NUMBER, newInfo.Weight);
        }

        /// <summary>
        /// Refreshes the pickup's position and rotation.
        /// </summary>
        public virtual void RefreshPositionAndRotation()
        {
            if (Position.y < AUTO_DELETE_HEIGHT)
            {
                Destroy();
                return;
            }

            Info.SetPositionAndRotation(Position, Rotation);
        }

        /// <summary>
        /// Spawns the pickup.
        /// </summary>
        /// <param name="parent">The parent <see cref="Transform"/>.</param>
        public virtual void Spawn(Transform parent = null) => Spawn(this, parent ? parent : Transform);
    }
}
