using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TerrainContentAsset", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TerrainContentAsset")]
    public class TerrainContentAsset : FrigidScriptableObject
    {
        [SerializeField]
        private string blueprintId;
        [SerializeField]
        private List<TerrainContent> terrainContentPrefabs;
        [SerializeField]
        private TerrainContentHeight height;
        [SerializeField]
        private List<TileTerrain> terrains;

        public string BlueprintID
        {
            get
            {
                return this.blueprintId;
            }
        }

        public List<TerrainContent> TerrainContentPrefabs
        {
            get
            {
                return this.terrainContentPrefabs;
            }
        }

        public TerrainContentHeight Height
        {
            get
            {
                return this.height;
            }
        }

        public List<TileTerrain> Terrains
        {
            get
            {
                return this.terrains;
            }
        }
    }
}
