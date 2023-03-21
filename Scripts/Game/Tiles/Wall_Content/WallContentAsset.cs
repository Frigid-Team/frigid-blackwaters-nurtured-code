using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "WallContentAsset", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "WallContentAsset")]
    public class WallContentAsset : FrigidScriptableObject
    {
        private const string NONE = "None";

        [SerializeField]
        [ShowIfProperty("IsNone", false)]
        private List<WallContent> wallContentPrefabs;

        public bool IsNone
        {
            get
            {
                return this.name.Equals(NONE);
            }
        }

        public List<WallContent> WallContentPrefabs
        {
            get
            {
                return this.wallContentPrefabs;
            }
        }
    }
}
