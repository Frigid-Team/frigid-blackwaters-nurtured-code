using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class InterfaceInput
    {
        private static PlayerActions playerActions;
        private static ControlCounter disabled;

        public static Vector2 PointPosition
        {
            get
            {
                return playerActions.Interface.Point.ReadValue<Vector2>();
            }
        }

        public static bool AccessPerformedThisFrame
        {
            get
            {
                return playerActions.Interface.Access.WasPerformedThisFrame();
            }
        }

        public static bool ReturnPerformedThisFrame
        {
            get
            {
                return playerActions.Interface.Return.WasPerformedThisFrame();
            }
        }

        public static bool ExpandPerformedThisFrame
        {
            get
            {
                return playerActions.Interface.Expand.WasPerformedThisFrame();
            }
        }

        public static bool InteractTriggered
        {
            get
            {
                return playerActions.Interface.Interact.WasPerformedThisFrame();
            }
        }

        public static bool QuickHeld
        {
            get
            {
                return playerActions.Interface.Quick.IsPressed();
            }
        }

        public static ControlCounter Disabled
        {
            get
            {
                return disabled;
            }
        }

        static InterfaceInput()
        {
            playerActions = new PlayerActions();
            playerActions.Interface.Enable();
            disabled = new ControlCounter();
            disabled.OnFirstRequest += playerActions.Character.Disable;
            disabled.OnLastRelease += playerActions.Character.Enable;
        }
    }
}
