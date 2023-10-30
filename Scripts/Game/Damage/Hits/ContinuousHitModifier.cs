using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ContinuousHitModifier : HitModifier
    {
        public override bool TryApplyOnHit(Vector2 hitPosition, Vector2 hitDirection, ref int damage, out bool evaluateFollowingModifiers)
        {
            damage = 0;
            evaluateFollowingModifiers = false;
            return true;
        }
    }
}
