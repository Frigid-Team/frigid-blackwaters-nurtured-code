using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class HitModifier : FrigidMonoBehaviour
    {
        public abstract bool ApplyOnHit(Vector2 hitPosition, Vector2 hitDirection, ref int damage);
    }
}
