using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledAreaTransitioner : FrigidMonoBehaviour
    {
        private const string CLOSE_ANIMATION_NAME = "Close";
        private const string OPEN_ANIMATION_NAME = "Open";
        private const string SPEED_PARAMETER_NAME = "TransitionSpeed";

        [SerializeField]
        private SpriteRenderer backgroundRenderer;
        [SerializeField]
        private SpriteMask focusMask;
        [SerializeField]
        private Animator focusAnimator;
        [SerializeField]
        private SpriteRenderer focusSpriteRenderer;
        [SerializeField]
        private AudioSource transitionAudioSource;

        [Header("Timing")]
        [SerializeField]
        private float centerDurationPercent;
        [SerializeField]
        private float zoomedDurationPercent;
        [SerializeField]
        private float animationDurationPercent;

        public void SetDimensions(Vector2Int dimensions)
        {
            this.backgroundRenderer.transform.localScale = (Vector2)dimensions * 2 * FrigidConstants.UNIT_WORLD_SIZE;
        }

        public void PlayTransitionTo(TiledAreaTransition transition, float duration, Vector2 entryPosition)
        {
            Bounds cameraBounds = MainCamera.Instance.WorldBounds;
            float focusMaskMaxScale = Mathf.Max(cameraBounds.extents.x, cameraBounds.extents.y);

            this.backgroundRenderer.enabled = true;

            this.focusSpriteRenderer.enabled = true;
            this.focusSpriteRenderer.transform.position = entryPosition;
            this.focusSpriteRenderer.sprite = transition.CloseSprite;

            this.focusMask.enabled = true;
            this.focusMask.transform.position = entryPosition;
            this.focusMask.transform.localScale = Vector2.zero;

            float zoomedDuration = duration * this.zoomedDurationPercent;
            FrigidCoroutine.Run(
                Tween.Value(
                    zoomedDuration, 
                    this.focusMask.transform.localScale,
                    Vector2.one,
                    EasingType.EaseInCirc, 
                    onValueUpdated: (Vector2 scale) => this.focusMask.transform.localScale = scale,
                    onComplete:
                    () =>
                    {
                        float animationDuration = duration * this.animationDurationPercent;
                        this.focusAnimator.runtimeAnimatorController = transition.AnimatorController;
                        this.focusAnimator.enabled = true;
                        this.focusAnimator.SetFloat(SPEED_PARAMETER_NAME, transition.BaseDuration / animationDuration);
                        this.focusAnimator.Play(Animator.StringToHash(OPEN_ANIMATION_NAME));
                        FrigidCoroutine.Run(
                            Tween.Delay(
                                animationDuration, 
                                () => 
                                {
                                    float centerDuration = duration * this.centerDurationPercent;
                                    this.focusAnimator.enabled = false;
                                    this.focusSpriteRenderer.enabled = false;
                                    FrigidCoroutine.Run(
                                        Tween.Value(
                                            centerDuration,
                                            this.focusMask.transform.position,
                                            cameraBounds.center,
                                            EasingType.EaseInCirc,
                                            onValueUpdated: (Vector2 position) => this.focusMask.transform.position = position,
                                            onComplete: () =>
                                            {
                                                this.focusMask.enabled = false;
                                                this.backgroundRenderer.enabled = false;
                                            }
                                            ),
                                        this.gameObject
                                        );
                                    FrigidCoroutine.Run(
                                        Tween.Value(
                                            centerDuration,
                                            this.focusMask.transform.localScale,
                                            new Vector2(focusMaskMaxScale, focusMaskMaxScale) * FrigidConstants.UNIT_WORLD_SIZE,
                                            EasingType.EaseInCirc,
                                            onValueUpdated: (Vector2 scale) => this.focusMask.transform.localScale = scale
                                            ),
                                        this.gameObject
                                        );
                                }
                                ),
                            this.gameObject
                            );
                    }
                    ), 
                this.gameObject
                );
            this.transitionAudioSource.clip = transition.TransitionToAudioClip;
            this.transitionAudioSource.Play();
        }

        public void PlayTransitionAway(TiledAreaTransition transition, float duration, Vector2 exitPosition)
        {
            Bounds cameraBounds = MainCamera.Instance.WorldBounds;
            float focusMaskMaxScale = Mathf.Max(cameraBounds.extents.x, cameraBounds.extents.y);

            this.backgroundRenderer.enabled = true;

            this.focusSpriteRenderer.transform.position = exitPosition;
            this.focusSpriteRenderer.sprite = transition.OpenSprite;

            this.focusMask.enabled = true;
            this.focusMask.transform.position = cameraBounds.center;
            this.focusMask.transform.localScale = new Vector2(focusMaskMaxScale, focusMaskMaxScale) * FrigidConstants.UNIT_WORLD_SIZE;

            float centerDuration = duration * this.centerDurationPercent;
            FrigidCoroutine.Run(
                Tween.Value(
                    centerDuration, 
                    this.focusMask.transform.localScale, 
                    Vector2.one,
                    EasingType.EaseOutCirc,
                    onValueUpdated: (Vector2 scale) => this.focusMask.transform.localScale = scale,
                    onComplete:
                    () =>
                    {
                        float animationDuration = duration * this.animationDurationPercent;
                        this.focusSpriteRenderer.enabled = true;
                        this.focusAnimator.runtimeAnimatorController = transition.AnimatorController;
                        this.focusAnimator.enabled = true;
                        this.focusAnimator.SetFloat(SPEED_PARAMETER_NAME, transition.BaseDuration / animationDuration);
                        this.focusAnimator.Play(Animator.StringToHash(CLOSE_ANIMATION_NAME));
                        FrigidCoroutine.Run(
                            Tween.Delay(
                                animationDuration,
                                () =>
                                {
                                    float zoomedDuration = duration * this.zoomedDurationPercent;
                                    FrigidCoroutine.Run(
                                        Tween.Value(
                                            zoomedDuration,
                                            this.focusMask.transform.localScale,
                                            Vector2.zero,
                                            EasingType.EaseOutCirc,
                                            onValueUpdated: (Vector2 scale) => this.focusMask.transform.localScale = scale,
                                            onComplete:
                                            () =>
                                            {
                                                this.focusMask.enabled = false;
                                                this.backgroundRenderer.enabled = false;
                                                this.focusAnimator.enabled = false;
                                                this.focusSpriteRenderer.enabled = false;
                                            }
                                            ),
                                        this.gameObject
                                        );
                                }
                                ),
                            this.gameObject
                            );
                    }
                    ), 
                this.gameObject
                );
            FrigidCoroutine.Run(
                Tween.Value(
                    centerDuration, 
                    this.focusMask.transform.position,
                    exitPosition, 
                    EasingType.EaseOutCirc,
                    onValueUpdated: (Vector2 position) => this.focusMask.transform.position = position),
                this.gameObject
                );
            this.transitionAudioSource.clip = transition.TransitionAwayAudioClip;
            this.transitionAudioSource.Play();
        }

        protected override void Awake()
        {
            base.Awake();
            this.focusMask.enabled = false;
            this.backgroundRenderer.enabled = false;
            this.focusSpriteRenderer.enabled = false;
            this.focusAnimator.enabled = false;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
