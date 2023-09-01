using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemStorageGrid
    {
        private Vector2Int dimensions;
        private ItemContainer container;
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
            this.stashes = new ContainerItemStash[this.container.Dimensions.x][];
            for (int x = 0; x < this.stashes.Length; x++)
            {
                this.stashes[x] = new ContainerItemStash[this.container.Dimensions.y];
                for (int y = 0; y < this.stashes[x].Length; y++)
                {
                    this.stashes[x][y] = new ContainerItemStash(this.container, storage);
                }
            }
        }
        
        public bool TryGetStash(Vector2Int indexPosition, out ContainerItemStash stash)
        {
            if (indexPosition.x >= this.dimensions.x || indexPosition.x < 0 || indexPosition.y >= this.dimensions.y || indexPosition.y < 0)
            {
                stash = null;
                return false;
            }
            stash = this.stashes[indexPosition.x][indexPosition.y];
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

                List<Item> items = lootRoll.Storable.CreateItems(lootRoll.Quantity);
                while(availableStashes.Count > 0 && items.Count > 0)
                {
                    int pickedStashIndex = Random.Range(0, availableStashes.Count);
                    ContainerItemStash pickedContainerStash = availableStashes[pickedStashIndex];
                    items.RemoveRange(0, pickedContainerStash.PushItems(lootRoll.Storable, items));
                    availableStashes.RemoveAt(pickedStashIndex);
                }
                ItemStorable.DiscardItems(items);
            }
        }
    }
}
