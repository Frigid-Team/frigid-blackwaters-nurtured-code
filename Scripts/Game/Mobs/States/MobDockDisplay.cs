using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDockDisplay : FrigidMonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer mainIconRenderer;
        [SerializeField]
        private List<IconSetting> iconSettings;
        [SerializeField]
        private List<SpriteRenderer> braceRenderers;
        [SerializeField]
        private float fadeDuration;
        [SerializeField]
        private float braceAnimationDistance;
        [SerializeField]
        private float braceAnimationDuration;

        private Mob owner;
        private MobDockState dockState;

        private Vector2[] braceAnimationToPositions;
        private Vector2[] braceAnimationOriginPositions;

        private SpriteRenderer[] allSpriteRenderers;

        private FrigidCoroutine showRoutine;
        private bool currShowAttempt;
        private FrigidCoroutine idleAnimationRoutine;
        private FrigidCoroutine fadeRoutine;

        public void Spawn(Mob owner, MobDockState dockState)
        {
            this.owner = owner;
            this.dockState = dockState;

            this.transform.SetParent(this.owner.TiledArea.ContentsTransform);
            this.owner.OnTiledAreaChanged += 
                (TiledArea previousTiledArea, TiledArea currentTiledArea) =>
                {
                    this.transform.SetParent(this.owner.TiledArea.ContentsTransform);
                    this.Fade(0, 0);
                    this.currShowAttempt = false;
                };

            this.allSpriteRenderers = new SpriteRenderer[1 + this.braceRenderers.Count];
            this.allSpriteRenderers[0] = this.mainIconRenderer;
            for (int i = 0; i < this.braceRenderers.Count; i++) this.allSpriteRenderers[1 + i] = this.braceRenderers[i];

            this.braceAnimationToPositions = new Vector2[this.braceRenderers.Count];
            this.braceAnimationOriginPositions = new Vector2[this.braceRenderers.Count];
            for (int i = 0; i < this.braceRenderers.Count; i++)
            {
                SpriteRenderer braceRenderer = this.braceRenderers[i];
                float braceOrientationAngle = (braceRenderer.transform.rotation.eulerAngles.z + 180) * Mathf.Deg2Rad;
                this.braceAnimationToPositions[i] = (Vector2)braceRenderer.transform.localPosition + new Vector2(Mathf.Cos(braceOrientationAngle), Mathf.Sin(braceOrientationAngle)) * this.braceAnimationDistance;
                this.braceAnimationOriginPositions[i] = braceRenderer.transform.localPosition;
            }

            this.owner.OnShowDisplaysChanged += this.ShowOrHideDisplay;
            this.ShowOrHideDisplay();

            this.owner.OnStatusTagAdded += (MobStatusTag addedStatusTag) => this.UpdateIcon();
            this.owner.OnStatusTagRemoved += (MobStatusTag removedStatusTag) => this.UpdateIcon();
            this.UpdateIcon();

            this.dockState.OnDockStarted += this.OnDockStarted;
            this.dockState.OnDockEnded += this.OnDockEnded;

            this.owner.OnActiveChanged += () => this.gameObject.SetActive(this.owner.Active);
            this.gameObject.SetActive(this.owner.Active);
        }

        private void ShowOrHideDisplay()
        {
            if (this.owner.ShowDisplays) this.Show();
            else this.Hide();
        }

        private void Show()
        {
            this.showRoutine = FrigidCoroutine.Run(this.ShowAtDockPosition(), this.gameObject);
            this.idleAnimationRoutine = FrigidCoroutine.Run(this.AnimateIdleBraces(), this.gameObject);
        }

        private void Hide()
        {
            FrigidCoroutine.Kill(this.showRoutine);
            FrigidCoroutine.Kill(this.idleAnimationRoutine);
            this.Fade(0, this.fadeDuration);
        }

        private void OnDockStarted()
        {
            FrigidCoroutine.Kill(this.idleAnimationRoutine);
            this.mainIconRenderer.enabled = false;
            for (int i = 0; i < this.braceRenderers.Count; i++)
            {
                SpriteRenderer braceRenderer = this.braceRenderers[i];
                FrigidCoroutine.Run(
                    Tween.Value(
                        this.braceAnimationDuration / 2,
                        this.braceAnimationOriginPositions[i],
                        this.braceAnimationToPositions[i] + (this.braceAnimationToPositions[i] - this.braceAnimationOriginPositions[i]) * 0.5f,
                        onValueUpdated: (Vector2 localPosition) => { braceRenderer.transform.localPosition = localPosition; }
                        ),
                    this.gameObject
                    );
            }
        }

        private void OnDockEnded()
        {
            this.mainIconRenderer.enabled = true;
            this.idleAnimationRoutine = FrigidCoroutine.Run(this.AnimateIdleBraces(), this.gameObject);
        }

        private IEnumerator<FrigidCoroutine.Delay> ShowAtDockPosition()
        {
            this.currShowAttempt = false;
            this.Fade(0, 0);
            while (true)
            {
                bool showAttempt = this.dockState.TryGetDockingPosition(out Vector2 dockPosition);
                if (showAttempt)
                {
                    this.transform.position = dockPosition;
                }
                if (this.currShowAttempt != showAttempt)
                {
                    this.currShowAttempt = showAttempt;
                    if (showAttempt) this.Fade(1, this.fadeDuration);
                    else this.Fade(0, this.fadeDuration);
                }
                yield return null;
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> AnimateIdleBraces()
        {
            for (int i = 0; i < this.braceRenderers.Count; i++)
            {
                this.braceRenderers[i].transform.localPosition = this.braceAnimationOriginPositions[i];
            }

            bool animatingToOrigin = false;
            float animationSpeed = this.braceAnimationDistance / this.braceAnimationDuration;
            float currentPeriod = 0;

            while (true)
            {
                for (int i = 0; i < this.braceRenderers.Count; i++)
                {
                    Vector3 toPosition;
                    if (animatingToOrigin)
                    {
                        toPosition = this.braceAnimationOriginPositions[i];
                    }
                    else
                    {
                        toPosition = this.braceAnimationToPositions[i];
                    }
                    this.braceRenderers[i].transform.localPosition += (toPosition - this.braceRenderers[i].transform.localPosition).normalized * animationSpeed * FrigidCoroutine.DeltaTime;
                }
                currentPeriod += FrigidCoroutine.DeltaTime;
                if (currentPeriod > this.braceAnimationDuration)
                {
                    currentPeriod = 0;
                    animatingToOrigin = !animatingToOrigin;
                }
                yield return null;
            }
        }

        private void Fade(float toAlpha, float duration)
        {
            FrigidCoroutine.Kill(this.fadeRoutine);
            float[] origAlphas = new float[this.allSpriteRenderers.Length];
            for (int i = 0; i < this.allSpriteRenderers.Length; i++) origAlphas[i] = this.allSpriteRenderers[i].color.a;
            this.fadeRoutine = FrigidCoroutine.Run(
                Tween.Value(
                    duration,
                    0, 
                    duration,
                    onValueUpdated: (float elapsed) =>
                    { 
                        for (int i = 0; i < this.allSpriteRenderers.Length; i++)
                        {
                            float specificDuration = Mathf.Abs(toAlpha - origAlphas[i]) * duration;
                            Color color = this.allSpriteRenderers[i].color;
                            color.a = origAlphas[i] + (toAlpha - origAlphas[i]) * (specificDuration == 0 ? 1 : Mathf.Clamp01(elapsed / specificDuration));
                            this.allSpriteRenderers[i].color = color;
                        }
                    }
                    ),
                this.gameObject
                );
        }

        private void UpdateIcon()
        {
            foreach (IconSetting iconSetting in this.iconSettings)
            {
                if (this.owner.HasStatusTag(iconSetting.StatusTag))
                {
                    this.mainIconRenderer.sprite = iconSetting.IconSprite;
                    break;
                }
            }
        }

        [Serializable]
        private struct IconSetting
        {
            [SerializeField]
            private MobStatusTag statusTag;
            [SerializeField]
            private Sprite iconSprite;

            public MobStatusTag StatusTag
            {
                get
                {
                    return this.statusTag;
                }
            }

            public Sprite IconSprite
            {
                get
                {
                    return this.iconSprite;
                }
            }
        }
    }
}
