using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "MobSpawnable", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Mobs + "MobSpawnable")]
    public class MobSpawnable : FrigidScriptableObject
    {
        [SerializeField]
        private Mob mobPrefab;
        [SerializeField]
        private int tier;
        [SerializeField]
        private List<MobSpawnTag> spawnTags;

        private Action<Mob> onSpawned;

        public int Tier
        {
            get
            {
                return this.tier;
            }
        }

        public List<MobSpawnTag> SpawnTags
        {
            get
            {
                return this.spawnTags;
            }
        }

        public Action<Mob> OnSpawned
        {
            get
            {
                return this.onSpawned;
            }
            set
            {
                this.onSpawned = value;
            }
        }

        public bool CanSpawnMobAt(Vector2 spawnPosition)
        {
            return this.mobPrefab.CanSpawnAt(spawnPosition);
        }

        public Mob SpawnMob(Vector2 spawnPosition, Vector2 facingDirection)
        {
            Debug.Assert(this.CanSpawnMobAt(spawnPosition), "Attempted to spawn Mob from MobSpawnable " + this.name + " at an invalid position.");

            Mob mob = FrigidMonoBehaviour.CreateInstance<Mob>(this.mobPrefab);
            mob.Spawn(spawnPosition, facingDirection);
            this.onSpawned?.Invoke(mob);
            return mob;
        }
    }
}
