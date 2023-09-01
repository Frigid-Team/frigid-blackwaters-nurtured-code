using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "CharacterTiledAreaMobSpawner", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "CharacterTiledAreaMobSpawner")]
    public class CharacterTiledAreaMobSpawner : TiledAreaMobSpawner
    {
        private SceneVariable<List<MobSpawnable>> mobSpawnablesRemaining;

        [SerializeField]
        private List<MobSpawnable> mobSpawnables;

        protected override void OnBegin()
        {
            base.OnBegin();
            this.mobSpawnablesRemaining = new SceneVariable<List<MobSpawnable>>(() => new List<MobSpawnable>(this.mobSpawnables));
        }

        protected override Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> DetermineMobSpawns(TiledLevelPlanArea planArea, TiledArea area, HashSet<TiledAreaMobSpawnPoint> mobSpawnPoints, int waveIndex)
        {
            Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> mobSpawns = new Dictionary<TiledAreaMobSpawnPoint, MobSpawnable>();

            foreach (TiledAreaMobSpawnPoint spawnPoint in mobSpawnPoints)
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
