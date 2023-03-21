using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobBlockedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private FloatSerializedReference detectionDistance;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.ImmutableValue.LinePushCast(this.mob.ImmutableValue.Position, this.mob.ImmutableValue.FacingDirection, this.detectionDistance.ImmutableValue).Count > 0;
        }
    }
}
