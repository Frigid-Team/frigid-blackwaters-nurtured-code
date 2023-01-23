using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ContainerItemStash : ItemStash
    {
        private ItemContainer itemContainer;

        public ContainerItemStash(
            ItemContainer itemContainer, 
            List<Mob> usingMobs,
            ItemPowerBudget itemPowerBudget,
            ItemCurrencyWallet itemCurrencyWallet,
            float buyCostModifier,
            float sellCostModifier,
            Transform stashTransform
            ) : base(usingMobs, itemPowerBudget, itemCurrencyWallet, buyCostModifier, sellCostModifier, stashTransform)
        {
            this.itemContainer = itemContainer;
        }

        protected override int CalculateMaxCapacity(ItemStorable itemStorable)
        {
            return this.itemContainer.CalculateMaxCapacityFromStorable(itemStorable);
        }
    }
}
