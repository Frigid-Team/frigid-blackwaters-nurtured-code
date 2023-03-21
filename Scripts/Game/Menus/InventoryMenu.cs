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
            return PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out _);
        }

        protected override void Opened()
        {
            List<ItemStorage> itemStorages = new List<ItemStorage>();
            if (PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out ItemStorage playerItemStorage))
            {
                itemStorages.Add(playerItemStorage);
                if (ItemStorage.TryGetNearestFindableStorage(player.Position, itemStorages, out ItemStorage nearbyStorage,  out _))
                {
                    itemStorages.Add(nearbyStorage);
                }
            }
            this.itemInterface.OpenStorages(itemStorages);
        }

        protected override void Closed()
        {
            this.itemInterface.CloseStorages();
        }

        protected override bool ShouldShowPrompt(out Vector2 trackedPosition)
        {
            trackedPosition = Vector2.zero;
            if (PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out ItemStorage playerItemStorage))
            {
                if (ItemStorage.TryGetNearestFindableStorage(player.Position, new List<ItemStorage>() { playerItemStorage }, out _, out Vector2 nearestAccessPosition))
                {
                    trackedPosition = nearestAccessPosition;
                    return true;
                }
            }
            return false;
        }
    }
}
