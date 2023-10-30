using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class ItemStash
    {
        private ItemStorable storable;
        private List<Item> stackedItems;
        private Action onQuantityUpdated;

        private ItemStorage storage;

        private Action onAnyInUseChanged;
        private Action onAllStorageChangeableChanged;

        public ItemStash(ItemStorage storage)
        {
            this.storable = null;
            this.stackedItems = new List<Item>();
            this.storage = storage;
        }

        public int CurrentQuantity
        {
            get
            {
                return this.stackedItems.Count;
            }
        }

        public int MaxQuantity
        {
            get
            {
                return this.storable == null ? int.MaxValue : this.CalculateMaxCapacity(this.storable);
            }
        }

        public Action OnQuantityUpdated
        {
            get
            {
                return this.onQuantityUpdated;
            }
            set
            {
                this.onQuantityUpdated = value;
            }
        }

        public bool IsFull
        {
            get
            {
                return this.storable != null && this.stackedItems.Count >= this.CalculateMaxCapacity(this.storable);
            }
        }

        public ItemStorage Storage
        {
            get
            {
                return this.storage;
            }
        }

        public bool AnyInUse
        {
            get
            {
                bool anyInUse = false;
                foreach (Item stackedItem in this.stackedItems)
                {
                    anyInUse |= stackedItem.InUse;
                }
                return anyInUse;
            }
        }

        public Action OnAnyInUseChanged
        {
            get
            {
                return this.onAnyInUseChanged;
            }
            set
            {
                this.onAnyInUseChanged = value;
            }
        }

        public bool AllStorageChangeable
        {
            get
            {
                bool allStorageChangeable = true;
                foreach (Item stackedItem in this.stackedItems)
                {
                    allStorageChangeable &= stackedItem.StorageChangeable;
                }
                return allStorageChangeable;
            }
        }

        public Action OnAllStorageChangeableChanged
        {
            get
            {
                return this.onAllStorageChangeableChanged;
            }
            set
            {
                this.onAllStorageChangeableChanged = value;
            }
        }

        public bool CanUseTopmostItem()
        {
            return 
                this.stackedItems.Count > 0 && 
                this.stackedItems[this.stackedItems.Count - 1].IsUsable && 
                (!this.storable.IsUniqueUse || this.AnyInUse || !this.Storage.ItemStores.Any(((ItemStorable storable, Item item) itemStore) => itemStore.storable == this.storable && itemStore.item.InUse));
        }

        public bool UseTopmostItem()
        {
            if (this.CanUseTopmostItem() && this.stackedItems[this.stackedItems.Count - 1].Used())
            {
                this.DecreaseStack(1);
                return true;
            }
            return false;
        }

        public bool CanStackStorable(ItemStorable storable)
        {
            return (this.storable == null || storable == this.storable) && this.CalculateMaxCapacity(storable) > 0;
        }

        public bool TryGetStorable(out ItemStorable storable)
        {
            storable = this.storable;
            return this.stackedItems.Count > 0;
        }

        public bool CanTransferFrom(ItemStash otherStash) 
        {
            return
                otherStash.storable != null &&
                (this.CanStackStorable(otherStash.storable) || this.Storage.DiscardReplacedItems) &&
                (otherStash.AllStorageChangeable || this.Storage == otherStash.Storage) && 
                !otherStash.Storage.CannotTransferOutItems && !this.Storage.CannotTransferInItems;
        }

        public bool DoesTransferInvolveTransaction(ItemStash otherStash)
        {
            return
               this.Storage.CurrencyWallet != otherStash.Storage.CurrencyWallet && 
               (!this.Storage.CurrencyWallet.IsIgnoringTransactionCosts || !otherStash.Storage.CurrencyWallet.IsIgnoringTransactionCosts) &&
               (otherStash.Storage.BuyCostModifier > 0 || otherStash.Storage.SellCostModifier > 0 || this.Storage.BuyCostModifier > 0 || this.Storage.SellCostModifier > 0);
        }

        public void TransferItemsFromStash(ItemStash otherStash, int quantity, bool isOtherSupplier)
        {
            if (!this.CanTransferFrom(otherStash))
            {
                return;
            }

            int maxQuantity = this.CalculateMaxCapacity(otherStash.storable);

            if (this.Storage.DiscardReplacedItems) 
            {
                (ItemStorable _, List<Item> poppedItems) = this.PopItems(this.storable == otherStash.storable ? Mathf.Max(quantity - (maxQuantity - this.CurrentQuantity), 0) : this.CurrentQuantity);
                ItemStorable.DiscardItems(poppedItems);
            }

            int quantityTransferred = Mathf.Clamp(quantity, 0, Mathf.Min(maxQuantity - this.stackedItems.Count, otherStash.stackedItems.Count));
            if (quantityTransferred == 0)
            {
                return;
            }

            if (this.DoesTransferInvolveTransaction(otherStash)) 
            {
                int cost;
                if (isOtherSupplier)
                {
                    cost = otherStash.CalculateBuyCost(otherStash.storable) * quantityTransferred;
                }
                else
                {
                    cost = this.CalculateSellCost(otherStash.storable) * quantityTransferred;
                }

                if (!otherStash.Storage.CurrencyWallet.TryTransactionFrom(this.Storage.CurrencyWallet, cost))
                {
                    return;
                }
            }

            (ItemStorable transferredStorable, List<Item> transferredItems) = otherStash.DecreaseStack(quantityTransferred);
            if (otherStash.Storage != this.Storage)
            {
                otherStash.Storage.ItemsUnstored(transferredStorable, transferredItems);
                this.Storage.ItemsStored(transferredStorable, transferredItems);
            }
            this.IncreaseStack(transferredStorable, transferredItems);

            if (otherStash.Storage.ReplenishTakenItems)
            {
                otherStash.PushItems(this.storable, this.storable.CreateItems(quantityTransferred));
            }
        }

        public int PushItems(ItemStorable storable, List<Item> itemsToPush)
        {
            int quantity = this.IncreaseStack(storable, itemsToPush);
            this.Storage.ItemsStored(storable, itemsToPush.GetRange(0, quantity));
            return quantity;
        }

        public (ItemStorable, List<Item>) PopItems(int quantity)
        {
            (ItemStorable poppedStorable, List<Item> poppedItems) = this.DecreaseStack(quantity);
            this.Storage.ItemsUnstored(poppedStorable, poppedItems);
            return (poppedStorable, poppedItems);
        }

        public int CalculateBuyCost(ItemStorable storable)
        {
            float modifiedBuyCost = storable.CurrencyValue * (storable.IgnoreBuyCostModifiers && this.Storage.BuyCostModifier != 0 ? 1 : this.Storage.BuyCostModifier);
            if (modifiedBuyCost > 0)
            {
                return Mathf.Max(1, Mathf.FloorToInt(modifiedBuyCost));
            }
            return 0;
        }

        public int CalculateSellCost(ItemStorable storable)
        {
            float modifiedSellCost = storable.CurrencyValue * (storable.IgnoreBuyCostModifiers && this.Storage.SellCostModifier != 0 ? 1 : this.Storage.SellCostModifier);
            if (modifiedSellCost > 0)
            {
                return Mathf.Max(1, Mathf.FloorToInt(modifiedSellCost));
            }
            return 0;
        }

        protected abstract int CalculateMaxCapacity(ItemStorable storable);

        private int IncreaseStack(ItemStorable storableToPush, List<Item> itemsToPush)
        {
            if (storableToPush != this.storable && this.storable != null)
            {
                return 0;
            }

            bool wasAnyInUse = this.AnyInUse;
            bool wasAllStorageChangeable = this.AllStorageChangeable;

            int maxQuantity = this.CalculateMaxCapacity(storableToPush);
            int quantityAdded = Mathf.Clamp(itemsToPush.Count, 0, maxQuantity - this.stackedItems.Count);
            List<Item> pushedItems = itemsToPush.GetRange(0, quantityAdded);
            this.stackedItems.AddRange(pushedItems);

            foreach (Item pushedItem in pushedItems)
            {
                pushedItem.OnInUseChanged += this.DetectAnyInUseChange;
                pushedItem.OnStorageChangeableChanged += this.DetectAllStorageChangeableChange;
            }

            if (this.stackedItems.Count > 0)
            {
                this.storable = storableToPush;
            }
            if (pushedItems.Count > 0) this.onQuantityUpdated?.Invoke();

            if (this.AnyInUse != wasAnyInUse) this.onAnyInUseChanged?.Invoke();
            if (this.AllStorageChangeable != wasAllStorageChangeable) this.onAllStorageChangeableChanged?.Invoke();

            return quantityAdded;
        }

        private (ItemStorable, List<Item>) DecreaseStack(int quantity)
        {
            bool wasAnyInUse = this.AnyInUse;
            bool wasAllStorageChangeable = this.AllStorageChangeable;

            int quantityRemoved = Mathf.Clamp(quantity, 0, this.stackedItems.Count);
            List<Item> poppedItems = this.stackedItems.GetRange(this.stackedItems.Count - quantityRemoved, quantityRemoved);
            ItemStorable poppedStorable = this.storable;
            this.stackedItems.RemoveRange(this.stackedItems.Count - quantityRemoved, quantityRemoved);

            foreach (Item poppedItem in poppedItems)
            {
                poppedItem.OnInUseChanged -= this.DetectAnyInUseChange;
                poppedItem.OnStorageChangeableChanged -= this.DetectAllStorageChangeableChange;
            }

            if (this.stackedItems.Count == 0)
            {
                this.storable = null;
            }
            if (poppedItems.Count > 0) this.onQuantityUpdated?.Invoke();

            if (this.AnyInUse != wasAnyInUse) this.onAnyInUseChanged?.Invoke();
            if (this.AllStorageChangeable != wasAllStorageChangeable) this.onAllStorageChangeableChanged?.Invoke();

            return (poppedStorable, poppedItems);
        }

        private void DetectAnyInUseChange()
        {
            int numInUse = 0;
            foreach (Item stackedItem in this.stackedItems)
            {
                if (stackedItem.InUse) numInUse++;
            }
            if (numInUse == 1 || numInUse == 0)
            {
                this.onAnyInUseChanged?.Invoke();
            }
        }

        private void DetectAllStorageChangeableChange()
        {
            int numStorageChangeable = 0;
            foreach (Item stackedItem in this.stackedItems)
            {
                if (stackedItem.StorageChangeable) numStorageChangeable++;
            }
            if (numStorageChangeable == this.stackedItems.Count - 1 || numStorageChangeable == this.stackedItems.Count)
            {
                this.onAllStorageChangeableChanged?.Invoke();
            }
        }
    }
}
