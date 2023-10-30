using UnityEngine;
using System.Collections.Generic;
using System;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "BoonInventory", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Boons + "BoonInventory")]
    public class BoonInventory : FrigidScriptableObject
    {
        private const string NumberLoadoutsSaveKey = "Boon_Loadout_Count";
        private const string LoadoutBoonsSaveKeyPrefix = "Boon_Loadout_Boons_";
        private const string LoadoutQuantitiesSaveKeyPrefix = "Boon_Loadout_Quantities_";
        private const string CurrentLoadoutIndexSaveKey = "Current_Loadout_Index";
        private const string AvailableBoonsSaveKey = "Available_Boons";

        [SerializeField]
        private List<Boon> initiallyUnlockedBoons;

        private List<BoonLoadout> loadouts;
        private int currentLoadoutIndex;

        private List<Boon> availableBoons;

        private Action onLoadoutsUpdated;
        private Action onAvailableBoonsUpdated;

        public int NumberLoadouts
        {
            get
            {
                return this.loadouts.Count;
            }
        }

        public int CurrentLoadoutIndex
        {
            get
            {
                return this.currentLoadoutIndex;
            }
        }

        public IReadOnlyList<Boon> AvailableBoons
        {
            get
            {
                return this.availableBoons;
            }
        }

        public Action OnLoadoutsUpdated
        {
            get
            {
                return this.onLoadoutsUpdated;
            }
            set
            {
                this.onLoadoutsUpdated = value;
            }
        }

        public Action OnAvailableBoonsUpdated
        {
            get
            {
                return this.onAvailableBoonsUpdated;
            }
            set
            {
                this.onAvailableBoonsUpdated = value;
            }
        }
        
        public bool TryGetCurrentLoadout(out BoonLoadout currentLoadout)
        {
            return this.TryGetLoadout(this.currentLoadoutIndex, out currentLoadout);
        }

        public bool TryGetLoadout(int index, out BoonLoadout loadout)
        {
            if (index >= 0 && index < this.NumberLoadouts)
            {
                loadout = this.loadouts[index];
                return true;
            }
            loadout = null;
            return false;
        }

        public void SwitchCurrentLoadout(int currentLoadoutIndex)
        {
            if (this.loadouts.Count == 0)
            {
                return;
            }
            this.currentLoadoutIndex = Mathf.Clamp(currentLoadoutIndex, 0, this.NumberLoadouts - 1);
            this.TryGetLoadout(currentLoadoutIndex, out BoonLoadout loadout);
            Stamps.CurrentAmount = Stamps.TotalAmount - loadout.StampCost;
            this.onLoadoutsUpdated?.Invoke();
            this.WriteToSave();
        }

        public void SwapLoadouts(int indexA, int indexB)
        {
            if(this.loadouts.Count <= 1)
            {
                return;
            }
            (this.loadouts[indexA], this.loadouts[indexB]) = (this.loadouts[indexB], this.loadouts[indexA]);
            this.onLoadoutsUpdated?.Invoke();
            this.WriteToSave();
        }

        public void AddLoadout(int index)
        {
            index = Mathf.Clamp(index, 0, this.NumberLoadouts);
            this.loadouts.Insert(index, new BoonLoadout());
            this.onLoadoutsUpdated?.Invoke();
            this.WriteToSave();
        }

        public void RemoveLoadout(int index)
        {
            index = Mathf.Clamp(index, 0, this.NumberLoadouts - 1);
            this.loadouts.RemoveAt(index);
            this.onLoadoutsUpdated?.Invoke();
            this.WriteToSave();
        }

        public void UnlockBoons(IEnumerable<Boon> boons)
        {
            foreach (Boon boon in boons)
            {
                if (!this.availableBoons.Contains(boon))
                {
                    this.availableBoons.Add(boon);
                }
            }
            this.onAvailableBoonsUpdated?.Invoke();
            this.WriteToSave();
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            this.loadouts = new List<BoonLoadout>() { new BoonLoadout() };
            this.currentLoadoutIndex = 0;
            this.availableBoons = new List<Boon>(this.initiallyUnlockedBoons);

            this.ReadFromSave();
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            this.WriteToSave();
        }

        private void ReadFromSave()
        {
            ActivelyBusy.Request(
                () =>
                {
                    SaveFileSystem.ReadSaveFile(
                        this.name,
                        (SaveFileData saveFileData) =>
                        {
                            if (!saveFileData.IsEmpty)
                            {
                                this.loadouts = new List<BoonLoadout>();
                                int numberLoadouts = saveFileData.GetInt(NumberLoadoutsSaveKey);
                                for (int loadoutIndex = 0; loadoutIndex < numberLoadouts; loadoutIndex++)
                                {
                                    BoonLoadout loadout = new BoonLoadout();
                                    Boon[] boons = saveFileData.GetAssetArray<Boon>(LoadoutBoonsSaveKeyPrefix + loadoutIndex);
                                    int[] quantities = saveFileData.GetIntArray(LoadoutQuantitiesSaveKeyPrefix + loadoutIndex);
                                    for (int boonIndex = 0; boonIndex < boons.Length; boonIndex++)
                                    {
                                        loadout.SetQuantity(boons[boonIndex], quantities[boonIndex]);
                                    }
                                    this.loadouts.Add(loadout);
                                }
                                this.currentLoadoutIndex = saveFileData.GetInt(CurrentLoadoutIndexSaveKey);
                                this.availableBoons = new List<Boon>(saveFileData.GetAssetArray<Boon>(AvailableBoonsSaveKey));
                            }
                            ActivelyBusy.Release();
                        }
                        );
                }
                );
        }

        private void WriteToSave()
        {
            SaveFileData saveFileData = new SaveFileData();
            saveFileData.SetInt(NumberLoadoutsSaveKey, this.loadouts.Count);
            for (int loadoutIndex = 0; loadoutIndex < this.loadouts.Count; loadoutIndex++)
            {
                BoonLoadout loadout = this.loadouts[loadoutIndex];
                List<(Boon, int)> boonQuantities = loadout.GetAllQuantities();
                Boon[] boons = new Boon[boonQuantities.Count];
                int[] quantities = new int[boonQuantities.Count];
                for (int boonQuantityIndex = 0; boonQuantityIndex <  boonQuantities.Count; boonQuantityIndex++)
                {
                    (Boon boon, int quantity) = boonQuantities[boonQuantityIndex];
                    boons[boonQuantityIndex] = boon;
                    quantities[boonQuantityIndex] = quantity;
                }
                saveFileData.SetAssetArray<Boon>(LoadoutBoonsSaveKeyPrefix + loadoutIndex, boons);
                saveFileData.SetIntArray(LoadoutQuantitiesSaveKeyPrefix + loadoutIndex, quantities);
            }
            saveFileData.SetInt(CurrentLoadoutIndexSaveKey, this.currentLoadoutIndex);
            saveFileData.SetAssetArray<Boon>(AvailableBoonsSaveKey, this.availableBoons.ToArray());
            PassivelyBusy.Request(() => SaveFileSystem.WriteSaveFile(this.name, saveFileData, PassivelyBusy.Release));
        }
    }
}
