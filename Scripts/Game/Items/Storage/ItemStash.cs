using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class ItemStash
    {
        private ItemStorable itemStorable;
        private List<Item> stackedItems;
        private Action onQuantityUpdated;
        private List<Mob> usingMobs;
        private ItemPowerBudget itemPowerBudget;
        private ItemCurrencyWallet itemCurrencyWallet;
        private float buyCostModifier;
        private float sellCostModifier;
        private Transform stashTransform;

        public ItemStash(
            List<Mob> usingMobs, 
            ItemPowerBudget itemPowerBudget,
            ItemCurrencyWallet itemCurrencyWallet, 
            float buyCostModifier,
            float sellCostModifier,
            Transform stashTransform
            )
        {
            this.itemStorable = null;
            this.stackedItems = new List<Item>();
            this.usingMobs = usingMobs;
            this.itemPowerBudget = itemPowerBudget;
            this.itemCurrencyWallet = itemCurrencyWallet;
            this.buyCostModifier = buyCostModifier;
            this.sellCostModifier = sellCostModifier;
            this.stashTransform = stashTransform;
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
                return this.itemStorable == null ? int.MaxValue : CalculateMaxCapacity(this.itemStorable);
            }
        }

        public bool IsFull
        {
            get
            {
                return this.itemStorable != null && this.stackedItems.Count >= CalculateMaxCapacity(this.itemStorable);
            }
        }

        public bool HasItemAndIsUsable
        {
            get
            {
                return this.stackedItems.Count > 0 && this.stackedItems[this.stackedItems.Count - 1].IsUsable;
            }
        }

        public bool HasUsingMobs
        {
            get
            {
                return this.usingMobs.Count > 0;
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
            if (this.stackedItems.Count > 0 && this.stackedItems[this.stackedItems.Count - 1].Used(this.usingMobs, this.itemPowerBudget))
            {
                RemoveItems(1, out List<Item> removedItems, out ItemStorable removedItemStorable);
                return true;
            }
            return false;
        }

        public bool CanStackItemStorable(ItemStorable itemStorable)
        {
            return (this.itemStorable == null || itemStorable == this.itemStorable) && CalculateMaxCapacity(itemStorable) > 0;
        }

        public bool TryGetItemStorable(out ItemStorable itemStorable)
        {
            itemStorable = this.itemStorable;
            return this.stackedItems.Count > 0;
        }

        public void RemoveItems(int quantity, out List<Item> removedItems, out ItemStorable removedItemStorable)
        {
            int quantityRemoved = Mathf.Clamp(quantity, 0, this.stackedItems.Count);
            removedItems = this.stackedItems.GetRange(this.stackedItems.Count - quantity, quantityRemoved);
            removedItemStorable = this.itemStorable;
            this.stackedItems.RemoveRange(this.stackedItems.Count - quantity, quantityRemoved);

            foreach (Item removedItem in removedItems)
            {
                removedItem.Unstashed(this.usingMobs, this.itemPowerBudget);
                removedItem.transform.SetParent(null);
            }

            if (this.stackedItems.Count == 0)
            {
                this.itemStorable = null;
            }

            this.onQuantityUpdated?.Invoke();
        }

        public void AddItems(List<Item> itemsToAdd, ItemStorable itemStorableToAdd)
        {
            if (itemStorableToAdd != this.itemStorable && this.itemStorable != null)
            {
                return;
            }

            int maxQuantity = CalculateMaxCapacity(itemStorableToAdd);
            int quantityAdded = Mathf.Clamp(itemsToAdd.Count, 0, maxQuantity - this.stackedItems.Count);
            List<Item> addedItems = itemsToAdd.GetRange(0, quantityAdded);
            this.stackedItems.AddRange(addedItems);

            foreach (Item addedItem in addedItems)
            {
                addedItem.transform.SetParent(this.stashTransform);
                addedItem.Stashed(this.usingMobs, this.itemPowerBudget);
            }

            if (this.stackedItems.Count > 0)
            {
                this.itemStorable = itemStorableToAdd;
            }

            this.onQuantityUpdated?.Invoke();
        }

        public void TransferItemsFromStash(ItemStash otherItemStash, int quantity, bool isOtherSupplier)
        {
            if (otherItemStash.itemStorable == null || otherItemStash.itemStorable != this.itemStorable && this.itemStorable != null)
            {
                return;
            }

            int maxQuantity = CalculateMaxCapacity(otherItemStash.itemStorable);
            int quantityTransferred = 
                Mathf.Clamp(quantity, 0, Mathf.Min(maxQuantity - this.stackedItems.Count, otherItemStash.stackedItems.Count));

            if (this.itemCurrencyWallet != otherItemStash.itemCurrencyWallet) 
            {
                int cost;
                if (isOtherSupplier)
                {
                    cost = otherItemStash.CalculateBuyCost(otherItemStash.itemStorable) * quantityTransferred;
                }
                else
                {
                    cost = CalculateSellCost(otherItemStash.itemStorable) * quantityTransferred;
                }

                if (!otherItemStash.itemCurrencyWallet.TryTransactionFrom(this.itemCurrencyWallet, cost))
                {
                    return;
                }
            }

            otherItemStash.RemoveItems(quantityTransferred, out List<Item> removedItems, out ItemStorable removedItemStorable);
            AddItems(removedItems, removedItemStorable);
        }

        public int CalculateBuyCost(ItemStorable itemStorable)
        {
            float modifiedBuyCost = itemStorable.CurrencyValue * (itemStorable.IgnoreBuyCostModifiers && this.buyCostModifier != 0 ? 1 : this.buyCostModifier);
            if (modifiedBuyCost > 0)
            {
                return Mathf.Max(1, Mathf.FloorToInt(modifiedBuyCost));
            }
            return 0;
        }

        public int CalculateSellCost(ItemStorable itemStorable)
        {
            float modifiedSellCost = itemStorable.CurrencyValue * (itemStorable.IgnoreBuyCostModifiers && this.sellCostModifier != 0 ? 1 : this.sellCostModifier);
            if (modifiedSellCost > 0)
            {
                return Mathf.Max(1, Mathf.FloorToInt(modifiedSellCost));
            }
            return 0;
        }

        protected abstract int CalculateMaxCapacity(ItemStorable itemStorable);
    }
}
