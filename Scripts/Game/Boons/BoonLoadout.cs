using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class BoonLoadout
    {
        private Dictionary<Boon, int> boonQuantities;
        private Action<Boon, int> onBoonQuantityChanged;

        private bool isActivated;
        private int stampCost;

        public Action<Boon, int> OnBoonQuantityChanged
        {
            get
            {
                return this.onBoonQuantityChanged;
            }
            set
            {
                this.onBoonQuantityChanged = value;
            }
        }

        public int StampCost
        {
            get
            {
                return this.stampCost;
            }

        }

        public BoonLoadout()
        {
            this.boonQuantities = new Dictionary<Boon, int>();
            this.isActivated = false;
        }

        public BoonLoadout(BoonLoadout other)
        {
            this.boonQuantities = new Dictionary<Boon, int>(other.boonQuantities);
            this.isActivated = other.isActivated;
        }

        public bool Activate()
        {
            if (this.isActivated) return false;

            this.isActivated = true;
            foreach (KeyValuePair<Boon, int> boonQuantity in this.boonQuantities)
            {
                Boon boon = boonQuantity.Key;
                int quantity = boonQuantity.Value;
                boon.ActivateQuantity(quantity);
            }
            return true;
        }

        public bool Deactivate()
        {
            if (!this.isActivated) return false;

            this.isActivated = false;
            foreach (KeyValuePair<Boon, int> boonQuantity in this.boonQuantities)
            {
                Boon boon = boonQuantity.Key;
                int quantity = boonQuantity.Value;
                boon.DeactivateQuantity(quantity);
            }
            return true;
        }

        public List<(Boon, int)> GetAllQuantities()
        {
            List<(Boon, int)> allBoonQuantities = new List<(Boon, int)>();
            foreach (KeyValuePair<Boon, int> boonQuantity in this.boonQuantities)
            {
                allBoonQuantities.Add((boonQuantity.Key, boonQuantity.Value));
            }
            return allBoonQuantities;
        }

        public int GetQuantity(Boon boon)
        {
            if (this.boonQuantities.TryGetValue(boon, out int quantity))
            {
                return quantity;
            }
            return 0;
        }

        public void SetQuantity(Boon boon, int quantity)
        {
            if (quantity <= 0)
            {
                this.boonQuantities.Remove(boon);
                this.onBoonQuantityChanged?.Invoke(boon, quantity);
                return;
            }
            
            if (boon.MaxLoadoutQuantity > 0)
            {
                quantity = Mathf.Clamp(quantity, 0, boon.MaxLoadoutQuantity);
            }
            bool increased = quantity > this.GetQuantity(boon);
            this.stampCost += boon.StampCost * (increased ? 1 : -1);
            Stamps.CurrentAmount += boon.StampCost * (!increased ? 1 : -1);

            if (!this.boonQuantities.TryAdd(boon, quantity))
            {
                this.boonQuantities[boon] = quantity;
            }
            this.onBoonQuantityChanged?.Invoke(boon, quantity);
        }
    }
}
