using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TerrainContentAsset", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TerrainContentAsset")]
    public class TerrainContentAsset : FrigidScriptableObject
    {
        private const string NONE = "None";

        [SerializeField]
        [ShowIfProperty("IsNone", false)]
        private List<TerrainContent> terrainContentPrefabs;

        public bool IsNone
        {
            get
            {
                return this.name.Equals(NONE);
            }
        }

        public List<TerrainContent> TerrainContentPrefabs
        {
            get
            {
                return this.terrainContentPrefabs;
            }
        }
    }
}
