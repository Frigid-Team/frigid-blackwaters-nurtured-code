using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemStorageGrid
    {
        private Vector2Int dimensions;
        private ItemContainer container;
        private ItemStorage storage;
        private ContainerItemStash[][] stashes;

        public Vector2Int Dimensions
        {
            get
            {
                return this.dimensions;
            }
        }

        public ItemContainer Container
        {
            get
            {
                return this.container;
            }
        }

        public ItemStorageGrid(ItemContainer container, ItemStorage storage)
        {
            this.dimensions = container.Dimensions;
            this.container = container;
            this.storage = storage;
            this.stashes = new ContainerItemStash[this.container.Dimensions.x][];
            for (int x = 0; x < this.stashes.Length; x++)
            {
                this.stashes[x] = new ContainerItemStash[this.container.Dimensions.y];
                for (int y = 0; y < this.stashes[x].Length; y++)
                {
                    this.stashes[x][y] = new ContainerItemStash(this.container, this.storage);
                }
            }
        }
        
        public bool TryGetStash(Vector2Int indices, out ContainerItemStash stash)
        {
            if (indices.x >= dimensions.x || indices.x < 0 || indices.y >= dimensions.y || indices.y < 0)
            {
                stash = null;
                return false;
            }
            stash = this.stashes[indices.x][indices.y];
            return true;
        }

        public void FillWithLootTable(ItemLootTable itemLootTable)
        {
            List<ItemLootRoll> lootRolls = itemLootTable.GenerateLootRolls();
            foreach (ItemLootRoll lootRoll in lootRolls)
            {
                List<ContainerItemStash> availableStashes = new List<ContainerItemStash>();
                foreach (ContainerItemStash[] row in this.stashes)
                {
                    foreach (ContainerItemStash containerStash in row)
                    {
                        if (containerStash.CanStackStorable(lootRoll.Storable) && containerStash.MaxQuantity - containerStash.CurrentQuantity >= lootRoll.Quantity)
                        {
                            availableStashes.Add(containerStash);
                        }
                    }
                }

                if (availableStashes.Count > 0)
                {
                    ContainerItemStash pickedContainerStash = availableStashes[Random.Range(0, availableStashes.Count)];
                    List<Item> items = lootRoll.Storable.CreateItems(lootRoll.Quantity);
                    this.storage.AddStoredItems(items);
                    pickedContainerStash.AddItems(items, lootRoll.Storable);
                }
            }
        }
    }
}
