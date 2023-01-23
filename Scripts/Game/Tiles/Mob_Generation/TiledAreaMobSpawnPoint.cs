using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class TiledAreaMobSpawnPoint
    {
        [SerializeField]
        private Vector2 localPosition;
        [SerializeField]
        private List<MobReach> reaches;

        public Vector2 LocalPosition
        {
            get
            {
                return this.localPosition;
            }
        }

        public List<MobReach> Reaches
        {
            get
            {
                return this.reaches;
            }
        }

        public TiledAreaMobSpawnPoint(Vector2 localPosition, List<MobReach> reaches)
        {
            this.localPosition = localPosition;
            this.reaches = reaches;
        }

        public bool CanSpawnHere(MobSpawnable mobSpawnable, Vector2 absoluteCenterPosition)
        {
            return false;
            /* TODO Fix later
                this.reaches.Intersect(mobSpawnable.Reaches).Count() > 0 && 
                mobSpawnable.CanSpawnAt(this.localPosition + absoluteCenterPosition);
            */
        }
    }
}
