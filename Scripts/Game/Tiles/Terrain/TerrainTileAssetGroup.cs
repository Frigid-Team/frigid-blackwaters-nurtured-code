using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TerrainTileAssetGroup", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TerrainTileAssetGroup")]
    public class TerrainTileAssetGroup : FrigidScriptableObject
    {
        [SerializeField]
        private List<TerrainTileAsset> terrainTileAssets;

        private Dictionary<TileTerrain, Dictionary<string, TerrainTileAsset>> terrainTileAssetMap;

        public List<TerrainTileAsset> TerrainTileAssets
        {
            get
            {
                return this.terrainTileAssets;
            }
        }

        public bool TryGetTerrainTileAsset(TileTerrain terrain, string blueprintId, out TerrainTileAsset terrainTileAsset)
        {
            terrainTileAsset = null;
            return this.terrainTileAssetMap.TryGetValue(terrain, out Dictionary<string, TerrainTileAsset> idsToAssets) && idsToAssets.TryGetValue(blueprintId, out terrainTileAsset);
        }

        protected override void Init()
        {
            base.Init();
            this.terrainTileAssetMap = new Dictionary<TileTerrain, Dictionary<string, TerrainTileAsset>>();
            foreach (TerrainTileAsset terrainTileAsset in this.terrainTileAssets)
            {
                if (!this.terrainTileAssetMap.ContainsKey(terrainTileAsset.Terrain))
                {
                    this.terrainTileAssetMap.Add(terrainTileAsset.Terrain, new Dictionary<string, TerrainTileAsset>());
                }

                if (this.terrainTileAssetMap[terrainTileAsset.Terrain].ContainsKey(terrainTileAsset.BlueprintID))
                {
                    Debug.LogError(this.name + " terrain tile asset group has duplicate ID " + terrainTileAsset.BlueprintID);
                    continue;
                }
                this.terrainTileAssetMap[terrainTileAsset.Terrain].Add(terrainTileAsset.BlueprintID, terrainTileAsset);
            }
        }
    }
}
