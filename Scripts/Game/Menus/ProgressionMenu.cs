using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ProgressionMenu : Menu
    {
        [SerializeField]
        private ProgressionInterface progressionInterface;

        public override void Opened()
        {
            base.Opened();
            this.progressionInterface.Opened();
        }

        public override void Closed()
        {
            base.Closed();
            this.progressionInterface.Closed();
        }

        public override bool WantsToOpen()
        {
            return InterfaceInput.InteractTriggered;
        }

        public override bool WantsToClose()
        {
            return InterfaceInput.InteractTriggered || InterfaceInput.ReturnPerformedThisFrame;
        }
    }
}
