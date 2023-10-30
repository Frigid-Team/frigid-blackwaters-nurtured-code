using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobOverheadDisplay : FrigidMonoBehaviour
    {
        private static SceneVariable<Dictionary<MobOverheadPopup, RecyclePool<MobOverheadPopup>>> popupPools;

        [SerializeField]
        private MobOverheadPopup popupPrefab;
        [SerializeField]
        private Transform popupsParent;
        [SerializeField]
        private float heightBuffer;
        [SerializeField]
        private float slideDuration;
        [SerializeField]
        private List<BarAlignmentSetting> barAlignmentSettings;
        [SerializeField]
        private SpriteRenderer outlineSpriteRenderer;
        [SerializeField]
        private SpriteRenderer accentsSpriteRenderer;
        [SerializeField]
        private SpriteRenderer frontBarSpriteRenderer;
        [SerializeField]
        private SpriteRenderer backBarSpriteRenderer;
        [SerializeField]
        private float baseWidth;
        [SerializeField]
        private float widthScalingHealth;
        [SerializeField]
        private float decreaseWaitDuration;
        [SerializeField]
        private float decreaseFillDuration;
        [SerializeField]
        private float increaseFillDuration;
        [SerializeField]
        private SpriteRenderer stunIconSpriteRenderer;
        [SerializeField]
        private float fadeDuration;

        private Mob owner;

        private FrigidCoroutine barRoutine;
        private FrigidCoroutine fadeRoutine;

        static MobOverheadDisplay()
        {
            popupPools = new SceneVariable<Dictionary<MobOverheadPopup, RecyclePool<MobOverheadPopup>>>(() => new Dictionary<MobOverheadPopup, RecyclePool<MobOverheadPopup>>());
        }
        
        public void Spawn(Mob owner)
        {
            this.owner = owner;

            if (!popupPools.Current.ContainsKey(this.popupPrefab))
            {
                popupPools.Current.Add(this.popupPrefab, new RecyclePool<MobOverheadPopup>(() => CreateInstance<MobOverheadPopup>(this.popupPrefab), (MobOverheadPopup popup) => DestroyInstance(popup)));
            }

            this.frontBarSpriteRenderer.color = Color.clear;
            this.backBarSpriteRenderer.color = Color.clear;
            foreach (BarAlignmentSetting barAlignmentSetting in this.barAlignmentSettings)
            {
                if (barAlignmentSetting.Alignment == this.owner.Alignment)
                {
                    this.frontBarSpriteRenderer.color = barAlignmentSetting.FrontBarColor;
                    this.backBarSpriteRenderer.color = barAlignmentSetting.BackBarColor;
                    break;
                }
            }

            this.owner.OnTiledAreaChanged +=
                (TiledArea previousTiledArea, TiledArea currentTiledArea) =>
                {
                    this.transform.SetParent(this.owner.TiledArea.ContentsTransform);
                    this.transform.position = this.owner.Position + Vector2.up * (this.owner.Height + this.heightBuffer);
                };
            this.transform.SetParent(this.owner.TiledArea.ContentsTransform);
            FrigidCoroutine.Run(this.FollowOwner(), this.gameObject);

            this.FadeDisplayAlpha(0, 0);

            this.owner.OnShowDisplaysChanged += this.ShowOrHideDisplay;
            this.ShowOrHideDisplay();
            this.owner.OnActiveChanged += () => { this.gameObject.SetActive(this.owner.Active); };
            this.gameObject.SetActive(this.owner.Active);
            this.owner.OnStunnedChanged += () => { this.stunIconSpriteRenderer.enabled = this.owner.Stunned; };
            this.stunIconSpriteRenderer.enabled = this.owner.Stunned;
            this.owner.OnHealed +=
                (int heal) =>
                {
                    if (!this.owner.ShowDisplays) return;
                    MobOverheadPopup overheadPopup = popupPools.Current[this.popupPrefab].Retrieve();
                    overheadPopup.transform.SetParent(this.popupsParent);
                    overheadPopup.ShowHeal(heal, () => popupPools.Current[this.popupPrefab].Return(overheadPopup));
                };
            this.owner.OnDamaged +=
                (int damage) =>
                {
                    if (!this.owner.ShowDisplays) return;
                    MobOverheadPopup overheadPopup = popupPools.Current[this.popupPrefab].Retrieve();
                    overheadPopup.transform.SetParent(this.popupsParent);
                    overheadPopup.ShowDamage(damage, () => popupPools.Current[this.popupPrefab].Return(overheadPopup));
                };
        }

        private void ShowOrHideDisplay()
        {
            if (this.owner.ShowDisplays) this.Show();
            else this.Hide();
        }

        private void Show()
        {
            FrigidCoroutine.Kill(this.fadeRoutine);
            FrigidCoroutine.Kill(this.barRoutine);

            this.SetBarSize();
            this.SetBarFill(this.frontBarSpriteRenderer, (float)this.owner.RemainingHealth / this.owner.MaxHealth);
            this.SetBarFill(this.backBarSpriteRenderer, (float)this.owner.RemainingHealth / this.owner.MaxHealth);

            this.owner.OnRemainingHealthChanged += this.ShowRemainingHealthChange;
            this.owner.OnMaxHealthChanged += this.ShowMaxHealthChange;
            this.FadeDisplayAlpha(1, this.fadeDuration);
        }

        private void Hide()
        {
            FrigidCoroutine.Kill(this.fadeRoutine);
            FrigidCoroutine.Kill(this.barRoutine);

            this.owner.OnRemainingHealthChanged -= this.ShowRemainingHealthChange;
            this.owner.OnMaxHealthChanged -= this.ShowMaxHealthChange;
            this.FadeDisplayAlpha(0, this.fadeDuration);
        }

        private IEnumerator<FrigidCoroutine.Delay> FollowOwner()
        {
            while (true)
            {
                this.transform.position = this.owner.Position + Vector2.up * (this.owner.Height + this.heightBuffer);
                yield return null;
            }
        }

        private void SetBarSize()
        {
            float borderXDiff = this.stunIconSpriteRenderer.size.x - this.outlineSpriteRenderer.size.x;
            float xSize = this.baseWidth * Mathf.Log(Mathf.Max((float)Math.E, this.owner.MaxHealth)) / Mathf.Log(this.widthScalingHealth);
            this.outlineSpriteRenderer.size = new Vector2(xSize, this.outlineSpriteRenderer.size.y);
            this.stunIconSpriteRenderer.size = new Vector2(xSize + borderXDiff, this.stunIconSpriteRenderer.size.y);
            Vector2 newBarLocalPosition = new Vector2(-this.outlineSpriteRenderer.size.x / 2, 0);
            this.frontBarSpriteRenderer.transform.localPosition = newBarLocalPosition;
            this.backBarSpriteRenderer.transform.localPosition = newBarLocalPosition;
            this.SetBarFill(this.frontBarSpriteRenderer, this.GetBarFill(this.frontBarSpriteRenderer));
            this.SetBarFill(this.backBarSpriteRenderer, this.GetBarFill(this.backBarSpriteRenderer));
        }

        private void ShowRemainingHealthChange(int previousCurrentHealth, int currentHealth)
        {
            FrigidCoroutine.Kill(this.barRoutine);
            float targetFill = (float)this.owner.RemainingHealth / this.owner.MaxHealth;
            this.SetBarFill(this.frontBarSpriteRenderer, targetFill);
            this.barRoutine = FrigidCoroutine.Run(
                Tween.Delay(
                    this.decreaseWaitDuration,
                    () =>
                    {
                        this.barRoutine = FrigidCoroutine.Run(
                            Tween.Value(
                                this.decreaseFillDuration,
                                this.GetBarFill(this.backBarSpriteRenderer),
                                targetFill,
                                onValueUpdated: (float progress01) => this.SetBarFill(this.backBarSpriteRenderer, progress01)
                                ),
                            this.gameObject
                            );
                    }
                    ),
                this.gameObject
                );
        }

        private void ShowMaxHealthChange(int previousMaxHealth, int maxHealth)
        {
            this.SetBarSize();
            FrigidCoroutine.Kill(this.barRoutine);
            float targetFill = (float)this.owner.RemainingHealth / this.owner.MaxHealth;
            this.SetBarFill(this.frontBarSpriteRenderer, targetFill);
            this.SetBarFill(this.backBarSpriteRenderer, targetFill);
        }

        private void FadeDisplayAlpha(float toAlpha, float duration)
        {
            FrigidCoroutine.Kill(this.fadeRoutine);
            float origOutlineAlpha = this.outlineSpriteRenderer.color.a;
            float origAccentsAlpha = this.accentsSpriteRenderer.color.a;
            float origFrontBarAlpha = this.frontBarSpriteRenderer.color.a;
            float origBackBarAlpha = this.backBarSpriteRenderer.color.a;
            float stunIconAlpha = this.stunIconSpriteRenderer.color.a;
            this.fadeRoutine = FrigidCoroutine.Run(
                Tween.Value(
                    duration,
                    0f,
                    1f,
                    onValueUpdated: (float progress) =>
                    {
                        this.outlineSpriteRenderer.color = new Color(this.outlineSpriteRenderer.color.r, this.outlineSpriteRenderer.color.g, this.outlineSpriteRenderer.color.b, origOutlineAlpha + (toAlpha - origOutlineAlpha) * progress);
                        this.accentsSpriteRenderer.color = new Color(this.accentsSpriteRenderer.color.r, this.accentsSpriteRenderer.color.g, this.accentsSpriteRenderer.color.b, origAccentsAlpha + (toAlpha - origAccentsAlpha) * progress);
                        this.frontBarSpriteRenderer.color = new Color(this.frontBarSpriteRenderer.color.r, this.frontBarSpriteRenderer.color.g, this.frontBarSpriteRenderer.color.b, origFrontBarAlpha + (toAlpha - origFrontBarAlpha) * progress);
                        this.backBarSpriteRenderer.color = new Color(this.backBarSpriteRenderer.color.r, this.backBarSpriteRenderer.color.g, this.backBarSpriteRenderer.color.b, origBackBarAlpha + (toAlpha - origBackBarAlpha) * progress);
                        this.stunIconSpriteRenderer.color = new Color(this.stunIconSpriteRenderer.color.r, this.stunIconSpriteRenderer.color.g, this.stunIconSpriteRenderer.color.b, stunIconAlpha + (toAlpha - stunIconAlpha) * progress);
                    }
                    ),
                this.gameObject
                );
        }

        private float GetBarFill(SpriteRenderer barSpriteRenderer)
        {
            return barSpriteRenderer.transform.localScale.x / (this.outlineSpriteRenderer.size.x / this.baseWidth);
        }

        private void SetBarFill(SpriteRenderer barSpriteRenderer, float progress01)
        {
            barSpriteRenderer.transform.localScale = new Vector2(this.outlineSpriteRenderer.size.x / this.baseWidth * progress01, 1);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        [Serializable]
        private struct BarAlignmentSetting
        {
            [SerializeField]
            private DamageAlignment alignment;
            [SerializeField]
            private ColorSerializedReference frontBarColor;
            [SerializeField]
            private ColorSerializedReference backBarColor;

            public DamageAlignment Alignment
            {
                get
                {
                    return this.alignment;
                }
            }

            public Color FrontBarColor
            {
                get
                {
                    return this.frontBarColor.ImmutableValue;
                }
            }

            public Color BackBarColor
            {
                get
                {
                    return this.backBarColor.ImmutableValue;
                }
            }
        }
    }
}
