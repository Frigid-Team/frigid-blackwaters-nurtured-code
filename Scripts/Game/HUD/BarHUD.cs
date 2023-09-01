using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class BarHUD : FrigidMonoBehaviour
    {
        [Header("Transitions")]
        [SerializeField]
        private FloatSerializedReference transitionDuration;

        [Header("Flashes")]
        [SerializeField]
        private IntSerializedReference numberFlashes;
        [SerializeField]
        private FloatSerializedReference individualTweenFlashDuration;

        [Header("Text")]
        [SerializeField]
        private bool showText;
        [SerializeField]
        [ShowIfBool("showText", true)]
        private Text currentText;
        [SerializeField]
        [ShowIfBool("showText", true)]
        private bool showCurrentTextFlash;
        [SerializeField]
        [ShowIfBool("showText", true)]
        [ShowIfBool("showCurrentTextFlash", true)]
        private Outline currentTextOutline;
        [SerializeField]
        [ShowIfBool("showText", true)]
        private Text maxText;
        [SerializeField]
        [ShowIfBool("showText", true)]
        private bool showMaxTextFlash;
        [SerializeField]
        [ShowIfBool("showText", true)]
        [ShowIfBool("showMaxTextFlash", true)]
        private Outline maxTextOutline;

        [Header("Main Bar")]
        [SerializeField]
        private Image mainBarImage;
        [SerializeField]
        private Image[] mainBarAccentImages;
        [SerializeField]
        private bool flashMainBar;
        [SerializeField]
        [ShowIfBool("flashMainBar", true)]
        private ColorSerializedReference mainBarFlashColor;

        [Header("Buffer Bar")]
        [SerializeField]
        private Image bufferBarImage;
        [SerializeField]
        private Image[] bufferBarAccentImages;
        [SerializeField]
        private bool flashBufferBar;
        [SerializeField]
        [ShowIfBool("flashBufferBar", true)]
        private ColorSerializedReference bufferBarFlashColor;

        [Header("Durations")]
        [SerializeField]
        private FloatSerializedReference decreaseFillDuration;
        [SerializeField]
        private FloatSerializedReference increaseFillDuration;

        private Color mainBarColor;
        private Color bufferBarColor;

        private int current;
        private int maximum;

        private List<FrigidCoroutine> transitionRoutines;
        private List<FrigidCoroutine> changeRoutines;

        public Color MainBarColor
        {
            get
            {
                return this.mainBarColor;
            }
            set
            {
                if (this.mainBarColor != value)
                {
                    this.StopTweensAndFlashes();
                    this.mainBarColor = value;
                    this.mainBarImage.color = this.mainBarColor;
                    foreach (Image mainBarAccentImage in this.mainBarAccentImages) mainBarAccentImage.color = this.mainBarColor;
                }
            }
        }

        public Color BufferBarColor
        {
            get
            {
                return this.bufferBarColor;
            }
            set
            {
                if (this.bufferBarColor != value)
                {
                    this.StopTweensAndFlashes();
                    this.bufferBarColor = value;
                    this.bufferBarImage.color = this.bufferBarColor;
                    foreach (Image bufferBarAccentImage in this.bufferBarAccentImages) bufferBarAccentImage.color = this.bufferBarColor;
                }
            }
        }

        public void Transition(int current, int maximum, float transitionRatio = 0.5f)
        {
            foreach (FrigidCoroutine transitionRoutine in this.transitionRoutines) FrigidCoroutine.Kill(transitionRoutine);
            this.transitionRoutines.Clear();

            float currentXScale = this.mainBarImage.transform.localScale.x;
            this.transitionRoutines.Add(
                FrigidCoroutine.Run(
                    Tween.Value(
                        this.transitionDuration.ImmutableValue * transitionRatio,
                        currentXScale,
                        0,
                        EasingType.EaseInOutSine,
                        useRealTime: true,
                        onValueUpdated:
                        (float xScale) =>
                        {
                            Vector3 scale = new Vector3(xScale, 1, 1);
                            this.mainBarImage.transform.localScale = scale;
                            this.bufferBarImage.transform.localScale = scale;
                        },
                        onComplete:
                        () =>
                        {
                            this.StopTweensAndFlashes();

                            this.current = current;
                            this.maximum = maximum;
                            if (this.showText)
                            {
                                this.currentText.text = this.current.ToString();
                                this.maxText.text = this.maximum.ToString();
                            }

                            this.mainBarImage.fillAmount = this.maximum == 0 ? 0 : ((float)this.current / this.maximum);
                            this.bufferBarImage.fillAmount = this.mainBarImage.fillAmount;

                            this.transitionRoutines.Add(
                                FrigidCoroutine.Run(
                                    Tween.Value(
                                        this.transitionDuration.ImmutableValue * (1 - transitionRatio),
                                        0,
                                        1,
                                        EasingType.EaseInOutSine,
                                        useRealTime: true,
                                        onValueUpdated:
                                        (float xScale) =>
                                        {
                                            Vector3 scale = new Vector3(xScale, 1, 1);
                                            this.mainBarImage.transform.localScale = scale;
                                            this.bufferBarImage.transform.localScale = scale;
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

        public void SetCurrent(int current)
        {
            if (current == this.current) return;

            this.StopTweensAndFlashes();

            int previousCurrent = this.current;
            this.current = current;

            if (this.showText)
            {
                if (this.showCurrentTextFlash) this.FlashTextOutline(this.currentTextOutline);
                this.currentText.text = this.current.ToString();
            }

            this.TweenAndFlashBar(this.maximum == 0 ? 0f : ((float)previousCurrent / this.maximum), this.maximum == 0 ? 0f : ((float)this.current / this.maximum));
        }

        public void SetMaximum(int maximum)
        {
            if (maximum == this.maximum) return;

            this.StopTweensAndFlashes();

            int previousMaximum = this.maximum;
            this.maximum = maximum;
            if (this.showText)
            {
                if (this.showMaxTextFlash) this.FlashTextOutline(this.maxTextOutline);
                this.maxText.text = this.maximum.ToString();
            }

            this.TweenAndFlashBar(previousMaximum == 0 ? 0f : ((float)this.current / previousMaximum), this.maximum == 0 ? 0f : ((float)this.current / this.maximum));
        }

        protected override void Awake()
        {
            base.Awake();
            this.mainBarColor = this.mainBarImage.color;
            foreach (Image mainBarAccentImage in this.mainBarAccentImages) mainBarAccentImage.color = this.mainBarColor;
            this.bufferBarColor = this.bufferBarImage.color;
            foreach (Image bufferBarAccentImage in this.bufferBarAccentImages) bufferBarAccentImage.color = this.bufferBarColor;

            this.transitionRoutines = new List<FrigidCoroutine>();
            this.changeRoutines = new List<FrigidCoroutine>();

            this.current = 0;
            this.maximum = 0;
            this.currentText.text = this.current.ToString();
            this.maxText.text = this.maximum.ToString();
            this.mainBarImage.fillAmount = 0f;
            this.bufferBarImage.fillAmount = 0f;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
        private void FlashTextOutline(Outline textOutline)
        {
            this.changeRoutines.Add(
                FrigidCoroutine.Run(
                    Tween.Value(
                        this.individualTweenFlashDuration.ImmutableValue,
                        0,
                        1,
                        EasingType.EaseInOutSine,
                        numberIterations:
                        this.numberFlashes.ImmutableValue,
                        pingPong: true,
                        useRealTime: true,
                        onValueUpdated: (float alpha) => { textOutline.effectColor = new Color(textOutline.effectColor.r, textOutline.effectColor.g, textOutline.effectColor.b, alpha); }
                        ),
                    this.gameObject
                    )
                );
        }

        private void TweenAndFlashBar(float from, float to)
        {
            this.bufferBarImage.fillAmount = from;
            this.mainBarImage.fillAmount = to;

            float fillDuration = from < to ? this.increaseFillDuration.ImmutableValue : this.decreaseFillDuration.ImmutableValue;

            if (this.flashMainBar)
            {
                this.changeRoutines.Add(
                    FrigidCoroutine.Run(
                        Tween.Value(
                            this.individualTweenFlashDuration.ImmutableValue,
                            this.mainBarFlashColor.ImmutableValue,
                            this.mainBarColor,
                            EasingType.EaseInOutSine,
                            numberIterations: this.numberFlashes.ImmutableValue,
                            pingPong: true,
                            useRealTime: true,
                            onValueUpdated: (Color color) => { this.mainBarImage.color = color; }
                            ),
                        this.gameObject
                        )
                    );
                this.mainBarImage.color = this.mainBarColor;
            }

            if (this.flashBufferBar)
            {
                this.changeRoutines.Add(
                    FrigidCoroutine.Run(
                        Tween.Value(
                            this.individualTweenFlashDuration.ImmutableValue,
                            this.bufferBarFlashColor.ImmutableValue,
                            this.bufferBarColor,
                            EasingType.EaseInOutSine,
                            numberIterations: this.numberFlashes.ImmutableValue,
                            pingPong: true,
                            useRealTime: true,
                            onValueUpdated: (Color color) => { this.bufferBarImage.color = color; },
                            onComplete:
                            () =>
                            {
                                this.bufferBarImage.color = this.bufferBarColor;
                                this.changeRoutines.Add(
                                    FrigidCoroutine.Run(
                                        Tween.Value(
                                            fillDuration,
                                            this.bufferBarImage.fillAmount,
                                            to,
                                            EasingType.EaseOutSine,
                                            useRealTime: true,
                                            onValueUpdated: (float fill) => { this.bufferBarImage.fillAmount = fill; }
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
            else
            {
                this.changeRoutines.Add(
                    FrigidCoroutine.Run(
                        Tween.Value(
                            fillDuration,
                            this.bufferBarImage.fillAmount,
                            to,
                            EasingType.EaseOutSine,
                            useRealTime: true,
                            onValueUpdated: (float fill) => { this.bufferBarImage.fillAmount = fill; }
                            ),
                        this.gameObject
                        )
                    );
            }
        }

        private void StopTweensAndFlashes()
        {
            foreach (FrigidCoroutine changeRoutine in this.changeRoutines) FrigidCoroutine.Kill(changeRoutine);
            this.changeRoutines.Clear();

            if (this.showText && this.showCurrentTextFlash)
            {
                this.currentTextOutline.effectColor = new Color(this.currentTextOutline.effectColor.r, this.currentTextOutline.effectColor.g, this.currentTextOutline.effectColor.b, 0);
            }
            if (this.showText && this.showMaxTextFlash)
            {
                this.maxTextOutline.effectColor = new Color(this.maxTextOutline.effectColor.r, this.maxTextOutline.effectColor.g, this.maxTextOutline.effectColor.b, 0);
            }
            if (this.flashMainBar)
            {
                this.mainBarImage.color = this.mainBarColor;
            }
            if (this.flashBufferBar)
            {
                this.bufferBarImage.color = this.bufferBarColor;
            }
        }
    }
}
