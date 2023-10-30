using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "GiveItemBoon", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Boons + "GiveItemBoon")]
    public class GiveItemBoon : Boon
    {
        [SerializeField]
        private List<GiveItemGroup> giveItemGroups;

        public override void ActivateQuantity(int quantity)
        {
            base.ActivateQuantity(quantity);
            foreach (GiveItemGroup giveItemGroup in this.giveItemGroups)
            {
                giveItemGroup.StartGivingQuantity(quantity);
            }
        }

        public override void DeactivateQuantity(int quantity)
        {
            base.DeactivateQuantity(quantity);
            foreach (GiveItemGroup giveItemGroup in this.giveItemGroups)
            {
                giveItemGroup.StopGivingQuantity(quantity);
            }
        }

        [Serializable]
        private class GiveItemGroup
        {
            [SerializeField]
            private List<MobSpawnable> mobSpawnables;
            [SerializeField]
            private List<ItemAllocation> itemAllocations;

            public void StartGivingQuantity(int quantity)
            {
                foreach (MobSpawnable mobSpawnable in this.mobSpawnables)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        mobSpawnable.OnSpawned += this.GiveItems;
                    }
                }
            }

            public void StopGivingQuantity(int quantity)
            {
                foreach (MobSpawnable mobSpawnable in this.mobSpawnables)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        mobSpawnable.OnSpawned -= this.GiveItems;
                    }
                }
            }

            private void GiveItems(Mob mob)
            {
                ItemStorage.TryGetStorageUsedByMob(mob, out ItemStorage storage);
                foreach (ItemAllocation itemAllocation in this.itemAllocations)
                {
                    List<ContainerItemStash> stashesModified = storage.AddAndCreateItems(itemAllocation.ItemStorable, itemAllocation.Quantity, ItemStorage.PickStashCriteria.FirstAvailable);
                    if (itemAllocation.UseWhenGiven)
                    {
                        foreach (ContainerItemStash stashModified in stashesModified)
                        {
                            stashModified.UseTopmostItem();
                        }
                    }
                }
            }

            [Serializable]
            private struct ItemAllocation
            {
                [SerializeField]
                private ItemStorable itemStorable;
                [SerializeField]
                private bool useWhenGiven;
                [SerializeField]
                private int quantity;

                public ItemStorable ItemStorable
                {
                    get
                    {
                        return this.itemStorable;
                    }
                }

                public bool UseWhenGiven
                {
                    get
                    {
                        return this.useWhenGiven;
                    }
                }

                public int Quantity
                {
                    get
                    {
                        return this.quantity;
                    }
                }
            }
        }
    }
}