using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ItemStorage : FrigidMonoBehaviour
    {
        private static SceneVariable<HashSet<ItemStorage>> findableStorages;

        [SerializeField]
        private List<AccessZone> accessZones;
        [SerializeField]
        private List<ItemContainer> defaultItemContainers;
        [SerializeField]
        private List<Mob> usingMobs;
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

        private ItemPowerBudget itemPowerBudget;
        private ItemCurrencyWallet itemCurrencyWallet;
        private List<ItemStorageGrid> storageGrids;

        static ItemStorage()
        {
            findableStorages = new SceneVariable<HashSet<ItemStorage>>(() => { return new HashSet<ItemStorage>(); });
        }

        public List<Mob> UsingMobs
        {
            get
            {
                return this.usingMobs;
            }
        }

        public ItemPowerBudget ItemPowerBudget
        {
            get
            {
                return this.itemPowerBudget;
            }
        }

        public ItemCurrencyWallet ItemCurrencyWallet
        {
            get
            {
                return this.itemCurrencyWallet;
            }
        }

        public List<ItemStorageGrid> StorageGrids
        {
            get
            {
                return this.storageGrids;
            }
        }

        public static bool TryFindNearestActiveStorage(Vector2 position, List<ItemStorage> excludedStorages, out ItemStorage nearbyStorage, out Vector2 nearestAbsoluteAccessPosition)
        {
            float closestDistance = float.MaxValue;
            nearbyStorage = null;
            nearestAbsoluteAccessPosition = position;
            bool foundStorage = false;
            foreach (ItemStorage findableStorage in findableStorages.Current)
            {
                if (!excludedStorages.Contains(findableStorage) &&
                    findableStorage.IsAccessibleFromPosition(position, out Vector2 accessPosition) &&
                    Vector2.Distance(accessPosition, position) < closestDistance)
                {
                    closestDistance = Vector2.Distance(accessPosition, position);
                    nearbyStorage = findableStorage;
                    nearestAbsoluteAccessPosition = accessPosition;
                    foundStorage = true;
                }
            }
            return foundStorage;
        }

        public static List<ItemStorage> FindStoragesUsedByMob(Mob mob)
        {
            List<ItemStorage> foundStorages = new List<ItemStorage>();
            foreach (ItemStorage findableStorage in findableStorages.Current)
            {
                if (findableStorage.UsingMobs.Contains(mob))
                {
                    foundStorages.Add(findableStorage);
                }
            }
            return foundStorages;
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

        public void SetStorageGridsFromContainers(List<ItemContainer> itemContainers)
        {
            if (itemContainers.Count == 0)
            {
                Debug.LogWarning("Setting storage grids on " + this.name + " with no item containers.");
                return;
            }

            this.storageGrids.Clear();
            foreach (ItemContainer itemContainer in itemContainers)
            {
                this.storageGrids.Add(
                    new ItemStorageGrid(
                        itemContainer,
                        this.usingMobs,
                        this.itemPowerBudget,
                        this.itemCurrencyWallet,
                        this.buyCostModifier.ImmutableValue,
                        this.sellCostModifier.ImmutableValue,
                        this.transform
                        )
                    );
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.itemPowerBudget = new ItemPowerBudget(this.baseMaxPower.ImmutableValue);
            this.itemCurrencyWallet = new ItemCurrencyWallet(this.isIgnoringTransactionCosts, this.initialCurrencyCount.ImmutableValue);
            this.storageGrids = new List<ItemStorageGrid>();
            foreach (ItemContainer itemContainer in this.defaultItemContainers)
            {
                this.storageGrids.Add(
                    new ItemStorageGrid(
                        itemContainer,
                        this.usingMobs,
                        this.itemPowerBudget,
                        this.itemCurrencyWallet,
                        this.buyCostModifier.ImmutableValue,
                        this.sellCostModifier.ImmutableValue,
                        this.transform
                        )
                    );
            }

            if (this.storageGrids.Count < 1)
            {
                Debug.LogError(this.name + " has an item storage with no default item containers.");
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
