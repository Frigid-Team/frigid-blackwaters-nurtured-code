using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ItemInterface : FrigidMonoBehaviour
    {
        [Header("Grids")]
        [SerializeField]
        private ItemInterfaceGrid gridPrefab;
        [SerializeField]
        private RectTransform gridsTransform;

        [Header("Container Bars")]
        [SerializeField]
        private ItemInterfaceBar barPrefab;
        [SerializeField]
        private RectTransform barsTransform;

        [Header("Hand")]
        [SerializeField]
        private ItemInterfaceStashSlot handSlot;

        [Header("Tooltip And Popups")]
        [SerializeField]
        private ItemInterfaceTooltip tooltip;

        [Header("Audio")]
        [SerializeField]
        private AudioSource openAudioSource;
        [SerializeField]
        private AudioSource closeAudioSource;
        [SerializeField]
        private AudioSource rotateAudioSource;

        [Header("Optimization")]
        [SerializeField]
        private int numberGridsPreparedInAdvance;
        [SerializeField]
        private int numberBarsPreparedInAdvance;

        private List<ItemStorage> currentStorages;

        private RecyclePool<ItemInterfaceGrid> gridPool;

        private RecyclePool<ItemInterfaceBar> barPool;

        private Dictionary<ItemStorageGrid, ItemInterfaceGrid> mappedGrids;
        private Dictionary<ItemStorage, ItemInterfaceBar> mappedBars;
        private Dictionary<ItemStorage, int> gridIndexes;
        private Dictionary<ItemStorage, Vector2> localCenterPositions;

        private ItemInterfaceHand hand;

        public void OpenStorages(List<ItemStorage> storages)
        {
            this.currentStorages.AddRange(storages);

            SetupInterfaceCenterPositions();
            SetupIndexes();
            PopulateGrids();
            PopulateBars();

            this.handSlot.gameObject.SetActive(true);

            foreach (ItemStorage storage in this.currentStorages)
            {
                this.mappedGrids[storage.StorageGrids[this.gridIndexes[storage]]].TransitionOnScreen(false);
                this.mappedBars[storage].TransitionOnScreen();
            }

            this.hand.CreateHeldItemStash();
            if (this.hand.TryGetHeldItemStash(out HoldingItemStash heldItemStash))
            {
                this.handSlot.PopulateForDisplay(heldItemStash);
            }

            this.tooltip.ShowTooltip(this.hand);

            this.openAudioSource.Play();
        }

        public void CloseStorages()
        {
            this.tooltip.HideTooltip();

            this.handSlot.gameObject.SetActive(false);
            this.hand.RestoreHeldItems();
            this.hand.EraseHeldItemStash();

            foreach (ItemStorage storage in this.currentStorages)
            {
                ItemInterfaceGrid interfaceGrid = this.mappedGrids[storage.StorageGrids[this.gridIndexes[storage]]];
                this.mappedGrids.Remove(storage.StorageGrids[this.gridIndexes[storage]]);
                interfaceGrid.TransitionOffScreen(true, () => this.gridPool.Pool(interfaceGrid));

                ItemInterfaceBar interfaceBar = this.mappedBars[storage];
                this.mappedBars.Remove(storage);
                interfaceBar.TransitionOffScreen(() => this.barPool.Pool(interfaceBar));
            }

            this.gridPool.Pool(this.mappedGrids.Values.ToList());
            this.mappedGrids.Clear();
            this.barPool.Pool(this.mappedBars.Values.ToList());
            this.mappedBars.Clear();

            this.currentStorages.Clear();

            this.closeAudioSource.Play();
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentStorages = new List<ItemStorage>();

            this.gridPool = new RecyclePool<ItemInterfaceGrid>(
                this.numberGridsPreparedInAdvance,
                () => FrigidInstancing.CreateInstance<ItemInterfaceGrid>(this.gridPrefab, this.gridsTransform, false),
                (ItemInterfaceGrid grid) => FrigidInstancing.DestroyInstance(grid)
                );

            this.barPool = new RecyclePool<ItemInterfaceBar>(
                this.numberBarsPreparedInAdvance,
                () => FrigidInstancing.CreateInstance<ItemInterfaceBar>(this.barPrefab, this.barsTransform, false),
                (ItemInterfaceBar bar) => FrigidInstancing.DestroyInstance(bar)
                );

            this.mappedGrids = new Dictionary<ItemStorageGrid, ItemInterfaceGrid>();
            this.mappedBars = new Dictionary<ItemStorage, ItemInterfaceBar>();
            this.localCenterPositions = new Dictionary<ItemStorage, Vector2>();
            this.gridIndexes = new Dictionary<ItemStorage, int>();

            this.hand = new ItemInterfaceHand(this.currentStorages, this.gridIndexes);
        }

        protected override void Start()
        {
            base.Start();
            this.handSlot.gameObject.SetActive(false);
        }

        protected override void Update()
        {
            base.Update();
            this.handSlot.transform.position = InterfaceInput.PointPosition;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void SetupInterfaceCenterPositions()
        {
            this.localCenterPositions.Clear();
            float xInterval = this.gridsTransform.rect.width / Mathf.Max(1, this.currentStorages.Count);
            for (int i = 0; i < this.currentStorages.Count; i++)
            {
                this.localCenterPositions[this.currentStorages[i]] = new Vector2((-this.gridsTransform.rect.width + xInterval) / 2 + i * xInterval, 0);
            }
        }

        private void SetupIndexes()
        {
            foreach (ItemStorage storage in this.currentStorages)
            {
                if (!this.gridIndexes.ContainsKey(storage))
                {
                    this.gridIndexes.Add(storage, 0);
                }
            }
        }

        private void PopulateGrids()
        {
            int totalNumberOfGrids = 0;
            this.currentStorages.ForEach((ItemStorage storage) => { totalNumberOfGrids += storage.StorageGrids.Count; });

            List<ItemInterfaceGrid> grids = this.gridPool.Retrieve(totalNumberOfGrids);

            this.mappedGrids.Clear();
            int count = 0;
            foreach (ItemStorage storage in this.currentStorages)
            {
                foreach (ItemStorageGrid storageGrid in storage.StorageGrids)
                {
                    this.mappedGrids.Add(storageGrid, grids[count]);
                    this.mappedGrids[storageGrid].Populate(storageGrid, this.hand, this.tooltip, this.localCenterPositions[storage]);
                    count++;
                }
            }
        }

        private void PopulateBars()
        {
            List<ItemInterfaceBar> bars = this.barPool.Retrieve(this.currentStorages.Count);

            this.mappedBars.Clear();
            float xInterval = this.gridsTransform.rect.width / Mathf.Max(1, this.currentStorages.Count);
            int count = 0;
            foreach (KeyValuePair<ItemStorage, Vector2> localCenterPositionEntry in this.localCenterPositions)
            {
                ItemStorage storage = localCenterPositionEntry.Key;
                Vector2 localCenterPosition = localCenterPositionEntry.Value;
                this.mappedBars.Add(storage, bars[count]);
                this.mappedBars[storage].Populate(
                    xInterval, 
                    (int index) => RotateGrids(storage, index), 
                    storage.StorageGrids,
                    storage.ItemPowerBudget,
                    storage.ItemCurrencyWallet,
                    this.gridIndexes[storage],
                    localCenterPosition
                    );
                count++;
            }
        }

        private void RotateGrids(ItemStorage storage, int newIndex)
        {
            bool moveUpwards = newIndex > this.gridIndexes[storage];
            this.mappedGrids[storage.StorageGrids[this.gridIndexes[storage]]].TransitionOffScreen(moveUpwards);
            this.gridIndexes[storage] = newIndex;
            this.mappedGrids[storage.StorageGrids[this.gridIndexes[storage]]].TransitionOnScreen(moveUpwards);
            this.mappedBars[storage].Scroll(this.gridIndexes[storage]);
            this.rotateAudioSource.clip = storage.StorageGrids[this.gridIndexes[storage]].ItemContainer.RustleAudioClip;
            this.rotateAudioSource.Play();
        }
    }
}
