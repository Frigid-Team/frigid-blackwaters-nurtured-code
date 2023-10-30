using FrigidBlackwaters.Core;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class BoonMenu : Menu
    {
        [SerializeField]
        private BoonExchangeInterface boonExhangeInterface;

        public override void Opened()
        {
            base.Opened();
            PlayerMob.TryGet(out PlayerMob player);
            BoonAccessible.TryFindNearest(player.Position, out BoonAccessible boonAccessible, out _);
            this.boonExhangeInterface.Open(boonAccessible.BoonLayout, boonAccessible.BoonInventory);
        }

        public override void Closed()
        {
            base.Closed();
            this.boonExhangeInterface.Close();
        }

        public override bool WantsToOpen()
        {
            return InterfaceInput.InteractTriggered && PlayerMob.TryGet(out PlayerMob player) && BoonAccessible.TryFindNearest(player.Position, out _, out _);
        }

        public override bool WantsToClose()
        {
            return InterfaceInput.InteractTriggered || InterfaceInput.ReturnPerformedThisFrame;
        }

        protected override bool ShouldShowPrompt(out Vector2 trackedPosition)
        {
            trackedPosition = Vector2.zero;
            return PlayerMob.TryGet(out PlayerMob player) && BoonAccessible.TryFindNearest(player.Position, out _, out trackedPosition);
        }
    }
}