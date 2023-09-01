using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ItemPowerBudget
    {
        private int maxPower;
        private int currentPower;
        private Action onCurrentPowerChanged;
        private Action onCurrentPowerChangeFailed;
        private Action onMaxPowerChanged;
        private Action onMaxPowerChangeFailed;
        private Dictionary<Item, int> powerUsages;
        private Dictionary<Item, int> maxPowerAdditions;

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

        public Action OnCurrentPowerChangeFailed
        {
            get
            {
                return this.onCurrentPowerChangeFailed;
            }
            set
            {
                this.onCurrentPowerChangeFailed = value;
            }
        }

        public Action OnMaxPowerChanged
        {
            get
            {
                return this.onMaxPowerChanged;
            }
            set
            {
                this.onMaxPowerChanged = value;
            }
        }

        public Action OnMaxPowerChangeFailed
        {
            get
            {
                return this.onMaxPowerChangeFailed;
            }
            set
            {
                this.onMaxPowerChangeFailed = value;
            }
        }

        public ItemPowerBudget(int baseMaxPower)
        {
            this.maxPower = baseMaxPower;
            this.currentPower = baseMaxPower;
            this.powerUsages = new Dictionary<Item, int>();
            this.maxPowerAdditions = new Dictionary<Item, int>();
        }

        public bool TryUsePower(Item item, int power)
        {
            if (!this.powerUsages.ContainsKey(item) && this.currentPower >= power && power >= 0)
            {
                this.powerUsages.Add(item, power);
                if (power > 0)
                {
                    this.currentPower -= power;
                    this.onCurrentPowerChanged?.Invoke();
                }
                return true;
            }
            this.onCurrentPowerChangeFailed?.Invoke();
            return false;
        }

        public bool TryReleasePower(Item item)
        {
            if (this.powerUsages.ContainsKey(item))
            {
                int power = this.powerUsages[item];
                this.powerUsages.Remove(item);
                if (power > 0)
                {
                    this.currentPower += power;
                    this.onCurrentPowerChanged?.Invoke();
                }
                return true;
            }
            this.onCurrentPowerChangeFailed?.Invoke();
            return false;
        }

        public bool TryIncreaseMaxPower(Item item, int maxPower)
        {
            if (!this.maxPowerAdditions.ContainsKey(item) && maxPower >= 0)
            {
                this.maxPowerAdditions.Add(item, maxPower);
                if (maxPower > 0)
                {
                    this.maxPower += maxPower;
                    this.currentPower += maxPower;
                    this.onCurrentPowerChanged?.Invoke();
                    this.onMaxPowerChanged?.Invoke();
                }
                return true;
            }
            this.onCurrentPowerChangeFailed?.Invoke();
            this.onMaxPowerChangeFailed?.Invoke();
            return false;
        }

        public bool TryDecreaseMaxPower(Item item)
        {
            if (this.maxPowerAdditions.ContainsKey(item))
            {
                int maxPower = this.maxPowerAdditions[item];
                if (this.currentPower >= maxPower)
                {
                    this.maxPowerAdditions.Remove(item);
                    if (maxPower > 0)
                    {
                        this.currentPower -= maxPower;
                        this.maxPower -= maxPower;
                        this.onCurrentPowerChanged?.Invoke();
                        this.onMaxPowerChanged?.Invoke();
                    }
                    return true;
                }
            }
            this.onCurrentPowerChangeFailed?.Invoke();
            this.onMaxPowerChangeFailed?.Invoke();
            return false;
        }
    }
}
