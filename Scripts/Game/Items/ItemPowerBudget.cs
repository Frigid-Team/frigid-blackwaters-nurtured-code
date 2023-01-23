using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ItemPowerBudget
    {
        private int maxPower;
        private int currentPower;
        private Action onCurrentPowerChanged;
        private Action onUsePowerFailed;
        private Dictionary<Item, int> powerUsages;

        public int MaxPower
        {
            get
            {
                return this.maxPower;
            }
        }

        public int CurrentPower
        {
            get
            {
                return this.currentPower;
            }
        }

        public Action OnCurrentPowerChanged
        {
            get
            {
                return this.onCurrentPowerChanged;
            }
            set
            {
                this.onCurrentPowerChanged = value;
            }
        }

        public Action OnUsePowerFailed
        {
            get
            {
                return this.onUsePowerFailed;
            }
            set
            {
                this.onUsePowerFailed = value;
            }
        }

        public ItemPowerBudget(int maxPower)
        {
            this.maxPower = maxPower;
            this.currentPower = maxPower;
            this.powerUsages = new Dictionary<Item, int>();
        }

        public bool TryUsePower(Item item, int power)
        {
            TryReleasePower(item);
            if (this.currentPower >= power)
            {
                this.powerUsages.Add(item, power);
                this.currentPower -= power;
                if (power > 0)
                {
                    this.onCurrentPowerChanged?.Invoke();
                }
                return true;
            }
            this.onUsePowerFailed?.Invoke();
            return false;
        }

        public bool TryReleasePower(Item item)
        {
            if (this.powerUsages.ContainsKey(item))
            {
                int power = this.powerUsages[item];
                this.currentPower += power;
                this.powerUsages.Remove(item);
                if (power > 0)
                {
                    this.onCurrentPowerChanged?.Invoke();
                }
                return true;
            }
            return false;
        }
    }
}
