using UnityEngine;
using UnityEngine.InputSystem;

namespace FrigidBlackwaters.Core
{
    public static class CharacterInput
    {
        private static PlayerActions playerActions;
        private static Vector2 lastInputtedMovementVector;
        private static bool dashHeld;
        private static bool attackHeld;
        private static bool dockHeld;
        private static CountingSemaphore disabled;

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
                return dashHeld;
            }
        }

        public static bool AttackHeld
        {
            get
            {
                return attackHeld;
            }
        }

        public static bool DockHeld
        {
            get
            {
                return dockHeld;
            }
        }

        public static CountingSemaphore Disabled
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
            playerActions.Character.Dash.started += DashStarted;
            playerActions.Character.Dash.canceled += DashCanceled;
            playerActions.Character.Attack.started += AttackStarted;
            playerActions.Character.Attack.canceled += AttackCanceled;
            playerActions.Character.Dock.started += DockStarted;
            playerActions.Character.Dock.canceled += DockCanceled;
            disabled = new CountingSemaphore();
            disabled.OnFirstRequest += playerActions.Character.Disable;
            disabled.OnLastRelease += playerActions.Character.Enable;
            dashHeld = false;
            attackHeld = false;
            dockHeld = false;
        }

        public static void LockMovementVector(Vector2 temporaryLastInputtedMovementVector)
        {
            lastInputtedMovementVector = temporaryLastInputtedMovementVector;
            playerActions.Character.Movement.performed -= CheckMovementVector;
        }

        public static void UnlockMovementVector()
        {
            CheckMovementVector(new InputAction.CallbackContext());
            playerActions.Character.Movement.performed += CheckMovementVector;
        }

        private static void CheckMovementVector(InputAction.CallbackContext callback)
        {
            Vector2 currentMovementVector = playerActions.Character.Movement.ReadValue<Vector2>();
            if (currentMovementVector != Vector2.zero)
            {
                lastInputtedMovementVector = currentMovementVector;
            }
        }

        private static void DashStarted(InputAction.CallbackContext callback)
        {
            dashHeld = true;
        }

        private static void DashCanceled(InputAction.CallbackContext callback)
        {
            dashHeld = false;
        }

        private static void AttackStarted(InputAction.CallbackContext callback)
        {
            attackHeld = true;
        }

        private static void AttackCanceled(InputAction.CallbackContext callback)
        {
            attackHeld = false;
        }

        private static void DockStarted(InputAction.CallbackContext callback)
        {
            dockHeld = true;
        }

        private static void DockCanceled(InputAction.CallbackContext callback)
        {
            dockHeld = false;
        }
    }
}
