using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TerrainTileAsset", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "TerrainTileAsset")]
    public class TerrainTileAsset : FrigidScriptableObject
    {
        [SerializeField]
        private TileTerrain terrain;
        [SerializeField]
        private int elevationFudgeFactor;
        [SerializeField]
        private TerrainTile terrainTilePrefab;
        [SerializeField]
        private List<TerrainCrossover> terrainCrossovers;

        private Dictionary<TerrainTileAsset, TerrainCrossoverTile> crossoverMap;

        public TileTerrain Terrain
        {
            get
            {
                return this.terrain;
            }
        }

        public int ElevationFudgeFactor
        {
            get
            {
                return this.elevationFudgeFactor;
            }
        }

        public TerrainTile TerrainTilePrefab
        {
            get
            {
                return this.terrainTilePrefab;
            }
        }

        public bool TryGetTerrainCrossoverTilePrefab(TerrainTileAsset crossoverTerrainTileAsset, out TerrainCrossoverTile terrainCrossoverTilePrefab)
        {
            return this.crossoverMap.TryGetValue(crossoverTerrainTileAsset, out terrainCrossoverTilePrefab);
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.crossoverMap = new Dictionary<TerrainTileAsset, TerrainCrossoverTile>();
            foreach (TerrainCrossover terrainCrossover in this.terrainCrossovers)
            {
                foreach (TerrainTileAsset crossoverTerrainTileAsset in terrainCrossover.CrossoverTerrainTileAssets)
                {
                    if (this.crossoverMap.ContainsKey(crossoverTerrainTileAsset))
                    {
                        Debug.LogWarning("TerrainTileAsset " + this.name + " has a duplicate crossover TerrainTileAsset: " + crossoverTerrainTileAsset.name + ".");
                        continue;
                    }
                    this.crossoverMap.Add(crossoverTerrainTileAsset, terrainCrossover.TerrainCrossoverTilePrefab);
                }
            }
        }

        [Serializable]
        private struct TerrainCrossover
        {
            [SerializeField]
            private List<TerrainTileAsset> crossoverTerrainTileAssets;
            [SerializeField]
            private TerrainCrossoverTile terrainCrossoverTilePrefab;

            public List<TerrainTileAsset> CrossoverTerrainTileAssets
            {
                get
                {
                    return this.crossoverTerrainTileAssets;
                }
            }

            public TerrainCrossoverTile TerrainCrossoverTilePrefab
            {
                get
                {
                    return this.terrainCrossoverTilePrefab;
                }
            }
        }
    }
}
