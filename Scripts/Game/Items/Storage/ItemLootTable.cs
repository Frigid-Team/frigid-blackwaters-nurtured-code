using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ItemLootTable", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.ITEMS + "ItemLootTable")]
    public class ItemLootTable : FrigidScriptableObject
    {
        [SerializeField]
        private List<LootGroup> lootGroups;

        public List<ItemLootRoll> GenerateLootRolls()
        {
            List<ItemLootRoll> generatedLootRolls = new List<ItemLootRoll>();
            foreach (LootGroup lootGroup in this.lootGroups)
            {
                generatedLootRolls.AddRange(lootGroup.GenerateLootRolls());
            }
            return generatedLootRolls;
        }

        [Serializable]
        private class LootGroup
        {
            [SerializeField]
            private int minNumberRolls;
            [SerializeField]
            private int maxNumberRolls;
            [SerializeField]
            private RelativeWeightPool<LootEntry> lootEntries;

            public List<ItemLootRoll> GenerateLootRolls()
            {
                List<ItemLootRoll> generatedLootRolls = new List<ItemLootRoll>();
                int numberRolls = UnityEngine.Random.Range(this.minNumberRolls, this.maxNumberRolls + 1);
                foreach (LootEntry lootEntry in this.lootEntries.Retrieve(numberRolls))
                {
                    generatedLootRolls.AddRange(lootEntry.GenerateLootRolls());
                }
                return generatedLootRolls;
            }
        }

        [Serializable]
        private class LootEntry
        {
            [SerializeField]
            private bool isNested;
            [SerializeField]
            [ShowIfBool("isNested", false)]
            private List<ItemStorable> itemStorables;
            [SerializeField]
            [ShowIfPreviouslyShown(true)]
            private int minQuantity;
            [SerializeField]
            [ShowIfPreviouslyShown(true)]
            private int maxQuantity;
            [SerializeField]
            [ShowIfBool("isNested", true)]
            private ItemLootTable nestedLootTable;

            public List<ItemLootRoll> GenerateLootRolls()
            {
                if (this.isNested)
                {
                    return this.nestedLootTable.GenerateLootRolls();
                }
                else
                {
                    List<ItemLootRoll> generatedLootRolls = new List<ItemLootRoll>();
                    if (this.itemStorables.Count > 0)
                    {
                        ItemStorable randomItemStorable = this.itemStorables[UnityEngine.Random.Range(0, this.itemStorables.Count)];
                        int quantity = UnityEngine.Random.Range(this.minQuantity, this.maxQuantity + 1);
                        generatedLootRolls.Add(new ItemLootRoll(randomItemStorable, quantity));
                    }
                    return generatedLootRolls;
                }
            }
        }
    }
}
