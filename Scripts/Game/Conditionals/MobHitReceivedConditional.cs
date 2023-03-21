using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobHitReceivedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private FloatSerializedReference lastingDuration;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return 
                this.mob.ImmutableValue.HitsReceived.Count > 0 && 
                Time.time - this.mob.ImmutableValue.HitsReceived.First.Value.TimeHit < this.lastingDuration.ImmutableValue + Time.deltaTime;
        }
    }
}
