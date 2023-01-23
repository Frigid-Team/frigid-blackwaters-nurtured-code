using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class BreakableFragment : FrigidMonoBehaviour
    {
        [SerializeField]
        private float upwardForce;
        [SerializeField]
        private AnimationCurveSerializedReference upwardCurve;
        [SerializeField]
        private float totalDuration;
        [SerializeField]
        private float rotationSpeed;
        [SerializeField]
        private bool alignStraight;
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private FragmentAnimationSet[] fragmentAnimations;

        public void LaunchFragment(Vector2 impactForce)
        {
            FrigidCoroutine.Run(FragmentLifetime(impactForce), this.gameObject);
        }

        private IEnumerator<FrigidCoroutine.Delay> FragmentLifetime(Vector2 impactForce)
        {
            FragmentAnimationSet chosenAnimations = this.fragmentAnimations[UnityEngine.Random.Range(0, this.fragmentAnimations.Length)];

            this.animatorBody.PlayByName(chosenAnimations.LaunchingAnimationName);

            float durationElapsed = 0;
            this.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
            float originalY = this.transform.position.y;

            if (this.alignStraight)
            {
                FrigidCoroutine.Run(
                    TweenCoroutine.Value(
                        this.totalDuration,
                        this.transform.rotation.eulerAngles.z,
                        (UnityEngine.Random.Range(0, 2) == 1 ? UnityEngine.Random.Range(-4, -2) : UnityEngine.Random.Range(3, 5)) * 360,
                        EasingType.EaseOutCirc,
                        onValueUpdated: (float rotation) => this.transform.rotation = Quaternion.Euler(0, 0, rotation)
                        ),
                    this.gameObject
                    );
            }

            while (durationElapsed < this.totalDuration)
            {
                durationElapsed += Time.deltaTime;
                float totalPercentCompleted = durationElapsed / this.totalDuration;
                float totalPercentRemaining = 1 - totalPercentCompleted;
                this.transform.position += new Vector3(
                    impactForce.x * totalPercentRemaining,
                    impactForce.y * totalPercentRemaining + this.upwardForce * this.upwardCurve.ImmutableValue.Evaluate(totalPercentCompleted)
                    ) * Time.deltaTime;
                
                if (!this.alignStraight)
                {
                    this.transform.Rotate(new Vector3(0, 0, this.rotationSpeed * totalPercentRemaining * Time.deltaTime));
                }

                yield return null;
            }

            this.animatorBody.PlayByName(chosenAnimations.FallenAnimationName);
        }

        [Serializable]
        private struct FragmentAnimationSet
        {
            [SerializeField]
            private string launchingAnimationName;
            [SerializeField]
            private string fallenAnimationName;

            public string LaunchingAnimationName
            {
                get
                {
                    return this.launchingAnimationName;
                }
            }

            public string FallenAnimationName
            {
                get
                {
                    return this.fallenAnimationName;
                }
            }
        }
    }
}
