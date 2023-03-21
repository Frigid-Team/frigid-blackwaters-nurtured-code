using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "MobSpawnable", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.MOBS + "MobSpawnable")]
    public class MobSpawnable : FrigidScriptableObject
    {
        [SerializeField]
        private Mob mobPrefab;
        [SerializeField]
        private int tier;

        public int Tier
        {
            get
            {
                return this.tier;
            }
        }

        public bool CanSpawnMobAt(Vector2 spawnPosition)
        {
            return this.mobPrefab.CanSpawnAt(spawnPosition);
        }

        public Mob SpawnMob(Vector2 spawnPosition, Vector2 facingDirection)
        {
            if (!CanSpawnMobAt(spawnPosition))
            {
                Debug.LogError("Attempted to spawn Mob from MobSpawnable " + this.name + " at an invalid position.");
                return null;
            }

            Mob mob = FrigidInstancing.CreateInstance<Mob>(this.mobPrefab);
            mob.Spawn(spawnPosition, facingDirection);
            return mob;
        }
    }
}
