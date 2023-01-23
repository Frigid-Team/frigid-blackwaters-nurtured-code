using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class HoldingItemStash : ItemStash
    {
        public HoldingItemStash(
            List<Mob> usingMobs, 
            ItemPowerBudget itemPowerBudget,
            ItemCurrencyWallet itemCurrencyWallet,
            float buyCostModifier,
            float sellCostModifier,
            Transform stashTransform
            ) : base(usingMobs, itemPowerBudget, itemCurrencyWallet, buyCostModifier, sellCostModifier, stashTransform) { }

        protected override int CalculateMaxCapacity(ItemStorable itemStorable)
        {
            return int.MaxValue;
        }
    }
}
