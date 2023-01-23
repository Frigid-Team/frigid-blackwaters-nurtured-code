using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceHand
    {
        private HoldingItemStash heldItemStash;
        private List<ItemStorage> currentStorages;
        private Dictionary<ItemStorage, int> gridIndexes;

        public ItemInterfaceHand(List<ItemStorage> currentStorages, Dictionary<ItemStorage, int> gridIndexes)
        {
            this.heldItemStash = null;
            this.currentStorages = currentStorages;
            this.gridIndexes = gridIndexes;
        }

        public bool TryGetHeldItemStash(out HoldingItemStash heldItemStash)
        {
            heldItemStash = this.heldItemStash;
            return this.heldItemStash != null;
        }
        
        public void CreateHeldItemStash()
        {
            this.heldItemStash = new HoldingItemStash(new List<Mob>(), new ItemPowerBudget(0), this.currentStorages[0].ItemCurrencyWallet, 0, 0, null);
        }

        public void EraseHeldItemStash()
        {
            this.heldItemStash = null;
        }

        public void RestoreHeldItems()
        {
            ItemStorageGrid firstStorageGrid = this.currentStorages[0].StorageGrids[this.gridIndexes[this.currentStorages[0]]];
            for (int x = 0; x < firstStorageGrid.Dimensions.x; x++)
            {
                for (int y = 0; y < firstStorageGrid.Dimensions.y; y++)
                {
                    if (this.heldItemStash.CurrentQuantity == 0) return;

                    if (firstStorageGrid.TryGetStash(new UnityEngine.Vector2Int(x, y), out ContainerItemStash currentItemStash))
                    {
                        currentItemStash.TransferItemsFromStash(this.heldItemStash, this.heldItemStash.CurrentQuantity, false);
                    }
                }
            }
        }

        public void HoldItems(ItemStash itemStash, int quantity, out int numItemsHeld)
        {
            int prevQuantity = this.heldItemStash.CurrentQuantity;
            this.heldItemStash.TransferItemsFromStash(itemStash, quantity, true);
            numItemsHeld = this.heldItemStash.CurrentQuantity - prevQuantity;
        }

        public void DepositItems(ItemStash itemStash, int quantity, out int numItemsDeposited)
        {
            int prevQuantity = this.heldItemStash.CurrentQuantity;
            itemStash.TransferItemsFromStash(this.heldItemStash, quantity, false);
            numItemsDeposited = prevQuantity - this.heldItemStash.CurrentQuantity;
        }

        public void QuickTransferItems(ItemStorageGrid itemStorageGrid, ItemStash itemStash, int quantity, out int numItemsTransferred)
        {
            numItemsTransferred = 0;
            int originalQuantity = itemStash.CurrentQuantity;
            foreach (ItemStorage currentStorage in this.currentStorages)
            {
                ItemStorageGrid currentStorageGrid = currentStorage.StorageGrids[this.gridIndexes[currentStorage]];
                if (currentStorageGrid != itemStorageGrid)
                {
                    for (int y = 0; y < currentStorageGrid.Dimensions.y; y++)
                    {
                        for (int x = 0; x < currentStorageGrid.Dimensions.x; x++)
                        {
                            if (currentStorageGrid.TryGetStash(new UnityEngine.Vector2Int(x, y), out ContainerItemStash currentItemStash))
                            {
                                currentItemStash.TransferItemsFromStash(itemStash, quantity, itemStorageGrid != this.currentStorages[0].StorageGrids[this.gridIndexes[this.currentStorages[0]]]);
                                numItemsTransferred = originalQuantity - itemStash.CurrentQuantity;
                                if (numItemsTransferred == quantity) return;
                            }
                        }
                    }
                }
            }
        }
    }
}
