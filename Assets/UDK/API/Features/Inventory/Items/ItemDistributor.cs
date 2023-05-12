namespace UDK.API.Features.Inventory.Items
{
    using UDK.API.Features.Core;
    using UDK.API.Features.Inventory.Pickups;
    using UnityEngine;

    /// <summary>
    /// The base component which handles item distribution.
    /// </summary>
    public class ItemDistributor : Actor
    {
        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            PlaceSpawnables();
        }

        /// <summary>
        /// Places all spawnable items.
        /// </summary>
        public virtual void PlaceSpawnables()
        {
            foreach (ItemSpawnpoint spawnpoint in ItemSpawnpoint.AllInstances)
            {
                if (!spawnpoint)
                    continue;

                sbyte[] items = spawnpoint.SpawnableItem.PossibleSpawns;
                CreatePickup(items[UnityEngine.Random.Range(0, items.Length)], spawnpoint.Occupy());
            }
        }

        /// <summary>
        /// Creates and spawns a pickup.
        /// </summary>
        /// <param name="id">The item id.</param>
        /// <param name="t">The parent <see cref="Transform"/>.</param>
        public virtual void CreatePickup(sbyte id, Transform t)
        {
            if (!ItemBase.TryGet(id, out ItemBase itemBase))
                return;

            ItemPickupBase pickup = itemBase.CreatePickup(new(id, t.position, t.rotation, itemBase.Weight));
            pickup.Spawn(t);
        }
    }
}
