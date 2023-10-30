using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class ItemStorage : SceneAccessible<ItemStorage>
    {
        private static SceneVariable<Dictionary<Mob, ItemStorage>> mobsToStorages;

        [SerializeField]
        private bool hasUsingMob;
        [SerializeField]
        [ShowIfBool("hasUsingMob", true)]
        private Mob usingMob;
        [SerializeField]
        private List<ItemContainer> defaultContainers;
        [SerializeField]
        private IntSerializedReference baseMaxPower;
        [SerializeField]
        private bool isIgnoringTransactionCosts;
        [SerializeField]
        [ShowIfBool("isIgnoringTransactionCosts", false)]
        private IntSerializedReference initialCurrencyCount;
        [SerializeField]
        private bool cannotTransferOutItems;
        [SerializeField]
        [ShowIfBool("cannotTransferOutItems", false)]
        private FloatSerializedReference buyCostModifier;
        [SerializeField]
        private bool cannotTransferInItems;
        [SerializeField]
        [ShowIfBool("cannotTransferInItems", false)]
        private FloatSerializedReference sellCostModifier;
        [SerializeField]
        private bool replenishTakenItems;
        [SerializeField]
        private bool discardReplacedItems;

        private ItemPowerBudget powerBudget;
        private ItemCurrencyWallet currencyWallet;
        private List<ItemStorageGrid> storageGrids;

        private Dictionary<(ItemStorable, Item), Action> itemStores;
        private Action<ItemStorable, Item> onItemStored;
        private Action<ItemStorable, Item> onItemUnstored;

        private HashSet<(ItemStorable, Item)> itemUsages;
        private Action<ItemStorable, Item> onItemUsed;
        private Action<ItemStorable, Item> onItemUnused;

        static ItemStorage()
        {
            mobsToStorages = new SceneVariable<Dictionary<Mob, ItemStorage>>(() => new Dictionary<Mob, ItemStorage>());
        }

        public ItemPowerBudget PowerBudget
        {
            get
            {
                return this.powerBudget;
            }
        }

        public ItemCurrencyWallet CurrencyWallet
        {
            get
            {
                return this.currencyWallet;
            }
        }

        public List<ItemStorageGrid> StorageGrids
        {
            get
            {
                return this.storageGrids;
            }
        }

        public IReadOnlyCollection<(ItemStorable, Item)> ItemStores
        {
            get
            {
                return this.itemStores.Keys;
            }
        }

        public Action<ItemStorable, Item> OnItemStored
        {
            get
            {
                return this.onItemStored;
            }
            set
            {
                this.onItemStored = value;
            }
        }

        public Action<ItemStorable, Item> OnItemUnstored
        {
            get
            {
                return this.onItemUnstored;
            }
            set
            {
                this.onItemUnstored = value;
            }
        }

        public IReadOnlyCollection<(ItemStorable, Item)> ItemUsages
        {
            get
            {
                return this.itemUsages;
            }
        }

        public Action<ItemStorable, Item> OnItemUsed
        {
            get
            {
                return this.onItemUsed;
            }
            set
            {
                this.onItemUsed = value;
            }
        }

        public Action<ItemStorable, Item> OnItemUnused
        {
            get
            {
                return this.onItemUnused;
            }
            set
            {
                this.onItemUnused = value;
            }
        }

        public bool CannotTransferOutItems
        {
            get
            {
                return this.cannotTransferOutItems;
            }
        }

        public float BuyCostModifier
        {
            get
            {
                return this.buyCostModifier.ImmutableValue;
            }
        }

        public bool CannotTransferInItems
        {
            get
            {
                return this.cannotTransferInItems;
            }
        }

        public float SellCostModifier
        {
            get
            {
                return this.sellCostModifier.ImmutableValue;
            }
        }

        public bool ReplenishTakenItems
        {
            get
            {
                return this.replenishTakenItems;
            }
        }

        public bool DiscardReplacedItems
        {
            get
            {
                return this.discardReplacedItems;
            }
        }

        public static bool TryGetStorageUsedByMob(Mob mob, out ItemStorage storage)
        {
            return mobsToStorages.Current.TryGetValue(mob, out storage);
        }

        public bool TryGetUsingMob(out Mob usingMob)
        {
            usingMob = this.usingMob;
            return this.hasUsingMob;
        }

        public void SetStorageGridsFromContainers(List<ItemContainer> containers)
        {
            if (containers.Count == 0)
            {
                Debug.LogWarning("Setting ItemStorageGrids on " + this.name + " with no ItemContainers.");
                return;
            }

            this.storageGrids.Clear();
            foreach (ItemContainer container in containers)
            {
                this.storageGrids.Add(new ItemStorageGrid(container, this));
            }
        }

        public void ItemsStored(ItemStorable storable, IEnumerable<Item> items)
        {
            foreach (Item item in items)
            {
                Action onUsed = 
                    () =>
                    {
                        if (item.InUse)
                        {
                            if (this.itemUsages.Add((storable, item)))
                            {
                                this.onItemUsed?.Invoke(storable, item);
                            }
                        }
                        else
                        {
                            if (this.itemUsages.Remove((storable, item)))
                            {
                                this.onItemUnused?.Invoke(storable, item);
                            }
                        }
                    };
                if (this.itemStores.TryAdd((storable, item), onUsed))
                {
                    item.StoredBy(this);
                    item.transform.SetParent(this.transform);
                    item.OnInUseChanged += onUsed;
                    if (item.InUse && this.itemUsages.Add((storable, item)))
                    {
                        this.onItemUsed?.Invoke(storable, item);
                    }
                    item.Stored();
                    this.onItemStored?.Invoke(storable, item);
                }
            }
        }

        public void ItemsUnstored(ItemStorable storable, IEnumerable<Item> items)
        {
            foreach (Item item in items)
            {
                if (this.itemStores.TryGetValue((storable, item), out Action onUsed))
                {
                    this.itemStores.Remove((storable, item));
                    item.Unstored();
                    item.OnInUseChanged -= onUsed;
                    if (item.InUse && this.itemUsages.Remove((storable, item)))
                    {
                        this.onItemUnused?.Invoke(storable, item);
                    }
                    item.StoredBy(null);
                    item.transform.SetParent(null);
                    this.onItemUnstored?.Invoke(storable, item);
                }
            }
        }

        public List<ContainerItemStash> AddAndCreateItems(ItemStorable storable, int quantity, PickStashCriteria pickStashCriteria) 
        {
            List<ContainerItemStash> pickedStashes = new List<ContainerItemStash>();
            List<Item> createdItems = storable.CreateItems(quantity);
            foreach (ItemStorageGrid storageGrid in this.StorageGrids)
            {
                List<ContainerItemStash> availableStashes = new List<ContainerItemStash>();
                for (int x = 0; x < storageGrid.Dimensions.x; x++)
                {
                    for (int y = 0; y < storageGrid.Dimensions.y; y++)
                    {
                        if (storageGrid.TryGetStash(new Vector2Int(x, y), out ContainerItemStash stash))
                        {
                            if (stash.CanStackStorable(storable) && stash.MaxQuantity - stash.CurrentQuantity >= quantity)
                            {
                                availableStashes.Add(stash);
                            }
                        }
                    }
                }
                while (availableStashes.Count > 0 && createdItems.Count > 0)
                {
                    int chosenStashIndex = -1;
                    switch (pickStashCriteria)
                    {
                        case PickStashCriteria.FirstAvailable:
                            chosenStashIndex = 0;
                            break;
                        case PickStashCriteria.Random:
                            chosenStashIndex = UnityEngine.Random.Range(0, availableStashes.Count);
                            break;
                    }
                    ContainerItemStash pickedStash = availableStashes[chosenStashIndex];
                    pickedStashes.Add(pickedStash);
                    createdItems.RemoveRange(0, pickedStash.PushItems(storable, createdItems));
                    availableStashes.RemoveAt(chosenStashIndex);
                }
            }
            if (createdItems.Count > 0)
            {
                ItemStorable.DiscardItems(createdItems);
            }
            return pickedStashes;
        }

        public List<ContainerItemStash> RemoveAndDiscardItems(ItemStorable storable, int quantity, PickStashCriteria pickStashCriteria)
        {
            List<ContainerItemStash> pickedStashes = new List<ContainerItemStash>();
            List<Item> itemsToDiscard = new List<Item>();
            foreach (ItemStorageGrid itemStorageGrid in this.StorageGrids)
            {
                List<ContainerItemStash> filledStashes = new List<ContainerItemStash>();
                for (int x = 0; x < itemStorageGrid.Dimensions.x; x++)
                {
                    for (int y = 0; y < itemStorageGrid.Dimensions.y; y++)
                    {
                        if (itemStorageGrid.TryGetStash(new Vector2Int(x, y), out ContainerItemStash stash))
                        {
                            if (stash.CanStackStorable(storable) && stash.CurrentQuantity > 0)
                            {
                                filledStashes.Add(stash);
                            }
                        }
                    }
                }
                while (filledStashes.Count > 0 && quantity > 0)
                {
                    int chosenStashIndex = -1;
                    switch (pickStashCriteria)
                    {
                        case PickStashCriteria.FirstAvailable:
                            chosenStashIndex = 0;
                            break;
                        case PickStashCriteria.Random:
                            chosenStashIndex = UnityEngine.Random.Range(0, filledStashes.Count);
                            break;
                    }
                    ContainerItemStash pickedStash = filledStashes[chosenStashIndex];
                    pickedStashes.Add(pickedStash);
                    (ItemStorable _, List<Item> poppedItems) = pickedStash.PopItems(quantity);
                    itemsToDiscard.AddRange(poppedItems);
                    quantity -= poppedItems.Count;
                    filledStashes.RemoveAt(chosenStashIndex);
                }
            }
            ItemStorable.DiscardItems(itemsToDiscard);
            return pickedStashes;
        }

        protected override void Awake()
        {
            base.Awake();
            this.powerBudget = new ItemPowerBudget(this.baseMaxPower.ImmutableValue);
            this.currencyWallet = new ItemCurrencyWallet(this.isIgnoringTransactionCosts, this.initialCurrencyCount.ImmutableValue);
            this.storageGrids = new List<ItemStorageGrid>();
            foreach (ItemContainer container in this.defaultContainers)
            {
                this.storageGrids.Add(new ItemStorageGrid(container, this));
            }

            if (this.storageGrids.Count < 1)
            {
                Debug.LogWarning(this.name + " has an ItemStorage with no default item containers.");
            }

            this.itemStores = new Dictionary<(ItemStorable, Item), Action>();
            this.itemUsages = new HashSet<(ItemStorable, Item)>();

            if (this.TryGetUsingMob(out Mob usingMob))
            {
                if (!mobsToStorages.Current.TryAdd(usingMob, this))
                {
                    Debug.LogError("Using Mob not specified or multiple storages used by Mob!");
                }
            }
        }

        public enum PickStashCriteria
        {
            FirstAvailable,
            Random
        }
    }
}
