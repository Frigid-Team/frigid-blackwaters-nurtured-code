using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobHitDealtConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private FloatSerializedReference lastingDuration;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return
                this.mob.ImmutableValue.HitsDealt.Count > 0 &&
                Time.time - this.mob.ImmutableValue.HitsDealt.First.Value.TimeHit < this.lastingDuration.ImmutableValue + Time.deltaTime;
        }
    }
}
