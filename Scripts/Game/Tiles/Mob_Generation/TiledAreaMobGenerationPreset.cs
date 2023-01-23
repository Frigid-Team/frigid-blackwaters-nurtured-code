using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledAreaMobGenerationPreset", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TiledAreaMobGenerationPreset")]
    public class TiledAreaMobGenerationPreset : FrigidScriptableObject
    {
        [SerializeField]
        private List<SpawnPointPreset> spawnPointPresets;

        public List<SpawnPointPreset> SpawnPointPresets
        {
            get
            {
                return this.spawnPointPresets;
            }
        }

        public void Setup()
        {
            this.spawnPointPresets = new List<SpawnPointPreset>();
        }

        [Serializable]
        public struct SpawnPointPreset
        {
            [SerializeField]
            private TiledAreaMobSpawnPoint spawnPoint;
            [SerializeField]
            private string strategyId;

            public SpawnPointPreset(string strategyId, TiledAreaMobSpawnPoint spawnPoint)
            {
                this.strategyId = strategyId;
                this.spawnPoint = spawnPoint;
            }

            public string StrategyID
            {
                get
                {
                    return this.strategyId;
                }
            }
            
            public TiledAreaMobSpawnPoint SpawnPoint
            {
                get
                {
                    return this.spawnPoint;
                }
            }
        }
    }
}