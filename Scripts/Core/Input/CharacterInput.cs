using UnityEngine;
using UnityEngine.InputSystem;

namespace FrigidBlackwaters.Core
{
    public static class CharacterInput
    {
        private static PlayerActions playerActions;
        private static Vector2 lastInputtedMovementVector;
        private static ControlCounter disabled;

        public static Vector2 AimWorldPosition
        {
            get
            {
                return MainCamera.Instance.Camera.ScreenToWorldPoint(playerActions.Character.Aim.ReadValue<Vector2>());
            }
        }

        public static Vector2 CurrentMovementVector
        {
            get
            {
                return playerActions.Character.Movement.ReadValue<Vector2>();
            }
        }

        public static Vector2 LastInputtedMovementVector
        {
            get
            {
                if (CurrentMovementVector != Vector2.zero)
                {
                    lastInputtedMovementVector = CurrentMovementVector;
                }
                return lastInputtedMovementVector;
            }
            set
            {
                lastInputtedMovementVector = value;
            }
        }

        public static bool DashHeld
        {
            get
            {
                return playerActions.Character.Dash.IsPressed();
            }
        }

        public static bool AttackHeld
        {
            get
            {
                return playerActions.Character.Attack.IsPressed();
            }
        }

        public static bool DockHeld
        {
            get
            {
                return playerActions.Character.Dock.IsPressed();
            }
        }

        public static bool EquipFirstPerformedThisFrame
        {
            get
            {
                return playerActions.Character.EquipFirst.WasPressedThisFrame();
            }
        }

        public static bool EquipSecondPerformedThisFrame
        {
            get
            {
                return playerActions.Character.EquipSecond.WasPressedThisFrame();
            }
        }

        public static bool EquipThirdPerformedThisFrame
        {
            get
            {
                return playerActions.Character.EquipThird.WasPressedThisFrame();
            }
        }

        public static ControlCounter Disabled
        {
            get
            {
                return disabled;
            }
        }

        static CharacterInput()
        {
            playerActions = new PlayerActions();
            playerActions.Character.Enable();
            playerActions.Character.Movement.performed += CheckMovementVector;
            disabled = new ControlCounter();
            disabled.OnFirstRequest += playerActions.Character.Disable;
            disabled.OnLastRelease += playerActions.Character.Enable;
        }

        private static void CheckMovementVector(InputAction.CallbackContext callback)
        {
            Vector2 currentMovementVector = playerActions.Character.Movement.ReadValue<Vector2>();
            if (currentMovementVector != Vector2.zero)
            {
                lastInputtedMovementVector = currentMovementVector;
            }
        }
    }
}
