using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "CharacterTiledAreaMobGeneratorStrategy", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "CharacterTiledAreaMobGeneratorStrategy")]
    public class CharacterTiledAreaMobGeneratorStrategy : TiledAreaMobGeneratorStrategy
    {
        private SceneVariable<List<MobSpawnable>> mobSpawnablesRemaining;

        [SerializeField]
        private List<MobSpawnable> mobSpawnables;

        protected override void Init()
        {
            base.Init();
            this.mobSpawnablesRemaining = new SceneVariable<List<MobSpawnable>>(() => { return new List<MobSpawnable>(this.mobSpawnables); });
        }

        protected override Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> DetermineMobSpawnsInTiledArea(TiledLevelPlanArea planArea, TiledArea tiledArea, HashSet<TiledAreaMobSpawnPoint> spawnPoints, int waveIndex)
        {
            Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> mobSpawns = new Dictionary<TiledAreaMobSpawnPoint, MobSpawnable>();

            foreach (TiledAreaMobSpawnPoint spawnPoint in spawnPoints)
            {
                if (this.mobSpawnablesRemaining.Current.Count == 0) break;

                int chosenIndex = Random.Range(0, this.mobSpawnablesRemaining.Current.Count);
                mobSpawns.Add(spawnPoint, this.mobSpawnablesRemaining.Current[chosenIndex]);
                this.mobSpawnablesRemaining.Current.RemoveAt(chosenIndex);
            }
            return mobSpawns;
        }
    }
}
