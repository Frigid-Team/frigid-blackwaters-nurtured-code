using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobModifiedHitReceivedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private List<HitModifier> hitModifiers;
        [SerializeField]
        private FloatSerializedReference lastingDuration;
        [SerializeField]
        private IntSerializedReference numberHitsRequired;
        [SerializeField]
        private FloatSerializedReference chanceOfNotOccuring;
        [SerializeField]
        private IntSerializedReference minimumDamage;
        [SerializeField]
        private bool onlyOnNonTrivial;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.Tally(elapsedDuration, elapsedDurationDelta) > 0;
        }

        protected override int CustomTally(float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return 0;
            }

            int numberHits = 0;
            foreach (HitInfo hitInfo in mob.HitsReceived)
            {
                if (Time.time - hitInfo.TimeHit >= this.lastingDuration.ImmutableValue + Time.deltaTime)
                {
                    break;
                }
                if ((this.numberHitsRequired.ImmutableValue == 0 || mob.HitsReceived.Count % this.numberHitsRequired.ImmutableValue == 0) &&
                    Random.Range(0f, 1f) >= this.chanceOfNotOccuring.ImmutableValue &&
                    hitInfo.Damage >= this.minimumDamage.ImmutableValue && (!this.onlyOnNonTrivial || hitInfo.IsNonTrivial) &&
                    hitInfo.AppliedHitModifiers.Intersect(this.hitModifiers).Count() > 0)
                {
                    numberHits++;
                }
            }
            return numberHits;
        }
    }
}
