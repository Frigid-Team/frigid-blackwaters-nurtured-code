using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TerrainContentAssetGroup", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TerrainContentAssetGroup")]
    public class TerrainContentAssetGroup : FrigidScriptableObject
    {
        [SerializeField]
        private List<TerrainContentAsset> terrainContentAssets;

        private Dictionary<TerrainContentHeight, Dictionary<TileTerrain, Dictionary<string, TerrainContentAsset>>> terrainContentAssetMap;

        public List<TerrainContentAsset> TerrainContentAssets
        {
            get
            {
                return this.terrainContentAssets;
            }
        }

        public bool TryGetTerrainContentAsset(TerrainContentHeight height, TileTerrain terrain, string blueprintId, out TerrainContentAsset terrainContentAsset)
        {
            terrainContentAsset = null;
            return 
                this.terrainContentAssetMap.TryGetValue(height, out Dictionary<TileTerrain, Dictionary<string, TerrainContentAsset>> terrainsToIdsAndAssets) &&
                terrainsToIdsAndAssets.TryGetValue(terrain, out Dictionary<string, TerrainContentAsset> idsToAssets) && 
                idsToAssets.TryGetValue(blueprintId, out terrainContentAsset);
        }

        protected override void Init()
        {
            base.Init();
            this.terrainContentAssetMap = new Dictionary<TerrainContentHeight, Dictionary<TileTerrain, Dictionary<string, TerrainContentAsset>>>();
            foreach (TerrainContentAsset terrainContentAsset in this.terrainContentAssets)
            {
                if (!this.terrainContentAssetMap.ContainsKey(terrainContentAsset.Height))
                {
                    this.terrainContentAssetMap.Add(terrainContentAsset.Height, new Dictionary<TileTerrain, Dictionary<string, TerrainContentAsset>>());
                }
                foreach (TileTerrain terrain in terrainContentAsset.Terrains)
                {
                    if (!this.terrainContentAssetMap[terrainContentAsset.Height].ContainsKey(terrain))
                    {
                        this.terrainContentAssetMap[terrainContentAsset.Height].Add(terrain, new Dictionary<string, TerrainContentAsset>());
                    }
                    if (this.terrainContentAssetMap[terrainContentAsset.Height][terrain].ContainsKey(terrainContentAsset.BlueprintID))
                    {
                        Debug.LogError(this.name + " TerrainContentAssetGroup has duplicate ID " + terrainContentAsset.BlueprintID);
                        continue;
                    }
                    this.terrainContentAssetMap[terrainContentAsset.Height][terrain].Add(terrainContentAsset.BlueprintID, terrainContentAsset);
                }
            }
        }
    }
}
