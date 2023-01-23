using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "WallTileAsset", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "WallTileAsset")]
    public class WallTileAsset : FrigidScriptableObject
    {
        [SerializeField]
        private string blueprintId;
        [SerializeField]
        [Range(0, WallsPopulator.MAX_WALL_DEPTH)]
        private int depth;
        [SerializeField]
        private List<WallTile> wallTilePrefabs;
        [SerializeField]
        private List<WallTerrainBoundary> terrainBoundaries;

        public string BlueprintID
        {
            get
            {
                return this.blueprintId;
            }
        }

        public int Depth
        {
            get
            {
                return this.depth;
            }
        }

        public WallTile GetWallTilePrefab(int depth)
        {
            if (depth >= 1 && depth <= this.depth) 
            {
                return this.wallTilePrefabs[depth - 1];
            }
            throw new Exception("Wall tile depth is not within bounds!");
        }

        public bool TryGetBoundaryTilePrefab(string nearestBlueprintId, out WallTile wallBoundaryTilePrefab)
        {
            foreach(WallTerrainBoundary wallTerrainBoundary in this.terrainBoundaries)
            {
                if(wallTerrainBoundary.BoundaryBlueprintIDs.Contains(nearestBlueprintId))
                {
                    wallBoundaryTilePrefab = wallTerrainBoundary.WallBoundaryTilePrefab;
                    return true;
                }
            }
            wallBoundaryTilePrefab = null;
            return false;
        }

        [Serializable]
        public struct WallTerrainBoundary
        {
            [SerializeField]
            private List<string> boundaryBlueprintIds;
            [SerializeField]
            private WallTile wallBoundaryTilePrefab;

            public List<string> BoundaryBlueprintIDs
            {
                get
                {
                    return this.boundaryBlueprintIds;
                }
            }

            public WallTile WallBoundaryTilePrefab
            {
                get
                {
                    return this.wallBoundaryTilePrefab;
                }
            }
        }
    }
}
