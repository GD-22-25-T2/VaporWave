namespace UDK.API.Features.Entity.Roles.Interfaces
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the base contract for respawnable roles.
    /// </summary>
    public interface IRespawnableRole
    {
        /// <summary>
        /// Gets or sets the spawnpoints.
        /// </summary>
        public List<Vector3> Spawnpoints { get; set; }

        /// <summary>
        /// Respawns the owner.
        /// </summary>
        /// <returns><see langword="true"/> if the owner was respawned; otherwise, <see langword="false"/>.</returns>
        public bool Respawn();

        /// <summary>
        /// Respawns the owner in the specified spawnpoint.
        /// </summary>
        /// <param name="spawnpoint">The spawnpoint to spawn the owner on.</param>
        /// <returns><see langword="true"/> if the owner was respawned; otherwise, <see langword="false"/>.</returns>
        public bool Respawn(Vector3 spawnpoint);
    }
}
