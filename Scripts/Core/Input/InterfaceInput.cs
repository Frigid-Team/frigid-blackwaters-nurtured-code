using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FrigidBlackwaters.Core
{
    public static class InterfaceInput
    {
        private static PlayerActions playerActions;
        private static Action onAccess;
        private static Action onReturn;
        private static Action onExpand;
        private static bool quickHeld;
        private static CountingSemaphore disabled;

        public static Vector2 PointPosition
        {
            get
            {
                return playerActions.Interface.Point.ReadValue<Vector2>();
            }
        }

        public static Action OnAccess
        {
            get
            {
                return onAccess;
            }
            set
            {
                onAccess = value;
            }
        }

        public static Action OnReturn
        {
            get
            {
                return onReturn;
            }
            set
            {
                onReturn = value;
            }
        }

        public static Action OnExpand
        {
            get
            {
                return onExpand;
            }
            set
            {
                onExpand = value;
            }
        }

        public static bool QuickHeld
        {
            get
            {
                return quickHeld;
            }
        }

        public static CountingSemaphore Disabled
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
            playerActions.Interface.Access.performed += AccessPerformed;
            playerActions.Interface.Return.performed += ReturnPerformed;
            playerActions.Interface.Expand.performed += ExpandPerformed;
            playerActions.Interface.Quick.started += QuickStarted;
            playerActions.Interface.Quick.canceled += QuickCanceled;
            disabled = new CountingSemaphore();
            disabled.OnFirstRequest += playerActions.Character.Disable;
            disabled.OnLastRelease += playerActions.Character.Enable;
            quickHeld = false;
        }

        private static void AccessPerformed(InputAction.CallbackContext callback)
        {
            onAccess?.Invoke();
        }

        private static void ReturnPerformed(InputAction.CallbackContext callback)
        {
            onReturn?.Invoke();
        }

        private static void ExpandPerformed(InputAction.CallbackContext callback)
        {
            onExpand?.Invoke();
        }

        private static void QuickStarted(InputAction.CallbackContext callback)
        {
            quickHeld = true;
        }

        private static void QuickCanceled(InputAction.CallbackContext callback)
        {
            quickHeld = false;
        }
    }
}
