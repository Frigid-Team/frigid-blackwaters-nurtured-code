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
        private List<MobSpawnable> restrictedMobSpawnables;

        public Vector2 LocalPosition
        {
            get
            {
                return this.localPosition;
            }
        }

        public TiledAreaMobSpawnPoint(Vector2 localPosition, List<MobSpawnable> restrictedMobSpawnables)
        {
            this.localPosition = localPosition;
            this.restrictedMobSpawnables = restrictedMobSpawnables;
        }

        public Vector2 GetSpawnPosition(TiledArea tiledArea)
        {
            return this.localPosition + tiledArea.CenterPosition;
        }

        public bool CanSpawnHere(MobSpawnable mobSpawnable, TiledArea tiledArea)
        {
            return !this.restrictedMobSpawnables.Contains(mobSpawnable) && mobSpawnable.CanSpawnMobAt(GetSpawnPosition(tiledArea));
        }
    }
}
