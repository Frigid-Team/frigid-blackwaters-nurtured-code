using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "WallTileAssetGroup", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "WallTileAssetGroup")]
    public class WallTileAssetGroup : FrigidScriptableObject
    {
        [SerializeField]
        private List<WallTileAsset> wallTileAssets;

        private Dictionary<string, WallTileAsset> wallTileAssetMap;

        public List<WallTileAsset> WallTileAssets
        {
            get
            {
                return this.wallTileAssets;
            }
        }

        public bool TryGetAsset(string blueprintId, out WallTileAsset wallTileAsset)
        {
            return this.wallTileAssetMap.TryGetValue(blueprintId, out wallTileAsset);
        }

        protected override void Init()
        {
            base.Init();
            this.wallTileAssetMap = new Dictionary<string, WallTileAsset>();
            foreach (WallTileAsset wallTileAsset in this.wallTileAssets)
            {
                if (this.wallTileAssetMap.ContainsKey(wallTileAsset.BlueprintID))
                {
                    Debug.LogError(this.name + " wall tile asset group has duplicate ID " + wallTileAsset.BlueprintID);
                    continue;
                }
                this.wallTileAssetMap.Add(wallTileAsset.BlueprintID, wallTileAsset);
            }
        }
    }
}
