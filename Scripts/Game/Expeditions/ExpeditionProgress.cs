using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ExpeditionProgress", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Expeditions + "ExpeditionProgress")]
    public class ExpeditionProgress : FrigidScriptableObject
    {
        private const string CurrentExpeditionSaveKey = "Current_Expedition";
        private const string CompletedExpeditionsSaveKey = "Completed_Expeditions";
        private const string AvailableExpeditionsSaveKey = "Available_Expeditions";

        [SerializeField]
        private BoonInventory boonInventoryForExpeditions;
        [SerializeField]
        private List<Expedition> startingExpeditions;

        private bool departed;
        private Expedition currentExpedition;
        private List<Expedition> completedExpeditions;
        private List<Expedition> availableExpeditions;
        private Action onUpdated;

        public IReadOnlyList<Expedition> CompletedExpeditions
        {
            get
            {
                return this.completedExpeditions;
            }
        }

        public IReadOnlyList<Expedition> AvailableExpeditions
        {
            get
            {
                return this.availableExpeditions;
            }
        }

        public Action OnUpdated
        {
            get
            {
                return this.onUpdated;
            }
            set
            {
                this.onUpdated = value;
            }
        }

        public bool TryGetSelectedExpedition(out Expedition selectedExpedition)
        {
            if (this.currentExpedition == null || this.departed)
            {
                selectedExpedition = null;
                return false;
            }
            selectedExpedition = this.currentExpedition;
            return true;
        }

        public bool TryGetActiveExpedition(out Expedition activeExpedition)
        {
            if (this.currentExpedition == null || !this.departed)
            {
                activeExpedition = null;
                return false;
            }
            activeExpedition = this.currentExpedition;
            return true;
        }

        public bool SelectExpedition(Expedition selectedExpedition)
        {
            if (selectedExpedition == null || this.departed)
            {
                return false;
            }
            this.currentExpedition = selectedExpedition;
            this.onUpdated?.Invoke();
            this.WriteToSave();
            return true;
        }

        public bool DepartOnExpedition()
        {
            // Use empty loadout if no loadout is available.
            BoonLoadout boonLoadoutForExpedition = this.boonInventoryForExpeditions.TryGetCurrentLoadout(out BoonLoadout currentBoonLoadout) ? currentBoonLoadout : new BoonLoadout();
            if (this.currentExpedition == null || this.departed || !this.currentExpedition.Depart(boonLoadoutForExpedition, this.CompleteExpedition))
            {
                return false;
            }
            this.departed = true;
            this.onUpdated?.Invoke();
            this.WriteToSave();
            return true;
        }

        public bool ReturnFromExpedition()
        {
            if (this.currentExpedition == null || !this.departed || !this.currentExpedition.Return())
            {
                return false;
            }

            this.currentExpedition = null;
            this.departed = false;
            this.onUpdated?.Invoke();
            this.WriteToSave();
            return true;
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            this.departed = false;

            this.currentExpedition = null;
            this.completedExpeditions = new List<Expedition>();
            this.availableExpeditions = new List<Expedition>(this.startingExpeditions);

            this.ReadFromSave();
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            this.WriteToSave();
        }

        private void CompleteExpedition()
        {
            if (this.currentExpedition == null || this.completedExpeditions.Contains(this.currentExpedition))
            {
                return;
            }
            this.completedExpeditions.Add(this.currentExpedition);
            Stamps.TotalAmount += this.currentExpedition.NumberAwardedStamps;
            Stamps.CurrentAmount += this.currentExpedition.NumberAwardedStamps;

            foreach (Expedition unlockedExpedition in this.currentExpedition.ExpeditionsUnlockedOnCompletion)
            {
                if (!this.availableExpeditions.Contains(unlockedExpedition))
                {
                    this.availableExpeditions.Add(unlockedExpedition);
                }
            }

            this.boonInventoryForExpeditions.UnlockBoons(this.currentExpedition.BoonsUnlockedOnCompletion);

            this.onUpdated?.Invoke();
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
                                this.currentExpedition = saveFileData.GetAsset<Expedition>(CurrentExpeditionSaveKey);
                                this.completedExpeditions = new List<Expedition>(saveFileData.GetAssetArray<Expedition>(CompletedExpeditionsSaveKey));
                                this.availableExpeditions = new List<Expedition>(saveFileData.GetAssetArray<Expedition>(AvailableExpeditionsSaveKey));
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
            saveFileData.SetAsset<Expedition>(CurrentExpeditionSaveKey, this.currentExpedition);
            saveFileData.SetAssetArray<Expedition>(CompletedExpeditionsSaveKey, this.completedExpeditions.ToArray());
            saveFileData.SetAssetArray<Expedition>(AvailableExpeditionsSaveKey, this.availableExpeditions.ToArray());
            PassivelyBusy.Request(() => SaveFileSystem.WriteSaveFile(this.name, saveFileData, PassivelyBusy.Release));
        }
    }
}