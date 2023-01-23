using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TerrainTileAsset", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TerrainTileAsset")]
    public class TerrainTileAsset : FrigidScriptableObject
    {
        [SerializeField]
        private string blueprintId;
        [SerializeField]
        private TileTerrain terrain;
        [SerializeField]
        private TerrainTile terrainTilePrefab;
        [SerializeField]
        private List<TerrainCrossover> terrainCrossovers;

        private Dictionary<string, TerrainCrossoverTile> crossoverMap;

        public string BlueprintID
        {
            get
            {
                return this.blueprintId;
            }
        }

        public TileTerrain Terrain
        {
            get
            {
                return this.terrain;
            }
        }

        public TerrainTile TerrainTilePrefab
        {
            get
            {
                return this.terrainTilePrefab;
            }
        }

        public bool TryGetTerrainCrossoverTilePrefab(string crossoverBlueprintId, out TerrainCrossoverTile terrainCrossoverTilePrefab)
        {
            return this.crossoverMap.TryGetValue(crossoverBlueprintId, out terrainCrossoverTilePrefab);
        }

        protected override void Init()
        {
            base.Init();
            this.crossoverMap = new Dictionary<string, TerrainCrossoverTile>();
            foreach (TerrainCrossover terrainCrossover in this.terrainCrossovers)
            {
                foreach (string crossoverBlueprintId in terrainCrossover.CrossoverBlueprintIDs)
                {
                    if (this.crossoverMap.ContainsKey(crossoverBlueprintId))
                    {
                        Debug.LogWarning("TerrainTileAsset " + this.name + " has a duplicate crossover blueprint id: " + crossoverBlueprintId + ".");
                        continue;
                    }
                    this.crossoverMap.Add(crossoverBlueprintId, terrainCrossover.TerrainCrossoverTilePrefab);
                }
            }
        }

        [Serializable]
        private struct TerrainCrossover
        {
            [SerializeField]
            private List<string> crossoverBlueprintIds;
            [SerializeField]
            private TerrainCrossoverTile terrainCrossoverTilePrefab;

            public List<string> CrossoverBlueprintIDs
            {
                get
                {
                    return this.crossoverBlueprintIds;
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
