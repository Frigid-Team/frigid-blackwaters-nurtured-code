using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageInfo
    {
        private Collider2D collision;

        public DamageInfo(Collider2D collision)
        {
            this.collision = collision;
        }

        public Collider2D Collision
        {
            get
            {
                return this.collision;
            }
        }

        public abstract bool IsNonTrivial
        {
            get;
        }
    }
}
