using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MoveByDashingForConstantDuration : MoveByDashing
    {
        [SerializeField]
        private FloatSerializedReference dashDuration;

        protected override float GetMovementDistance(float dashSpeed)
        {
            return dashSpeed * this.dashDuration.ImmutableValue;
        }
    }
}
