namespace UDK.API.Features.Inventory.Pickups.PhysicsModules
{
    using UDK.API.Features.Core;
    using UnityEngine;

    /// <summary>
    /// The default physics module used by items.
    /// </summary>
    public class SimplifiedPhysicsModule : UObject, IPickupPhysicsModule
    {
        /// <summary>
        /// Represents the default refresh rate.
        /// </summary>
        public const float REFRESH_RATE = 4f;

        /// <summary>
        /// Represents the default position smoothing.
        /// </summary>
        public const byte POSITION_SMOOTHING = 2;

        /// <summary>
        /// Represents the default angular smoothing.
        /// </summary>
        public const byte ANGULAR_SMOOTHING = 3;

        /// <summary>
        /// Represents the default lerp cutoff value.
        /// </summary>
        public const byte LERP_CUTOFF_VALUE = 10;

        #region BackingFields

        private PickupSyncInfo _receivedInfo;

        #endregion

        protected SimplifiedPhysicsModule(ItemPickupBase ipb)
            : base(ipb.GameObject)
        {
            Pickup = ipb;
            Rigidbody = ipb.Rigidbody;
        }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the <see cref="Pickup"/>'s <see cref="UnityEngine.Rigidbody"/>
        /// </summary>
        public Rigidbody Rigidbody { get; }

        /// <summary>
        /// Gets the pickup.
        /// </summary>
        public ItemPickupBase Pickup { get; }

        /// <inheritdoc/>
        public void Update()
        {
            if (!IsEnabled)
                return;

            if ((_receivedInfo.Position - Rigidbody.position).sqrMagnitude < 10f)
            {
                Rigidbody.position = Vector3.Lerp(Rigidbody.position, _receivedInfo.Position, 2f * Time.fixedDeltaTime);
                Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, _receivedInfo.Rotation, 2f * Time.fixedDeltaTime);
                return;
            }

            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.position = _receivedInfo.Position;
        }

        /// <inheritdoc/>
        public void ReceiveInfo(PickupSyncInfo info) => _receivedInfo = info;

        /// <inheritdoc/>
        public void FinalizePhysics()
        {
        }
    }
}
