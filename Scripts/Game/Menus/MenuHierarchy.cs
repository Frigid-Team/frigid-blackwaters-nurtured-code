using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MenuHierarchy : FrigidMonoBehaviour
    {
        [Header("Menus")]
        [SerializeField]
        private Menu onReturnMenu;
        [SerializeField]
        private List<Menu> onAccessMenus;
        [SerializeField]
        private List<Menu> onExpandMenus;
        [SerializeField]
        private Transform menusTransform;

        [Header("Backdrop")]
        [SerializeField]
        private Image backdropImage;
        [SerializeField]
        private float backdropFadeAlpha;
        [SerializeField]
        private float backdropFadeDuration;

        [Header("Pointer")]
        [SerializeField]
        private Image pointerImage;

        private Menu currentMenu;
        private FrigidCoroutine backdropRoutine;

        protected override void Awake()
        {
            base.Awake();
            this.pointerImage.enabled = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InterfaceInput.OnReturn += ReturnMenus;
            InterfaceInput.OnAccess += ToggleOnAccessMenus;
            InterfaceInput.OnExpand += ToggleOnExpandMenus;
            if (this.currentMenu != null)
            {
                CharacterInput.Disabled.Request();
                TimePauser.Paused.Request();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InterfaceInput.OnReturn -= ReturnMenus;
            InterfaceInput.OnAccess -= ToggleOnAccessMenus;
            InterfaceInput.OnExpand -= ToggleOnExpandMenus;
            if (this.currentMenu != null)
            {
                CharacterInput.Disabled.Release();
                TimePauser.Paused.Release();
            }
        }

        protected override void Update()
        {
            base.Update();
            this.pointerImage.transform.position = InterfaceInput.PointPosition;
        }

        private void ReturnMenus()
        {
            if (this.currentMenu != null)
            {
                if (this.currentMenu.IsClosable())
                {
                    CloseMenu(this.currentMenu);
                }
            }
            else
            {
                if (this.onReturnMenu.IsOpenable())
                {
                    OpenMenu(this.onReturnMenu);
                }
            }
        }

        private void ToggleOnAccessMenus()
        {
            ToggleMenuOnShortcut(this.onAccessMenus);
        }

        private void ToggleOnExpandMenus()
        {
            ToggleMenuOnShortcut(this.onExpandMenus);
        }

        private void ToggleMenuOnShortcut(List<Menu> menus)
        {
            Menu chosenMenu = null;
            if (menus.Contains(this.currentMenu))
            {
                chosenMenu = this.currentMenu;
            }
            else
            {
                foreach (Menu menu in menus)
                {
                    if (menu.IsOpenable())
                    {
                        chosenMenu = menu;
                        break;
                    }
                }
            }
            ToggleMenu(chosenMenu);
        }

        private void ToggleMenu(Menu newMenu)
        {
            if (newMenu == null) return;

            if (newMenu == this.currentMenu)
            {
                if (this.currentMenu.IsClosable())
                {
                    CloseMenu(this.currentMenu);
                }
                return;
            }

            if (newMenu.IsOpenable())
            {
                if (this.currentMenu != null)
                {
                    if (this.currentMenu.IsClosable())
                    {
                        CloseMenu(this.currentMenu);
                    }
                    else
                    {
                        return;
                    }
                }
                OpenMenu(newMenu);
            }
        }

        private void OpenMenu(Menu targetMenu)
        {
            targetMenu.Open();
            targetMenu.transform.SetParent(this.menusTransform);
            targetMenu.transform.SetAsLastSibling();
            this.currentMenu = targetMenu;

            TimePauser.Paused.Request();
            CharacterInput.Disabled.Request();
            FrigidCoroutine.Kill(this.backdropRoutine);
            this.backdropRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    this.backdropFadeDuration * (this.backdropFadeAlpha - this.backdropImage.color.a),
                    this.backdropImage.color.a,
                    this.backdropFadeAlpha,
                    EasingType.EaseOutSine,
                    useRealTime: true,
                    onValueUpdated: (float alpha) => this.backdropImage.color = new Color(this.backdropImage.color.r, this.backdropImage.color.g, this.backdropImage.color.b, alpha)
                    ), 
                this.gameObject
                );
            this.pointerImage.enabled = true;
            CursorDisplay.Hidden.Request();
        }

        private void CloseMenu(Menu targetMenu)
        {
            targetMenu.Close();
            this.currentMenu = null;

            TimePauser.Paused.Release();
            CharacterInput.Disabled.Release();
            FrigidCoroutine.Kill(this.backdropRoutine);
            this.backdropRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    this.backdropFadeDuration * this.backdropImage.color.a,
                    this.backdropImage.color.a,
                    0,
                    EasingType.EaseOutSine,
                    useRealTime: true,
                    onValueUpdated: (float alpha) => this.backdropImage.color = new Color(this.backdropImage.color.r, this.backdropImage.color.g, this.backdropImage.color.b, alpha)
                    ),
                this.gameObject
                );
            this.pointerImage.enabled = false;
            CursorDisplay.Hidden.Release();
        }
    }
}
