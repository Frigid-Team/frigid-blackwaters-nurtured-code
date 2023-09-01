using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobBlockedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private FloatSerializedReference detectionDistance;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.TryGetValue(out Mob mob) && mob.PushCast(mob.Position, mob.FacingDirection, this.detectionDistance.ImmutableValue, out _);
        }
    }
}
