using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "WallContentAsset", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "WallContentAsset")]
    public class WallContentAsset : FrigidScriptableObject
    {
        [SerializeField]
        private string blueprintId;
        [SerializeField]
        private List<WallContent> wallContentPrefabs;
        [SerializeField]
        private List<TileTerrain> terrains;

        public string BlueprintID
        {
            get
            {
                return this.blueprintId;
            }
        }

        public List<WallContent> WallContentPrefabs
        {
            get
            {
                return this.wallContentPrefabs;
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
