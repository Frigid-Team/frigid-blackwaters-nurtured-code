using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ConicHitModifier : HitModifier
    {
        [SerializeField]
        private Direction normalDirection;
        [SerializeField]
        private FloatSerializedReference coneAngleDegrees;

        public override bool ShouldApplyOnHit(Vector2 hitPosition, Vector2 hitDirection)
        {
            return Vector2.Angle(-hitDirection, this.normalDirection.Calculate(hitDirection, 0, 0)) < this.coneAngleDegrees.ImmutableValue / 2;
        }
    }
}
