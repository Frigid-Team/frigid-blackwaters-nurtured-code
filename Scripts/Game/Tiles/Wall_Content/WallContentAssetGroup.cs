using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "WallContentAssetGroup", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "WallContentAssetGroup")]
    public class WallContentAssetGroup : FrigidScriptableObject
    {
        [SerializeField]
        private List<WallContentAsset> wallContentAssets;

        private Dictionary<TileTerrain, Dictionary<string, WallContentAsset>> wallContentAssetMap;

        public List<WallContentAsset> WallContentAssets
        {
            get
            {
                return this.wallContentAssets;
            }
        }

        public bool TryGetWallContentAsset(TileTerrain terrain, string blueprintId, out WallContentAsset wallContentAsset)
        {
            wallContentAsset = null;
            return this.wallContentAssetMap.TryGetValue(terrain, out Dictionary<string, WallContentAsset> idsToAssets) && idsToAssets.TryGetValue(blueprintId, out wallContentAsset);
        }

        protected override void Init()
        {
            base.Init();
            this.wallContentAssetMap = new Dictionary<TileTerrain, Dictionary<string, WallContentAsset>>();
            foreach (WallContentAsset wallContentAsset in this.wallContentAssets)
            {
                foreach (TileTerrain terrain in wallContentAsset.Terrains)
                {
                    if (!this.wallContentAssetMap.ContainsKey(terrain))
                    {
                        this.wallContentAssetMap.Add(terrain, new Dictionary<string, WallContentAsset>());
                    }

                    if (this.wallContentAssetMap[terrain].ContainsKey(wallContentAsset.BlueprintID))
                    {
                        Debug.LogError(this.name + " WallContentAssetGroup has duplicate ID " + wallContentAsset.BlueprintID);
                        continue;
                    }

                    this.wallContentAssetMap[terrain].Add(wallContentAsset.BlueprintID, wallContentAsset);
                }
            }
        }
    }
}
