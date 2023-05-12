namespace UDK.API.Features.Core.Framework
{
    using UnityEngine;

    /// <inheritdoc/>
    [AddComponentMenu("UDK/Core/Framework/PlayerController")]
    [DisallowMultipleComponent]
    public abstract class PlayerController : Controller
    {
        /// <summary>
        /// Gets or sets a value indicating whether the controller thinks it's able to restart.
        /// </summary>
        public virtual bool CanRestartPlayer { get; set; }

        /// <summary>
        /// Gets or sets the respawn delay.
        /// </summary>
        public virtual float RespawnDelay { get; set; }
    }
}
