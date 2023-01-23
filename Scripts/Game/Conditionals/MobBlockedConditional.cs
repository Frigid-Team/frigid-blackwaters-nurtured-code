using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobBlockedConditional : Conditional
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private FloatSerializedReference detectionDistance;

        protected override bool CustomValidate()
        {
            return this.mob.Physicality.LinePushCast(this.mob.AbsolutePosition, this.mob.FacingDirection, this.detectionDistance.ImmutableValue).Count > 0;
        }
    }
}
