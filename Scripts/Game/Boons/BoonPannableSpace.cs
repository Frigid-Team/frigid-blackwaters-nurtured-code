using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static FrigidBlackwaters.Game.BoonExchangeLayout;

namespace FrigidBlackwaters.Game
{
    public class BoonPannableSpace : FrigidMonoBehaviour
    {
        [SerializeField]
        private Transform parentBoonsObject;
        [SerializeField]
        private BoonInventory boonInventory;
        [SerializeField]
        private BoonSlotInterface boonObject;

        [SerializeField]
        private Image backgroundImage;

        private List<BoonSlotInterface> boonSlots;

        private BoonExchangeLayout boonLayout;

        private void Awake()
        {
            this.boonSlots = new List<BoonSlotInterface>();
        }

        public void OnMenuOpen(BoonExchangeLayout boonLayout)
        {
            this.boonLayout = boonLayout;
            this.backgroundImage.sprite = boonLayout.BackgroundImage;

            this.boonInventory.OnLoadoutsUpdated += this.RefreshBoonInterfaces;

            this.RefreshBoonInterfaces();
        }

        public void OnMenuClose()
        {
            this.boonInventory.OnLoadoutsUpdated -= this.RefreshBoonInterfaces;

            foreach (BoonSlotInterface boonSlotInterface in this.boonSlots)
            {
                FrigidMonoBehaviour.DestroyInstance(boonSlotInterface);
            }
            this.boonSlots.Clear();
        }

        private void RefreshBoonInterfaces()
        {
            foreach (BoonSlotInterface boonSlotInterface in this.boonSlots)
            {
                FrigidMonoBehaviour.DestroyInstance(boonSlotInterface);
            }

            this.boonSlots.Clear();

            if (!this.boonInventory.TryGetCurrentLoadout(out BoonLoadout currentLoadout))
            {
                return;
            }

            List<(Boon, int)> boons = currentLoadout.GetAllQuantities();
            Dictionary<Boon, int> boonQuantityDictionary = boons.ToDictionary(x => x.Item1, x => x.Item2);

            for (int boonIndex = 0; boonIndex < boonLayout.BoonLocations.Count; boonIndex++)
            {
                BoonSlotInterface boonInterface = FrigidMonoBehaviour.CreateInstance<BoonSlotInterface>(this.boonObject, this.parentBoonsObject, false);
                boonInterface.SetupBoon(boonLayout.BoonLocations[boonIndex].Boon,
                    boonQuantityDictionary.ContainsKey(boonLayout.BoonLocations[boonIndex].Boon) ? boonQuantityDictionary[boonLayout.BoonLocations[boonIndex].Boon] : 0,
                    this.boonInventory.AvailableBoons.Contains(boonLayout.BoonLocations[boonIndex].Boon)
                    );
                boonInterface.gameObject.transform.localPosition = boonLayout.BoonLocations[boonIndex].Position;
                boonInterface.OnPrimaryClick += (Boon boon) => {
                    this.AddBoon(boon);
                    if (this.boonInventory.TryGetCurrentLoadout(out BoonLoadout currentLoadout))
                    {
                        boonInterface.UpdateCurrentQuantity(currentLoadout.GetQuantity(boon));
                    }
                    // TODO Add red indicator when this is false
                };
                boonInterface.OnSecondaryClick += (Boon boon) => {
                    this.SubtractBoon(boon);
                    if (this.boonInventory.TryGetCurrentLoadout(out BoonLoadout currentLoadout))
                    {
                        boonInterface.UpdateCurrentQuantity(currentLoadout.GetQuantity(boon));
                    }
                    // TODO Add red indicator when this is false
                };
                this.boonSlots.Add(boonInterface);
            }
        }

        private bool AddBoon(Boon boon)
        {
            if (!this.boonInventory.TryGetCurrentLoadout(out BoonLoadout currentLoadout) || currentLoadout.GetQuantity(boon) >= boon.MaxLoadoutQuantity || Stamps.CurrentAmount < boon.StampCost)
            {
                return false;
            }
            currentLoadout.SetQuantity(boon, currentLoadout.GetQuantity(boon) + 1);
            return true;
        }

        private bool SubtractBoon(Boon boon)
        {
            if (!this.boonInventory.TryGetCurrentLoadout(out BoonLoadout currentLoadout) || currentLoadout.GetQuantity(boon) <= 0)
            {
                return false;
            }
            currentLoadout.SetQuantity(boon, currentLoadout.GetQuantity(boon) - 1);
            return true;
        }
    }
}