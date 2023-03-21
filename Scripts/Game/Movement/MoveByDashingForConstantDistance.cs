using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MoveByDashingForConstantDistance : MoveByDashing
    {
        [SerializeField]
        private FloatSerializedReference dashDistance;

        public float DashDistance
        {
            get
            {
                return this.dashDistance.ImmutableValue;
            }
        }

        protected override float GetMovementDistance(float dashSpeed)
        {
            return this.dashDistance.ImmutableValue;
        }
    }
}
