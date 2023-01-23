using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class Consumable : Item
    {
        [SerializeField]
        private List<ItemRuleApplication> itemRuleApplications;
        [SerializeField]
        private IntSerializedReference consumePowerUsage;

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
                return false;
            }
        }

        public override bool Used(List<Mob> usingMobs, ItemPowerBudget itemPowerBudget)
        {
            if (itemPowerBudget.TryUsePower(this, this.consumePowerUsage.ImmutableValue))
            {
                foreach (ItemRuleApplication itemRuleApplication in this.itemRuleApplications)
                {
                    FrigidCoroutine.Run(
                        itemRuleApplication.ApplyRoutine.GetRoutine(
                            onIterationComplete:
                            () =>
                            {
                                UnapplyItemRule(usingMobs, itemRuleApplication.ItemRule);
                                ApplyItemRule(usingMobs, itemRuleApplication.ItemRule);
                            },
                            onComplete: () =>
                            {
                                if (!itemRuleApplication.LastsForever) UnapplyItemRule(usingMobs, itemRuleApplication.ItemRule);
                            }
                            ),
                        this.gameObject
                        );
                }
                return true;
            }
            return false;
        }

        [Serializable]
        private struct ItemRuleApplication
        {
            [SerializeField]
            private ItemRule itemRule;
            [SerializeField]
            private TweenCoroutineTemplate applyRoutine;
            [SerializeField]
            private bool lastsForever;

            public ItemRule ItemRule
            {
                get
                {
                    return this.itemRule;
                }
            }

            public TweenCoroutineTemplate ApplyRoutine
            {
                get
                {
                    return this.applyRoutine;
                }
            }

            public bool LastsForever
            {
                get
                {
                    return this.lastsForever;
                }
            }
        }
    }
}
