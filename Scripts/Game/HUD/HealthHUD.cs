using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class HealthHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private IntSerializedReference numberTweenFlashes;
        [SerializeField]
        private FloatSerializedReference individualTweenFlashDuration;

        [Header("Health Text")]
        [SerializeField]
        private Text currentHealthText;
        [SerializeField]
        private Outline currentHealthOutline;
        [SerializeField]
        private Text maxHealthText;

        [Header("Health Bar")]
        [SerializeField]
        private Image healthBarImage;
        [SerializeField]
        private Image healthBarBufferImage;
        [SerializeField]
        private ColorSerializedReference bufferFlashFromColor;
        [SerializeField]
        private ColorSerializedReference bufferFlashToColor;
        [SerializeField]
        private ColorSerializedReference bufferNeutralColor;
        [SerializeField]
        private FloatSerializedReference decreaseFillDuration;
        [SerializeField]
        private FloatSerializedReference increaseFillDuration;

        /*
         * TODO MOBS_V2
        private MobHealth_Legacy mobHealth;
        private List<FrigidCoroutine> currentTransitionRoutines;
        private List<FrigidCoroutine> currentHealthChangeRoutines;

        public void TransitionToNewHealth(float transitionDuration, MobHealth_Legacy newMobHealth, float transitionOutPercent01 = 0.5f)
        {
            foreach (FrigidCoroutine transitionRoutine in this.currentTransitionRoutines) FrigidCoroutine.Kill(transitionRoutine);
            this.currentTransitionRoutines.Clear();

            float currentXScale = this.healthBarImage.transform.localScale.x;
            this.currentTransitionRoutines.Add(
                FrigidCoroutine.Run(
                    TweenCoroutine.Value(
                        transitionDuration * transitionOutPercent01,
                        currentXScale,
                        0,
                        EasingType.EaseInOutSine,
                        useRealTime: true,
                        onValueUpdated:
                        (float xScale) =>
                        {
                            Vector3 scale = new Vector3(xScale, 1, 1);
                            this.healthBarImage.transform.localScale = scale;
                            this.healthBarBufferImage.transform.localScale = scale;
                        },
                        onComplete:
                        () =>
                        {
                            UnsubscribeToHealthAssignment();
                            ResetHealthChanges();
                            this.mobHealth = newMobHealth;
                            this.currentHealthText.text = this.mobHealth.CurrentHealth.ToString();
                            this.maxHealthText.text = this.mobHealth.MaxHealth.ToString();
                            this.healthBarImage.fillAmount = (float)this.mobHealth.CurrentHealth / this.mobHealth.MaxHealth;
                            this.healthBarBufferImage.fillAmount = this.healthBarImage.fillAmount;
                            SubscribeToHealthAssignment();

                            this.currentTransitionRoutines.Add(
                                FrigidCoroutine.Run(
                                    TweenCoroutine.Value(
                                        transitionDuration * (1 - transitionOutPercent01),
                                        0,
                                        1,
                                        EasingType.EaseInOutSine,
                                        useRealTime: true,
                                        onValueUpdated:
                                        (float xScale) =>
                                        {
                                            Vector3 scale = new Vector3(xScale, 1, 1);
                                            this.healthBarImage.transform.localScale = scale;
                                            this.healthBarBufferImage.transform.localScale = scale;
                                        }
                                        ),
                                    this.gameObject
                                    )
                                );
                        }
                        ),
                    this.gameObject
                    )
                );
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentTransitionRoutines = new List<FrigidCoroutine>();
            this.currentHealthChangeRoutines = new List<FrigidCoroutine>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SubscribeToHealthAssignment();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeToHealthAssignment();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void SubscribeToHealthAssignment()
        {
            if (this.mobHealth == null) return;

            this.mobHealth.OnCurrentHealthChanged += ShowCurrentHealthChangeOnBar;
            this.mobHealth.OnMaxHealthChanged += ShowMaxHealthChangeOnBar;
        }

        private void UnsubscribeToHealthAssignment()
        {
            if (this.mobHealth == null) return;

            this.mobHealth.OnCurrentHealthChanged -= ShowCurrentHealthChangeOnBar;
            this.mobHealth.OnMaxHealthChanged -= ShowMaxHealthChangeOnBar;
        }

        private void ShowCurrentHealthChangeOnBar(int previousCurrentHealth, int currentHealth)
        {
            ResetHealthChanges();
            this.currentHealthText.text = this.mobHealth.CurrentHealth.ToString();
            float newFill = (float)this.mobHealth.CurrentHealth / this.mobHealth.MaxHealth;

            if (currentHealth > previousCurrentHealth)
            {
                this.healthBarBufferImage.fillAmount = newFill;
                this.healthBarBufferImage.color = this.bufferNeutralColor.ImmutableValue;
                this.currentHealthChangeRoutines.Add(
                    FrigidCoroutine.Run(
                        TweenCoroutine.Value(
                            this.decreaseFillDuration.ImmutableValue,
                            this.healthBarImage.fillAmount,
                            newFill,
                            EasingType.EaseOutSine,
                            useRealTime: true,
                            onValueUpdated: (float fill) => { this.healthBarImage.fillAmount = fill; }
                            ),
                        this.gameObject
                        )
                    );
            }
            else
            {
                this.currentHealthChangeRoutines.Add(
                    FrigidCoroutine.Run(
                        TweenCoroutine.Value(
                            this.individualTweenFlashDuration.ImmutableValue,
                            0,
                            1,
                            EasingType.EaseInOutSine,
                            numberIterations:
                            this.numberTweenFlashes.ImmutableValue,
                            pingPong: true,
                            useRealTime: true,
                            onValueUpdated: (float alpha) => { this.currentHealthOutline.effectColor = new Color(this.currentHealthOutline.effectColor.r, this.currentHealthOutline.effectColor.g, this.currentHealthOutline.effectColor.b, alpha); }
                            ),
                        this.gameObject
                        )
                    );

                this.healthBarBufferImage.fillAmount = this.healthBarImage.fillAmount;
                this.healthBarImage.fillAmount = newFill;

                this.currentHealthChangeRoutines.Add(
                    FrigidCoroutine.Run(
                        TweenCoroutine.Value(
                            this.individualTweenFlashDuration.ImmutableValue,
                            this.bufferFlashFromColor.ImmutableValue,
                            this.bufferFlashToColor.ImmutableValue,
                            EasingType.EaseInOutSine,
                            numberIterations: this.numberTweenFlashes.ImmutableValue,
                            pingPong: true,
                            useRealTime: true,
                            onValueUpdated: (Color color) => { this.healthBarBufferImage.color = color; },
                            onComplete:
                            () =>
                            {
                                this.currentHealthChangeRoutines.Add(
                                    FrigidCoroutine.Run(
                                        TweenCoroutine.Value(
                                            this.decreaseFillDuration.ImmutableValue,
                                            this.healthBarBufferImage.fillAmount,
                                            newFill,
                                            EasingType.EaseOutSine,
                                            useRealTime: true,
                                            onValueUpdated: (float fill) => { this.healthBarBufferImage.fillAmount = fill; }
                                            ),
                                        this.gameObject
                                        )
                                    );
                            }
                            ),
                        this.gameObject
                        )
                    );
            }
        }

        private void ShowMaxHealthChangeOnBar(int previousMaxHealth, int maxHealth)
        {
            ResetHealthChanges();
            this.maxHealthText.text = this.mobHealth.MaxHealth.ToString();
            float newFill = (float)this.mobHealth.CurrentHealth / this.mobHealth.MaxHealth;
            float fillDuration = maxHealth < previousMaxHealth ? this.increaseFillDuration.ImmutableValue : this.decreaseFillDuration.ImmutableValue;
            this.currentHealthChangeRoutines.Add(
                FrigidCoroutine.Run(
                    TweenCoroutine.Value(
                        fillDuration,
                        this.healthBarBufferImage.fillAmount,
                        newFill,
                        EasingType.EaseOutSine,
                        useRealTime: true,
                        onValueUpdated: (float fill) => { this.healthBarBufferImage.fillAmount = fill; }
                        ),
                    this.gameObject
                    )
                );
            this.currentHealthChangeRoutines.Add(
                FrigidCoroutine.Run(
                    TweenCoroutine.Value(
                        fillDuration,
                        this.healthBarImage.fillAmount,
                        newFill,
                        EasingType.EaseOutSine,
                        useRealTime: true,
                        onValueUpdated: (float fill) => { this.healthBarImage.fillAmount = fill; }
                        ),
                    this.gameObject
                    )
                );
        }

        private void ResetHealthChanges()
        {
            foreach (FrigidCoroutine healthChangeRoutine in this.currentHealthChangeRoutines) FrigidCoroutine.Kill(healthChangeRoutine);
            this.currentHealthChangeRoutines.Clear();
            this.currentHealthOutline.effectColor = new Color(this.currentHealthOutline.effectColor.r, this.currentHealthOutline.effectColor.g, this.currentHealthOutline.effectColor.b, 0);
            this.healthBarBufferImage.color = this.bufferFlashFromColor.ImmutableValue;
        }
        */
    }
}
