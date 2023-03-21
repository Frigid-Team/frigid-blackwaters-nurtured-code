using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "OneRandomTiledAreaMobGeneratorStrategy", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "OneRandomTiledAreaMobGeneratorStrategy")]
    public class OneRandomTiledAreaMobGeneratorStrategy : TiledAreaMobGeneratorStrategy
    {
        [SerializeField]
        private MobSpawnable mobSpawnable;
        [SerializeField]
        private bool lockEntrancesUntilDead;

        public override bool ShouldLockEntrances(MobSet mobsInPreviousWaves, MobSet mobsInWavesToAdvance)
        {
            return mobsInPreviousWaves.ThatAreNotDead().Count > 0 && this.lockEntrancesUntilDead;
        }

        protected override Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> DetermineMobSpawnsInTiledArea(TiledLevelPlanArea planArea, TiledArea tiledArea, HashSet<TiledAreaMobSpawnPoint> spawnPoints, int waveIndex)
        {
            Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> mobSpawns = new Dictionary<TiledAreaMobSpawnPoint, MobSpawnable>();

            foreach (TiledAreaMobSpawnPoint spawnPoint in spawnPoints)
            {
                mobSpawns.Add(spawnPoint, this.mobSpawnable);
                break;
            }
            return mobSpawns;
        }
    }
}
