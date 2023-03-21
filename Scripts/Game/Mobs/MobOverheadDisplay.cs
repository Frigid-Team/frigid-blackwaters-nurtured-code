using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobOverheadDisplay : FrigidMonoBehaviour
    {
        [SerializeField]
        private float heightBuffer;
        [SerializeField]
        private float slideDuration;
        [SerializeField]
        private List<AlignmentColoring> alignmentColorings;
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
        private SpriteRenderer stunBorderSpriteRenderer;
        [SerializeField]
        private float fadeDuration;

        private Mob owner;

        private FrigidCoroutine barRoutine;
        private FrigidCoroutine fadeRoutine;
        
        public void Spawn(Mob owner)
        {
            this.owner = owner;

            foreach (AlignmentColoring groupColoring in this.alignmentColorings)
            {
                if (groupColoring.Alignment == this.owner.Alignment)
                {
                    this.frontBarSpriteRenderer.color = groupColoring.FrontBarColor;
                    this.backBarSpriteRenderer.color = groupColoring.BackBarColor;
                    break;
                }
            }

            this.owner.OnTiledAreaChanged +=
                (TiledArea previousTiledArea, TiledArea currentTiledArea) =>
                {
                    this.transform.SetParent(this.owner.TiledArea.ContentsTransform);
                    this.transform.position = this.owner.DisplayPosition + Vector2.up * (this.owner.Height + this.heightBuffer);
                };
            this.transform.SetParent(this.owner.TiledArea.ContentsTransform);
            FrigidCoroutine.Run(FollowOwner(), this.gameObject);

            FadeDisplayAlpha(0, 0);

            this.owner.OnShowDisplaysChanged += ShowOrHideDisplay;
            ShowOrHideDisplay();
            this.owner.OnActiveChanged += () => { this.gameObject.SetActive(this.owner.Active); };
            this.gameObject.SetActive(this.owner.Active);
            this.owner.OnStunnedChanged += () => { this.stunBorderSpriteRenderer.enabled = this.owner.Stunned; };
            this.stunBorderSpriteRenderer.enabled = this.owner.Stunned;
        }

        private void ShowOrHideDisplay()
        {
            if (this.owner.ShowDisplays) Show();
            else Hide();
        }

        private void Show()
        {
            FrigidCoroutine.Kill(this.fadeRoutine);
            FrigidCoroutine.Kill(this.barRoutine);

            SetBarSize();
            SetBarFill(this.frontBarSpriteRenderer, (float)this.owner.RemainingHealth / this.owner.MaxHealth);
            SetBarFill(this.backBarSpriteRenderer, (float)this.owner.RemainingHealth / this.owner.MaxHealth);

            this.owner.OnRemainingHealthChanged += ShowRemainingHealthChange;
            this.owner.OnMaxHealthChanged += ShowMaxHealthChange;
            FadeDisplayAlpha(1, this.fadeDuration);
        }

        private void Hide()
        {
            FrigidCoroutine.Kill(this.fadeRoutine);
            FrigidCoroutine.Kill(this.barRoutine);

            this.owner.OnRemainingHealthChanged -= ShowRemainingHealthChange;
            this.owner.OnMaxHealthChanged -= ShowMaxHealthChange;
            FadeDisplayAlpha(0, this.fadeDuration);
        }

        private IEnumerator<FrigidCoroutine.Delay> FollowOwner()
        {
            while (true)
            {
                this.transform.position = this.owner.DisplayPosition + Vector2.up * (this.owner.Height + this.heightBuffer);
                yield return null;
            }
        }

        private void SetBarSize()
        {
            float borderXDiff = this.stunBorderSpriteRenderer.size.x - this.outlineSpriteRenderer.size.x;
            float xSize = this.baseWidth * Mathf.Log(this.owner.MaxHealth) / Mathf.Log(this.widthScalingHealth);
            this.outlineSpriteRenderer.size = new Vector2(xSize, this.outlineSpriteRenderer.size.y);
            this.stunBorderSpriteRenderer.size = new Vector2(xSize + borderXDiff, this.stunBorderSpriteRenderer.size.y);
            Vector2 newBarLocalPosition = new Vector2(-this.outlineSpriteRenderer.size.x / 2, 0);
            this.frontBarSpriteRenderer.transform.localPosition = newBarLocalPosition;
            this.backBarSpriteRenderer.transform.localPosition = newBarLocalPosition;
            SetBarFill(this.frontBarSpriteRenderer, GetBarFill(this.frontBarSpriteRenderer));
            SetBarFill(this.backBarSpriteRenderer, GetBarFill(this.backBarSpriteRenderer));
        }

        private void ShowRemainingHealthChange(int previousCurrentHealth, int currentHealth)
        {
            FrigidCoroutine.Kill(this.barRoutine);
            float targetFill = (float)this.owner.RemainingHealth / this.owner.MaxHealth;
            SetBarFill(this.frontBarSpriteRenderer, targetFill);
            this.barRoutine = FrigidCoroutine.Run(
                TweenCoroutine.DelayedCall(
                    this.decreaseWaitDuration,
                    () =>
                    {
                        this.barRoutine = FrigidCoroutine.Run(
                            TweenCoroutine.Value(
                                this.decreaseFillDuration,
                                GetBarFill(this.backBarSpriteRenderer),
                                targetFill,
                                onValueUpdated: (float progress01) => SetBarFill(this.backBarSpriteRenderer, progress01)
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
            SetBarSize();
            FrigidCoroutine.Kill(this.barRoutine);
            float targetFill = (float)this.owner.RemainingHealth / this.owner.MaxHealth;
            SetBarFill(this.frontBarSpriteRenderer, targetFill);
            SetBarFill(this.backBarSpriteRenderer, targetFill);
        }

        private void FadeDisplayAlpha(float toAlpha, float duration)
        {
            FrigidCoroutine.Kill(this.fadeRoutine);
            float origOutlineAlpha = this.outlineSpriteRenderer.color.a;
            float origAccentsAlpha = this.accentsSpriteRenderer.color.a;
            float origFrontBarAlpha = this.frontBarSpriteRenderer.color.a;
            float origBackBarAlpha = this.backBarSpriteRenderer.color.a;
            float stunIconAlpha = this.stunBorderSpriteRenderer.color.a;
            this.fadeRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    duration,
                    0f,
                    1f,
                    onValueUpdated: (float progress) =>
                    {
                        this.outlineSpriteRenderer.color = new Color(this.outlineSpriteRenderer.color.r, this.outlineSpriteRenderer.color.g, this.outlineSpriteRenderer.color.b, origOutlineAlpha + (toAlpha - origOutlineAlpha) * progress);
                        this.accentsSpriteRenderer.color = new Color(this.accentsSpriteRenderer.color.r, this.accentsSpriteRenderer.color.g, this.accentsSpriteRenderer.color.b, origAccentsAlpha + (toAlpha - origAccentsAlpha) * progress);
                        this.frontBarSpriteRenderer.color = new Color(this.frontBarSpriteRenderer.color.r, this.frontBarSpriteRenderer.color.g, this.frontBarSpriteRenderer.color.b, origFrontBarAlpha + (toAlpha - origFrontBarAlpha) * progress);
                        this.backBarSpriteRenderer.color = new Color(this.backBarSpriteRenderer.color.r, this.backBarSpriteRenderer.color.g, this.backBarSpriteRenderer.color.b, origBackBarAlpha + (toAlpha - origBackBarAlpha) * progress);
                        this.stunBorderSpriteRenderer.color = new Color(this.stunBorderSpriteRenderer.color.r, this.stunBorderSpriteRenderer.color.g, this.stunBorderSpriteRenderer.color.b, stunIconAlpha + (toAlpha - stunIconAlpha) * progress);
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
        public struct AlignmentColoring
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
