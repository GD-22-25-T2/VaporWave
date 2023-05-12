namespace UDK.API.Features.Inventory.Pickups.PhysicsModules
{
    /// <summary>
    /// Defines the contract for basic physics implementation.
    /// </summary>
    public interface IPickupPhysicsModule
    {
        /// <summary>
        /// Gets or sets a value indicating whether the physics module is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Destroys the physics module.
        /// </summary>
        void FinalizePhysics();

        /// <summary>
        /// Updates the physics module.
        /// </summary>
        void Update();

        /// <summary>
        /// Handles received info.
        /// </summary>
        /// <param name="psi">The <see cref="PickupSyncInfo"/> to handle.</param>
        void ReceiveInfo(PickupSyncInfo psi);
    }
}
