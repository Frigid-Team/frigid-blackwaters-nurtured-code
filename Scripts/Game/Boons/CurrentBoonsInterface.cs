using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class CurrentBoonsInterface : FrigidMonoBehaviour
    {
        [SerializeField]
        private Transform boonsParentTransform;
        [SerializeField]
        private BoonSlotInterface boonInterfacePrefab;

        private List<BoonSlotInterface> currentBoons;
        private BoonInventory boonInventory;

        private RecyclePool<BoonSlotInterface> pool;

        protected override void Awake()
        {
            base.Awake();
            this.currentBoons = new List<BoonSlotInterface>();
        }

        public void OnMenuOpen(BoonInventory boonInventory)
        {
            this.boonInventory = boonInventory;
            this.boonInventory.OnLoadoutsUpdated += this.RefreshBoonInterfaces;

            this.RefreshBoonInterfaces();
        }

        public void OnMenuClose()
        {
            this.boonInventory.OnLoadoutsUpdated -= this.RefreshBoonInterfaces;
        }

        private void RefreshBoonInterfaces()
        {
            if (!this.boonInventory.TryGetCurrentLoadout(out BoonLoadout currentLoadout))
            {
                return;
            }

            List<(Boon, int)> boons = currentLoadout.GetAllQuantities();

            currentLoadout.OnBoonQuantityChanged += this.UpdateBoonQuantity;

            foreach(Transform child in this.boonsParentTransform)
            {
                FrigidMonoBehaviour.DestroyInstance(child.gameObject.GetComponent<FrigidMonoBehaviour>());
            }

            this.currentBoons = new List<BoonSlotInterface>();

            foreach ((Boon, int) boon in boons)
            {
                if (boon.Item2 > 0)
                {
                    BoonSlotInterface boonInterface = FrigidMonoBehaviour.CreateInstance<BoonSlotInterface>(this.boonInterfacePrefab, this.boonsParentTransform, false);
                    boonInterface.SetupBoon(boon.Item1, boon.Item2, true);
                    boonInterface.OnPrimaryClick += this.PanToBoon;
                    this.currentBoons.Add(boonInterface);
                }
            }
        }

        private void UpdateBoonQuantity(Boon boon, int newQuantity)
        {
            foreach(BoonSlotInterface boonSlot in this.currentBoons)
            {
                if(boonSlot.Boon == boon)
                {
                    boonSlot.UpdateCurrentQuantity(newQuantity);
                    return;
                }
            }

            if(newQuantity == 1)
            {
                BoonSlotInterface boonInterface = FrigidMonoBehaviour.CreateInstance<BoonSlotInterface>(this.boonInterfacePrefab, this.boonsParentTransform, false);
                boonInterface.SetupBoon(boon, newQuantity, true);
                boonInterface.OnPrimaryClick += this.PanToBoon;
                this.currentBoons.Add(boonInterface);
            }
        }

        private void PanToBoon(Boon boon)
        {
            Debug.Log("TODO::: PAN TO THIS BOON " + boon.DisplayName);
        }

    }
}