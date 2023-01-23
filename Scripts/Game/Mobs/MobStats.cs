using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobStats
    {
        private Dictionary<MobStat, StatValue> statValues;

        public StatValue GetStatValue(MobStat stat)
        {
            return this.statValues[stat];
        }

        public MobStats()
        {
            this.statValues = new Dictionary<MobStat, StatValue>();
            for (int i = 0; i < (int)MobStat.Count; i++)
            {
                this.statValues.Add((MobStat)i, new StatValue(0));
            }
        }

        public class StatValue
        {
            private int baseAmount;
            private int bonusAmount;
            private int amount;
            private Action<int, int> onAmountChanged;

            public int Amount
            {
                get
                {
                    return this.amount;
                }
            }

            public int BonusAmount
            {
                get
                {
                    return this.bonusAmount;
                }
                set
                {
                    this.bonusAmount = value;
                    int previousAmount = this.amount;
                    this.amount = this.baseAmount + this.bonusAmount;
                    if (previousAmount != this.amount) this.onAmountChanged?.Invoke(previousAmount, this.amount);
                }
            }

            public Action<int, int> OnAmountChanged
            {
                get
                {
                    return this.onAmountChanged;
                }
                set
                {
                    this.onAmountChanged = value;
                }
            }

            public StatValue(int baseAmount)
            {
                this.baseAmount = baseAmount;
                this.amount = this.baseAmount;
                this.bonusAmount = 0;
            }
        }
    }
}
