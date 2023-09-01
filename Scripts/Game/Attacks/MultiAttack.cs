using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MultiAttack : Attack
    {
        [SerializeField]
        private bool loop;
        [SerializeField]
        private Step[] steps;

        public override void Perform(float elapsedDuration, Action onComplete = null)
        {
            IEnumerator<FrigidCoroutine.Delay> StepThrough()
            {
                float accumulatedDuration = 0;
                foreach (Step step in this.steps)
                {
                    int numberRepetitions = Mathf.Max(1, 1 + step.NumberRepetitionsByReference.MutableValue);

                    for (int i = 0; i < numberRepetitions; i++)
                    {
                        foreach (Attack attack in step.Attacks)
                        {
                            attack.DamageAlignment = this.DamageAlignment;
                            Action<HitInfo> onHitDealt = this.OnHitDealt;
                            Action<BreakInfo> onBreakDealt = this.OnBreakDealt;
                            Action<ThreatInfo> onThreatDealt = this.OnThreatDealt;

                            attack.DamageBonus += this.DamageBonus;
                            attack.OnHitDealt += onHitDealt;
                            attack.OnBreakDealt += onBreakDealt;
                            attack.OnThreatDealt += onThreatDealt;
                            attack.Perform(
                                elapsedDuration + accumulatedDuration, 
                                () => 
                                {
                                    attack.DamageBonus -= this.DamageBonus;
                                    attack.OnHitDealt -= onHitDealt;
                                    attack.OnBreakDealt -= onBreakDealt;
                                    attack.OnThreatDealt -= onThreatDealt;
                                }
                                );
                        }

                        float duration = step.DurationByReference.MutableValue;
                        yield return new FrigidCoroutine.DelayForSeconds(duration);
                        accumulatedDuration += duration;
                    }
                }
                onComplete?.Invoke();
            }
            FrigidCoroutine.Run(StepThrough(), this.gameObject);
        }

        [Serializable]
        private struct Step
        {
            [SerializeField]
            private Attack[] attacks;
            [SerializeField]
            private FloatSerializedReference duration;
            [SerializeField]
            private IntSerializedReference numberRepetitions;

            public Attack[] Attacks
            {
                get
                {
                    return this.attacks;
                }
            }

            public FloatSerializedReference DurationByReference
            {
                get
                {
                    return this.duration;
                }
            }

            public IntSerializedReference NumberRepetitionsByReference
            {
                get
                {
                    return this.numberRepetitions;
                }
            }
        }
    }
}
