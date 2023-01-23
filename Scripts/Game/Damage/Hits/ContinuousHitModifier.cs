using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ContinuousHitModifier : HitModifier
    {
        public override bool ShouldApplyOnHit(Vector2 hitPosition, Vector2 hitDirection)
        {
            return true;
        }
    }
}
