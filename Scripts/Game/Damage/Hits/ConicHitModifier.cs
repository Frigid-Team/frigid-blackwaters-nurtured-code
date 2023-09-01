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

        public override bool ApplyOnHit(Vector2 hitPosition, Vector2 hitDirection, ref int damage)
        {
            if (Vector2.Angle(-hitDirection, this.normalDirection.Retrieve(hitDirection, 0, 0)) < this.coneAngleDegrees.ImmutableValue / 2)
            {
                damage = 0;
                return true;
            }
            return false;
        }
    }
}
