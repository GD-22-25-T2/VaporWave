namespace UDK.API.Features.Inventory.Items
{
    using System;

    /// <summary>
    /// A struct which handles spawnable items information.
    /// </summary>
    [Serializable]
    public struct SpawnableItem
    {
        /// <summary>
        /// Gts or sets the minimal amount of items to be spawned.
        /// </summary>
        public float MinAmount { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of spawnable items.
        /// </summary>
        public float MaxAmount { get; set; }

        /// <summary>
        /// Gets or sets the possible spawnable item types.
        /// </summary>
        public sbyte[] PossibleSpawns { get; set; }
    }
}
