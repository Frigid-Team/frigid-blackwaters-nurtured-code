using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ShockVibrationEffect : SpecialEffect
    {
        [SerializeField]
        private FloatSerializedReference maxRotationInDegrees;
        [SerializeField]
        private IntSerializedReference numberIterations;
        [SerializeField]
        private FloatSerializedReference rotationDuration;
        [SerializeField]
        private Transform rotationTransform;

        private FrigidCoroutine currentRotationRoutine;

        protected override void Played(float effectsTimeScaling = 1)
        {
            RotateShakeAnimation(this.maxRotationInDegrees.ImmutableValue, this.numberIterations.ImmutableValue, effectsTimeScaling);
        }

        protected override void Stopped() 
        {
            FrigidCoroutine.Kill(this.currentRotationRoutine);
            UpdateZRotation(0);
        }

        private void RotateShakeAnimation(float maxRotation, int iterations, float effectsTimeScaling)
        {
            float rotationDuration = this.rotationDuration.ImmutableValue * effectsTimeScaling;
            this.currentRotationRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    rotationDuration,
                    maxRotation,
                    0,
                    onComplete:
                    () =>
                    {
                        this.currentRotationRoutine = FrigidCoroutine.Run(
                            TweenCoroutine.Value(
                                -maxRotation,
                                0,
                                rotationDuration,
                                onComplete:
                                () =>
                                {
                                    UpdateZRotation(0);
                                    if (iterations != 1)
                                    {
                                        RotateShakeAnimation(maxRotation / iterations, iterations - 1, effectsTimeScaling);
                                    }
                                }
                                ),
                            this.gameObject
                            );
                    }
                    ),
                this.gameObject
                );
        }

        private void UpdateZRotation(float rotation)
        {
            this.rotationTransform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }
}
