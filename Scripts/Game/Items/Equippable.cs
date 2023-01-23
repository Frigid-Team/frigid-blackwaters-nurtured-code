using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class Equippable : Item
    {
        [SerializeField]
        private List<ItemRule> equippedItemRules;
        [SerializeField]
        private IntSerializedReference equipPowerUsage;

        private bool isEquipped;

        public override bool IsUsable
        {
            get
            {
                return true;
            }
        }

        public override bool IsInEffect
        {
            get
            {
                return this.isEquipped;
            }
        }

        public override bool Used(List<Mob> usingMobs, ItemPowerBudget itemPowerBudget)
        {
            if (!this.isEquipped)
            {
                if (itemPowerBudget.TryUsePower(this, this.equipPowerUsage.ImmutableValue))
                {
                    this.isEquipped = true;
                    foreach (ItemRule equippedItemRule in this.equippedItemRules)
                    {
                        ApplyItemRule(usingMobs, equippedItemRule);
                    }
                }
            }
            else
            {
                if (itemPowerBudget.TryReleasePower(this))
                {
                    this.isEquipped = false;
                    foreach (ItemRule equippedItemRule in this.equippedItemRules)
                    {
                        UnapplyItemRule(usingMobs, equippedItemRule);
                    }
                }
            }
            return false;
        }

        public override void Unstashed(List<Mob> usingMobs, ItemPowerBudget itemPowerBudget)
        {
            if (itemPowerBudget.TryReleasePower(this) && this.isEquipped)
            {
                this.isEquipped = false;
                foreach (ItemRule equippedItemRule in this.equippedItemRules)
                {
                    UnapplyItemRule(usingMobs, equippedItemRule);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.isEquipped = false;
        }
    }
}
