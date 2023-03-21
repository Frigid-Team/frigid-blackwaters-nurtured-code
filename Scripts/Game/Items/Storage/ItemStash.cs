using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class ItemStash
    {
        private ItemStorable storable;
        private List<Item> stackedItems;
        private Action onQuantityUpdated;
        private ItemStorage storage;

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
                return this.storable == null ? int.MaxValue : CalculateMaxCapacity(this.storable);
            }
        }

        public bool IsFull
        {
            get
            {
                return this.storable != null && this.stackedItems.Count >= CalculateMaxCapacity(this.storable);
            }
        }

        public bool HasItemAndIsUsable
        {
            get
            {
                return this.stackedItems.Count > 0 && this.storage.TryGetUsingMob(out _) && this.stackedItems[this.stackedItems.Count - 1].IsUsable;
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

        public bool TryGetTopmostItem(out Item topmostItem)
        {
            if (this.stackedItems.Count > 0)
            {
                topmostItem = this.stackedItems[this.stackedItems.Count - 1];
                return true;
            }
            topmostItem = null;
            return false;
        }

        public bool UseTopmostItem()
        {
            if (this.HasItemAndIsUsable && this.stackedItems[this.stackedItems.Count - 1].Used())
            {
                RemoveItems(1, out _, out _);
                return true;
            }
            return false;
        }

        public bool CanStackStorable(ItemStorable storable)
        {
            return (this.storable == null || storable == this.storable) && CalculateMaxCapacity(storable) > 0;
        }

        public bool TryGetStorable(out ItemStorable storable)
        {
            storable = this.storable;
            return this.stackedItems.Count > 0;
        }

        public void TransferItemsFromStash(ItemStash otherStash, int quantity, bool isOtherSupplier)
        {
            if (otherStash.storable == null || otherStash.storable != this.storable && this.storable != null)
            {
                return;
            }

            int maxQuantity = CalculateMaxCapacity(otherStash.storable);
            int quantityTransferred = 
                Mathf.Clamp(quantity, 0, Mathf.Min(maxQuantity - this.stackedItems.Count, otherStash.stackedItems.Count));

            if (this.storage.CurrencyWallet != otherStash.storage.CurrencyWallet) 
            {
                int cost;
                if (isOtherSupplier)
                {
                    cost = otherStash.CalculateBuyCost(otherStash.storable) * quantityTransferred;
                }
                else
                {
                    cost = CalculateSellCost(otherStash.storable) * quantityTransferred;
                }

                if (!otherStash.storage.CurrencyWallet.TryTransactionFrom(this.storage.CurrencyWallet, cost))
                {
                    return;
                }
            }

            otherStash.RemoveItems(quantityTransferred, out List<Item> transferredItems, out ItemStorable transferredItemStorable);
            if (otherStash.storage != this.storage)
            {
                otherStash.storage.RemoveStoredItems(transferredItems);
                this.storage.AddStoredItems(transferredItems);
            }
            AddItems(transferredItems, transferredItemStorable);
        }

        public void AddItems(List<Item> itemsToAdd, ItemStorable storableToAdd)
        {
            if (storableToAdd != this.storable && this.storable != null)
            {
                return;
            }

            int maxQuantity = CalculateMaxCapacity(storableToAdd);
            int quantityAdded = Mathf.Clamp(itemsToAdd.Count, 0, maxQuantity - this.stackedItems.Count);
            List<Item> addedItems = itemsToAdd.GetRange(0, quantityAdded);
            this.stackedItems.AddRange(addedItems);

            foreach (Item addedItem in addedItems)
            {
                addedItem.Stashed();
            }

            if (this.stackedItems.Count > 0)
            {
                this.storable = storableToAdd;
            }

            this.onQuantityUpdated?.Invoke();
        }

        public void RemoveItems(int quantity, out List<Item> removedItems, out ItemStorable removedStorable)
        {
            int quantityRemoved = Mathf.Clamp(quantity, 0, this.stackedItems.Count);
            removedItems = this.stackedItems.GetRange(this.stackedItems.Count - quantity, quantityRemoved);
            removedStorable = this.storable;
            this.stackedItems.RemoveRange(this.stackedItems.Count - quantity, quantityRemoved);

            foreach (Item removedItem in removedItems)
            {
                removedItem.Unstashed();
            }

            if (this.stackedItems.Count == 0)
            {
                this.storable = null;
            }

            this.onQuantityUpdated?.Invoke();
        }

        public int CalculateBuyCost(ItemStorable storable)
        {
            float modifiedBuyCost = storable.CurrencyValue * (storable.IgnoreBuyCostModifiers && this.storage.BuyCostModifier != 0 ? 1 : this.storage.BuyCostModifier);
            if (modifiedBuyCost > 0)
            {
                return Mathf.Max(1, Mathf.FloorToInt(modifiedBuyCost));
            }
            return 0;
        }

        public int CalculateSellCost(ItemStorable storable)
        {
            float modifiedSellCost = storable.CurrencyValue * (storable.IgnoreBuyCostModifiers && this.storage.SellCostModifier != 0 ? 1 : this.storage.SellCostModifier);
            if (modifiedSellCost > 0)
            {
                return Mathf.Max(1, Mathf.FloorToInt(modifiedSellCost));
            }
            return 0;
        }

        protected abstract int CalculateMaxCapacity(ItemStorable storable);
    }
}
