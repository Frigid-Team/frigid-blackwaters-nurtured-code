using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ExpeditionSelectMenu : Menu
    {
        [SerializeField]
        private ExpeditionInterface expeditionInterfaceBoard;
        [SerializeField]
        private ProgressionInterface progressionInterface;

        public override void Opened()
        {
            base.Opened();
            PlayerMob.TryGet(out PlayerMob player);
            ExpeditionBoard.TryFindNearest(player.Position, out ExpeditionBoard expeditionBoard, out _);
            this.expeditionInterfaceBoard.Open(expeditionBoard);
            this.progressionInterface.Opened();
        }

        public override void Closed()
        {
            base.Closed();
            this.expeditionInterfaceBoard.Close();
            this.progressionInterface.Closed();
        }

        public override bool WantsToOpen()
        {
            return InterfaceInput.InteractTriggered && PlayerMob.TryGet(out PlayerMob player) && ExpeditionBoard.TryFindNearest(player.Position, out _, out _);
        }

        public override bool WantsToClose()
        {
            return InterfaceInput.InteractTriggered || InterfaceInput.ReturnPerformedThisFrame;
        }

        protected override bool ShouldShowPrompt(out Vector2 trackedPosition)
        {
            trackedPosition = Vector2.zero;
            return PlayerMob.TryGet(out PlayerMob player) && ExpeditionBoard.TryFindNearest(player.Position, out _, out trackedPosition);
        }
    }
}