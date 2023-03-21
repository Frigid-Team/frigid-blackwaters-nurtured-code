using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "WallTileAsset", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "WallTileAsset")]
    public class WallTileAsset : FrigidScriptableObject
    {
        [SerializeField]
        [Range(0, WallsPopulator.MAX_WALL_DEPTH)]
        private int depth;
        [SerializeField]
        private List<WallTile> wallTilePrefabs;
        [SerializeField]
        private List<WallTerrainBoundary> terrainBoundaries;

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

        public bool TryGetBoundaryTilePrefab(TerrainTileAsset nearestTerrainTileAsset, out WallTile wallBoundaryTilePrefab)
        {
            foreach(WallTerrainBoundary wallTerrainBoundary in this.terrainBoundaries)
            {
                if(wallTerrainBoundary.BoundaryTerrainTileAssets.Contains(nearestTerrainTileAsset))
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
            private List<TerrainTileAsset> boundaryTerrainTileAssets;
            [SerializeField]
            private WallTile wallBoundaryTilePrefab;

            public List<TerrainTileAsset> BoundaryTerrainTileAssets
            {
                get
                {
                    return this.boundaryTerrainTileAssets;
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
