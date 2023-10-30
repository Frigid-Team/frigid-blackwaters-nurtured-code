using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageInfo
    {
        private Collider2D collision;
        private float timeOfIncident;

        public DamageInfo(Collider2D collision)
        {
            this.collision = collision;
            this.timeOfIncident = Time.fixedTime; // These should only be constructed on physics updates.
        }

        public Collider2D Collision
        {
            get
            {
                return this.collision;
            }
        }

        public float TimeOfIncident
        {
            get
            {
                return this.timeOfIncident;
            }
        }

        public abstract bool IsNonTrivial
        {
            get;
        }
    }
}
