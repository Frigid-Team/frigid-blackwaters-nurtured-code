using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ContinuousHitModifier : HitModifier
    {
        public override bool ApplyOnHit(Vector2 hitPosition, Vector2 hitDirection, ref int damage)
        {
            damage = 0;
            return true;
        }
    }
}
