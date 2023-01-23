using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobState : MobStateNode
    {
        [SerializeField]
        private Vector2 size;
        [SerializeField]
        private TraversableTerrain traversableTerrain;

        public Vector2 Size
        {
            get
            {
                return this.size;
            }
        }

        public TraversableTerrain TraversableTerrain
        {
            get
            {
                return this.traversableTerrain;
            }
        }

        public abstract bool Dead
        {
            get;
        }
    }
}
