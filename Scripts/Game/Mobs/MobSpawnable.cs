using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobSpawnable : FrigidScriptableObject
    {
        // MOBS_V2_TODO: Fill out later

        [SerializeField]
        private Mob mob;
        [SerializeField]
        private int tier;

        public int Tier
        {
            get
            {
                return this.tier;
            }
        }

        public TraversableTerrain SpawnTraversableTerrain
        {
            get
            {
                return this.mob.InitialTraversableTerrain;
            }
        }
    }
}
