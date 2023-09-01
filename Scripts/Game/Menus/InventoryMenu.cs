using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class InventoryMenu : Menu
    {
        [Header("Inventory")]
        [SerializeField]
        private ItemInterface itemInterface;

        public override bool WantsToOpen()
        {
            return InterfaceInput.AccessPerformedThisFrame && PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out _);
        }

        public override bool WantsToClose()
        {
            return InterfaceInput.AccessPerformedThisFrame || InterfaceInput.ReturnPerformedThisFrame;
        }

        public override void Opened()
        {
            base.Opened();
            List<ItemStorage> itemStorages = new List<ItemStorage>();
            if (PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out ItemStorage playerItemStorage))
            {
                itemStorages.Add(playerItemStorage);
                if (ItemStorage.TryFindNearest(player.Position, itemStorages, out ItemStorage nearbyStorage,  out _))
                {
                    itemStorages.Add(nearbyStorage);
                }
            }
            this.itemInterface.OpenStorages(itemStorages);
        }

        public override void Closed()
        {
            base.Closed();
            this.itemInterface.CloseStorages();
        }

        protected override bool ShouldShowPrompt(out Vector2 trackedPosition)
        {
            trackedPosition = Vector2.zero;
            if (PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out ItemStorage playerItemStorage))
            {
                if (ItemStorage.TryFindNearest(player.Position, new List<ItemStorage>() { playerItemStorage }, out _, out Vector2 nearestAccessPosition))
                {
                    trackedPosition = nearestAccessPosition;
                    return true;
                }
            }
            return false;
        }
    }
}
