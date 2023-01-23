using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class InventoryMenu : Menu
    {
        [Header("Inventory")]
        [SerializeField]
        private ItemInterface itemInterface;

        public override bool IsOpenable()
        {
            /*
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob))
            {
                return ItemStorage.FindStoragesUsedByMob(recentPlayerMob).Count > 0;
            }
            */
            return false;
        }

        protected override void Opened()
        {
            List<ItemStorage> itemStorages = new List<ItemStorage>();
            /*
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob))
            {
                itemStorages.AddRange(ItemStorage.FindStoragesUsedByMob(recentPlayerMob));
                if (ItemStorage.TryFindNearestActiveStorage(
                    recentPlayerMob.transform.position,
                    itemStorages, 
                    out ItemStorage nearbyStorage, 
                    out Vector2 nearestAbsoluteAccessPosition
                    ))
                {
                    itemStorages.Add(nearbyStorage);
                }
            }
            */
            this.itemInterface.OpenStorages(itemStorages);
        }

        protected override void Closed()
        {
            this.itemInterface.CloseStorages();
        }

        protected override bool ShouldShowPrompt(out Vector2 trackedAbsolutePosition)
        {
            trackedAbsolutePosition = Vector2.zero;
            /*
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob))
            {
                if (ItemStorage.TryFindNearestActiveStorage(
                    recentPlayerMob.transform.position,
                    ItemStorage.FindStoragesUsedByMob(recentPlayerMob),
                    out ItemStorage nearbyStorage,
                    out Vector2 nearestAbsoluteAccessPosition
                    ))
                {
                    trackedAbsolutePosition = nearestAbsoluteAccessPosition;
                    return true;
                }
            }
            */
            return false;
        }
    }
}
