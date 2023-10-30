using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MultiAttack : Attack
    {
        [SerializeField]
        private Step[] steps;

        public override void Perform(float elapsedDuration, ref Action toForceComplete, Action onComplete = null)
        {
            Action toForceAttackComplete = null;
            toForceAttackComplete += () => toForceAttackComplete.Invoke();
            IEnumerator<FrigidCoroutine.Delay> StepThrough()
            {
                int numberAttacks = 0;
                foreach (Step step in this.steps)
                {
                    numberAttacks += step.Attacks.Length;
                }
                if (numberAttacks == 0)
                {
                    onComplete?.Invoke();
                    yield break;
                }
                int numberAttacksCompleted = 0;

                float accumulatedDuration = 0;
                foreach (Step step in this.steps)
                {
                    int numberIterations = Mathf.Max(1, 1 + step.NumberRepetitionsByReference.MutableValue);

                    for (int i = 0; i < numberIterations; i++)
                    {
                        foreach (Attack attack in step.Attacks)
                        {
                            attack.DamageAlignment = this.DamageAlignment;
                            attack.IsIgnoringDamage = this.IsIgnoringDamage;
                            Action<HitInfo> onHitDealt = this.OnHitDealt;
                            Action<BreakInfo> onBreakDealt = this.OnBreakDealt;
                            Action<ThreatInfo> onThreatDealt = this.OnThreatDealt;

                            attack.DamageBonus += this.DamageBonus;
                            attack.OnHitDealt += onHitDealt;
                            attack.OnBreakDealt += onBreakDealt;
                            attack.OnThreatDealt += onThreatDealt;
                            attack.Perform(
                                elapsedDuration + accumulatedDuration, 
                                ref toForceAttackComplete,
                                () => 
                                {
                                    attack.DamageBonus -= this.DamageBonus;
                                    attack.OnHitDealt -= onHitDealt;
                                    attack.OnBreakDealt -= onBreakDealt;
                                    attack.OnThreatDealt -= onThreatDealt;

                                    numberAttacksCompleted++;
                                    if (numberAttacksCompleted == numberAttacks)
                                    {
                                        onComplete?.Invoke();
                                    }
                                }
                                );
                        }

                        float duration = step.DurationByReference.MutableValue;
                        yield return new FrigidCoroutine.DelayForSeconds(duration);
                        accumulatedDuration += duration;
                    }
                }
            }
            FrigidCoroutine stepThroughRoutine = FrigidCoroutine.Run(StepThrough(), this.gameObject);
            toForceComplete += () => FrigidCoroutine.Kill(stepThroughRoutine);
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
