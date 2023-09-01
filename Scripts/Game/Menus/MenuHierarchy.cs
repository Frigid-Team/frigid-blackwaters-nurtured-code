using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MenuHierarchy : FrigidMonoBehaviourWithUpdate
    {
        [Header("Menus")]
        [SerializeField]
        private List<Menu> menus;

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
            if (this.currentMenu != null)
            {
                CharacterInput.Disabled.Request();
                TimePauser.Paused.Request();
            }
            LoadingOverlay.OnLoadStart += this.CloseCurrentMenu;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.currentMenu != null)
            {
                CharacterInput.Disabled.Release();
                TimePauser.Paused.Release();
            }
            LoadingOverlay.OnLoadStart -= this.CloseCurrentMenu;
        }

        protected override void Update()
        {
            base.Update();
            this.pointerImage.transform.position = InterfaceInput.PointPosition;
            if (this.currentMenu == null)
            {
                foreach (Menu menu in this.menus)
                {
                    if (menu.WantsToOpen())
                    {
                        this.OpenMenu(menu);
                        return;
                    }
                }
            }
            else
            {
                if (this.currentMenu.WantsToClose())
                {
                    this.CloseCurrentMenu();
                }
            }
        }

        private void OpenMenu(Menu menu)
        {
            if (menu == null) return;
            this.CloseCurrentMenu();

            menu.Opened();
            menu.transform.SetAsLastSibling();
            this.currentMenu = menu;

            TimePauser.Paused.Request();
            CharacterInput.Disabled.Request();
            FrigidCoroutine.Kill(this.backdropRoutine);
            this.backdropRoutine = FrigidCoroutine.Run(
                Tween.Value(
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

        private void CloseCurrentMenu()
        {
            if (this.currentMenu == null) return;

            this.currentMenu.Closed();
            this.currentMenu = null;

            TimePauser.Paused.Release();
            CharacterInput.Disabled.Release();
            FrigidCoroutine.Kill(this.backdropRoutine);
            this.backdropRoutine = FrigidCoroutine.Run(
                Tween.Value(
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
