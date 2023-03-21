using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobThreatReceivedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private FloatSerializedReference lastingDuration;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return 
                this.mob.ImmutableValue.ThreatsReceived.Count > 0 && 
                Time.time - this.mob.ImmutableValue.ThreatsReceived.First.Value.TimeWarned < Time.deltaTime + this.lastingDuration.ImmutableValue;
        }
    }
}
