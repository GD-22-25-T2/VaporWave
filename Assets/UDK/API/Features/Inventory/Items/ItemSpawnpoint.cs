namespace UDK.API.Features.Inventory.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using UDK.API.Features.Core;
    using UnityEngine;

    /// <summary>
    /// The base component containing all the information to spawn items.
    /// </summary>
    public class ItemSpawnpoint : Actor
    {
        #region Editor

        [SerializeField]
        private int _maxUses;

        [SerializeField]
        private Transform[] _positionVariants;

        [SerializeField]
        private SpawnableItem _spawnableItem;

        #endregion

        #region BackingFields

        private static readonly HashSet<ItemSpawnpoint> _allInstances = new();

        private int _uses;

        #endregion

        /// <summary>
        /// Gets all <see cref="ItemSpawnpoint"/> instances.
        /// </summary>
        public static IEnumerable<ItemSpawnpoint> AllInstances => _allInstances;

        /// <summary>
        /// Gets the max uses.
        /// </summary>
        public int MaxUses => _maxUses;

        /// <summary>
        /// Gets all <see cref="Transform"/>s used by the item spawnpoint.
        /// </summary>
        public Transform[] PositionVariants => _positionVariants;

        /// <summary>
        /// Gets the relative <see cref="Items.SpawnableItem"/>.
        /// </summary>
        public SpawnableItem SpawnableItem => _spawnableItem;

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            _allInstances.Add(this);
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            _allInstances.Remove(this);
        }

        /// <summary>
        /// Gets a value indicating whether the items can be spawned.
        /// </summary>
        /// <param name="items">The items to check.</param>
        /// <returns>A value indicating whether the items can be spawned.</returns>
        public bool CanSpawn(sbyte[] items) => !items.Any(item => !CanSpawn(item));

        /// <summary>
        /// Gets a value indicating whether the item can be spawned.
        /// </summary>
        /// <param name="targetItem">The item to check.</param>
        /// <returns>A value indicating whether the item spawn be spawned.</returns>
        public bool CanSpawn(sbyte targetItem) => _uses < _maxUses && _spawnableItem.PossibleSpawns.Any(item => item == targetItem);

        /// <summary>
        /// Consumes a spawn.
        /// </summary>
        /// <returns>The <see cref="Transform"/> variant.</returns>
        public Transform Occupy()
        {
            _uses++;
            return _positionVariants[Random.Range(0, _positionVariants.Length)];
        }
    }
}
