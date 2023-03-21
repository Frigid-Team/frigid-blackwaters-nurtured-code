using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceHand
    {
        private HoldingItemStash holdingStash;
        private List<ItemStorage> currentStorages;
        private Dictionary<ItemStorage, int> gridIndexes;

        public ItemInterfaceHand(List<ItemStorage> currentStorages, Dictionary<ItemStorage, int> gridIndexes)
        {
            this.holdingStash = null;
            this.currentStorages = currentStorages;
            this.gridIndexes = gridIndexes;
        }

        public bool TryGetHoldingStash(out HoldingItemStash heldItemStash)
        {
            heldItemStash = this.holdingStash;
            return this.holdingStash != null;
        }
        
        public void CreateHoldingStash()
        {
            this.holdingStash = new HoldingItemStash(this.currentStorages[0]);
        }

        public void EraseHoldingStash()
        {
            this.holdingStash = null;
        }

        public void RestoreHeldItems()
        {
            ItemStorageGrid firstStorageGrid = this.currentStorages[0].StorageGrids[this.gridIndexes[this.currentStorages[0]]];
            for (int x = 0; x < firstStorageGrid.Dimensions.x; x++)
            {
                for (int y = 0; y < firstStorageGrid.Dimensions.y; y++)
                {
                    if (this.holdingStash.CurrentQuantity == 0) return;

                    if (firstStorageGrid.TryGetStash(new UnityEngine.Vector2Int(x, y), out ContainerItemStash currentItemStash))
                    {
                        currentItemStash.TransferItemsFromStash(this.holdingStash, this.holdingStash.CurrentQuantity, false);
                    }
                }
            }
        }

        public void HoldItems(ItemStash stash, int quantity, out int numItemsHeld)
        {
            int prevQuantity = this.holdingStash.CurrentQuantity;
            this.holdingStash.TransferItemsFromStash(stash, quantity, true);
            numItemsHeld = this.holdingStash.CurrentQuantity - prevQuantity;
        }

        public void DepositItems(ItemStash stash, int quantity, out int numItemsDeposited)
        {
            int prevQuantity = this.holdingStash.CurrentQuantity;
            stash.TransferItemsFromStash(this.holdingStash, quantity, false);
            numItemsDeposited = prevQuantity - this.holdingStash.CurrentQuantity;
        }

        public void QuickTransferItems(ItemStorageGrid storageGrid, ItemStash stash, int quantity, out int numItemsTransferred)
        {
            numItemsTransferred = 0;
            int originalQuantity = stash.CurrentQuantity;
            foreach (ItemStorage currentStorage in this.currentStorages)
            {
                ItemStorageGrid currentStorageGrid = currentStorage.StorageGrids[this.gridIndexes[currentStorage]];
                if (currentStorageGrid != storageGrid)
                {
                    for (int y = 0; y < currentStorageGrid.Dimensions.y; y++)
                    {
                        for (int x = 0; x < currentStorageGrid.Dimensions.x; x++)
                        {
                            if (currentStorageGrid.TryGetStash(new UnityEngine.Vector2Int(x, y), out ContainerItemStash currentItemStash))
                            {
                                currentItemStash.TransferItemsFromStash(stash, quantity, storageGrid != this.currentStorages[0].StorageGrids[this.gridIndexes[this.currentStorages[0]]]);
                                numItemsTransferred = originalQuantity - stash.CurrentQuantity;
                                if (numItemsTransferred == quantity) return;
                            }
                        }
                    }
                }
            }
        }
    }
}
