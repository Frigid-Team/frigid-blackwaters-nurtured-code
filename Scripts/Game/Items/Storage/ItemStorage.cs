using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class ItemStorage : FrigidMonoBehaviour
    {
        private static SceneVariable<HashSet<ItemStorage>> findableStorages;
        private static SceneVariable<Dictionary<Mob, ItemStorage>> mobsToStorages;

        [SerializeField]
        private bool hasUsingMob;
        [SerializeField]
        [ShowIfBool("hasUsingMob", true)]
        private Mob usingMob;
        [SerializeField]
        private List<AccessZone> accessZones;
        [SerializeField]
        private List<ItemContainer> defaultContainers;
        [SerializeField]
        private IntSerializedReference baseMaxPower;
        [SerializeField]
        private bool isIgnoringTransactionCosts;
        [SerializeField]
        private IntSerializedReference initialCurrencyCount;
        [SerializeField]
        private FloatSerializedReference buyCostModifier;
        [SerializeField]
        private FloatSerializedReference sellCostModifier;

        private ItemPowerBudget powerBudget;
        private ItemCurrencyWallet currencyWallet;
        private List<ItemStorageGrid> storageGrids;
        private HashSet<Item> storedItems;

        static ItemStorage()
        {
            findableStorages = new SceneVariable<HashSet<ItemStorage>>(() => new HashSet<ItemStorage>());
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

        public float BuyCostModifier
        {
            get
            {
                return this.buyCostModifier.ImmutableValue;
            }
        }

        public float SellCostModifier
        {
            get
            {
                return this.sellCostModifier.ImmutableValue;
            }
        }

        public static bool TryGetNearestFindableStorage(Vector2 position, List<ItemStorage> excludedStorages, out ItemStorage nearbyStorage, out Vector2 nearestAccessPosition)
        {
            float closestDistance = float.MaxValue;
            nearbyStorage = null;
            nearestAccessPosition = position;
            bool foundStorage = false;
            foreach (ItemStorage findableStorage in findableStorages.Current)
            {
                if (!excludedStorages.Contains(findableStorage) &&
                    findableStorage.IsAccessibleFromPosition(position, out Vector2 accessPosition) &&
                    Vector2.Distance(accessPosition, position) < closestDistance)
                {
                    closestDistance = Vector2.Distance(accessPosition, position);
                    nearbyStorage = findableStorage;
                    nearestAccessPosition = accessPosition;
                    foundStorage = true;
                }
            }
            return foundStorage;
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

        public bool IsAccessibleFromPosition(Vector2 position, out Vector2 accessPosition)
        {
            float closestDistance = float.MaxValue;
            accessPosition = position;
            bool foundAccess = false;
            foreach (AccessZone accessZone in this.accessZones)
            {
                if (position.WithinBox(accessZone.AccessPoint, accessZone.AccessSize))
                {
                    if (Vector2.Distance(position, accessZone.AccessPoint) < closestDistance)
                    {
                        closestDistance = Vector2.Distance(position, accessZone.AccessPoint);
                        accessPosition = accessZone.AccessPoint;
                        foundAccess = true;
                    }
                }
            }
            return foundAccess;
        }

        public void SetStorageGridsFromContainers(List<ItemContainer> containers)
        {
            if (containers.Count == 0)
            {
                Debug.LogWarning("Setting storage grids on " + this.name + " with no item containers.");
                return;
            }

            this.storageGrids.Clear();
            foreach (ItemContainer container in containers)
            {
                this.storageGrids.Add(new ItemStorageGrid(container, this));
            }
        }

        public void AddStoredItems(IEnumerable<Item> items)
        {
            foreach (Item item in items)
            {
                if (this.storedItems.Add(item))
                {
                    item.Assign(this);
                    item.transform.SetParent(this.transform);
                    item.Stored();
                }
            }
        }

        public void RemoveStoredItems(IEnumerable<Item> items)
        {
            foreach (Item item in items)
            {
                if (this.storedItems.Remove(item))
                {
                    item.Unstored();
                    item.Unassign();
                    item.transform.SetParent(null);
                }
            }
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
                Debug.LogError(this.name + " has an ItemStorage with no default item containers.");
            }
            this.storedItems = new HashSet<Item>();

            if (TryGetUsingMob(out Mob usingMob))
            {
                if (!mobsToStorages.Current.TryAdd(usingMob, this))
                {
                    Debug.LogError("Using mob not specified or multiple storages used by mob!");
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            findableStorages.Current.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            findableStorages.Current.Remove(this);
        }

        [Serializable]
        private struct AccessZone
        {
            [SerializeField]
            private Vector2SerializedReference accessSize;
            [SerializeField]
            private Transform accessPointTransform;

            public Vector2 AccessSize
            {
                get
                {
                    return this.accessSize.ImmutableValue;
                }
            }

            public Vector2 AccessPoint
            {
                get
                {
                    return this.accessPointTransform.position;
                }
            }
        }
    }
}
