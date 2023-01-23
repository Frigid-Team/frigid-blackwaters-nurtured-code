using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemStorageGrid
    {
        private Vector2Int dimensions;
        private ContainerItemStash[][] itemStashes;
        private ItemContainer itemContainer;

        public Vector2Int Dimensions
        {
            get
            {
                return this.dimensions;
            }
        }

        public ItemContainer ItemContainer
        {
            get
            {
                return this.itemContainer;
            }
        }

        public ItemStorageGrid(
            ItemContainer itemContainer, 
            List<Mob> usingMobs,
            ItemPowerBudget itemPowerBudget,
            ItemCurrencyWallet itemCurrencyWallet,
            float buyCostModifier,
            float sellCostModifier,
            Transform storageTransform
            )
        {
            this.dimensions = itemContainer.Dimensions;
            this.itemStashes = new ContainerItemStash[itemContainer.Dimensions.x][];
            for (int x = 0; x < this.itemStashes.Length; x++)
            {
                this.itemStashes[x] = new ContainerItemStash[itemContainer.Dimensions.y];
                for (int y = 0; y < this.itemStashes[x].Length; y++)
                {
                    this.itemStashes[x][y] = new ContainerItemStash(
                        itemContainer, 
                        usingMobs, 
                        itemPowerBudget,
                        itemCurrencyWallet,
                        buyCostModifier,
                        sellCostModifier,
                        storageTransform
                        );
                }
            }
            this.itemContainer = itemContainer;
        }
        
        public bool TryGetStash(Vector2Int indices, out ContainerItemStash itemStash)
        {
            if (indices.x >= dimensions.x || indices.x < 0 || indices.y >= dimensions.y || indices.y < 0)
            {
                itemStash = null;
                return false;
            }
            itemStash = this.itemStashes[indices.x][indices.y];
            return true;
        }

        public void FillWithLootTable(ItemLootTable itemLootTable)
        {
            List<ItemLootRoll> lootRolls = itemLootTable.GenerateLootRolls();
            foreach (ItemLootRoll lootRoll in lootRolls)
            {
                List<ContainerItemStash> availableItemStashes = new List<ContainerItemStash>();
                foreach (ContainerItemStash[] row in this.itemStashes)
                {
                    foreach (ContainerItemStash itemStash in row)
                    {
                        if (itemStash.CanStackItemStorable(lootRoll.ItemStorable) && itemStash.MaxQuantity - itemStash.CurrentQuantity >= lootRoll.Quantity)
                        {
                            availableItemStashes.Add(itemStash);
                        }
                    }
                }

                if (availableItemStashes.Count > 0)
                {
                    ContainerItemStash pickedItemStash = availableItemStashes[Random.Range(0, availableItemStashes.Count)];
                    pickedItemStash.AddItems(lootRoll.ItemStorable.CreateItems(lootRoll.Quantity), lootRoll.ItemStorable);
                }
            }
        }
    }
}
