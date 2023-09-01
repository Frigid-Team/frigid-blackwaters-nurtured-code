using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "OneRandomTiledAreaMobSpawner", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "OneRandomTiledAreaMobSpawner")]
    public class OneRandomTiledAreaMobSpawner : TiledAreaMobSpawner
    {
        [SerializeField]
        private MobSpawnable mobSpawnable;
        [SerializeField]
        private bool lockEntrancesUntilDead;

        public override bool ShouldLockEntrances(HashSet<Mob> mobsInPreviousWaves, HashSet<Mob> mobsInWavesToAdvance)
        {
            return mobsInPreviousWaves.Any((Mob mobInPreviousWave) => mobInPreviousWave.Status != MobStatus.Dead) && this.lockEntrancesUntilDead;
        }

        protected override Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> DetermineMobSpawns(TiledLevelPlanArea planArea, TiledArea area, HashSet<TiledAreaMobSpawnPoint> mobSpawnPoints, int waveIndex)
        {
            Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> mobSpawns = new Dictionary<TiledAreaMobSpawnPoint, MobSpawnable>();
            foreach (TiledAreaMobSpawnPoint spawnPoint in mobSpawnPoints)
            {
                mobSpawns.Add(spawnPoint, this.mobSpawnable);
                break;
            }
            return mobSpawns;
        }
    }
}
