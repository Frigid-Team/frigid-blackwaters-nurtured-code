using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "WallTileAsset", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "WallTileAsset")]
    public class WallTileAsset : FrigidScriptableObject
    {
        [SerializeField]
        [Range(0, TiledArea.MaxWallDepth)]
        private int depth;
        [SerializeField]
        private List<WallTile> wallTilePrefabs;
        [SerializeField]
        private List<WallTerrainBoundary> terrainBoundaries;

        private Dictionary<TerrainTileAsset, WallBoundaryTile> boundaryMap;

        public int Depth
        {
            get
            {
                return this.depth;
            }
        }

        public WallTile GetWallTilePrefab(int depth)
        {
            Debug.Assert(depth >= 1 && depth <= this.depth, "Wall tile depth is not within bounds!");
            return this.wallTilePrefabs[depth - 1];
        }

        public bool TryGetWallBoundaryTilePrefab(TerrainTileAsset terrainTileAsset, out WallBoundaryTile wallBoundaryTilePrefab)
        {
            return this.boundaryMap.TryGetValue(terrainTileAsset, out wallBoundaryTilePrefab);
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.boundaryMap = new Dictionary<TerrainTileAsset, WallBoundaryTile>();
            foreach (WallTerrainBoundary terrainBoundary in this.terrainBoundaries)
            {
                foreach (TerrainTileAsset boundaryTerrainTileAsset in terrainBoundary.BoundaryTerrainTileAssets)
                {
                    if (this.boundaryMap.ContainsKey(boundaryTerrainTileAsset))
                    {
                        Debug.LogWarning("WallTileAsset " + this.name + " has a duplicate boundary TerrainTileAsset: " + boundaryTerrainTileAsset.name + ".");
                        continue;
                    }
                    this.boundaryMap.Add(boundaryTerrainTileAsset, terrainBoundary.WallBoundaryTilePrefab);
                }
            }
        }

        [Serializable]
        private struct WallTerrainBoundary
        {
            [SerializeField]
            private List<TerrainTileAsset> boundaryTerrainTileAssets;
            [SerializeField]
            private WallBoundaryTile wallBoundaryTilePrefab;

            public List<TerrainTileAsset> BoundaryTerrainTileAssets
            {
                get
                {
                    return this.boundaryTerrainTileAssets;
                }
            }

            public WallBoundaryTile WallBoundaryTilePrefab
            {
                get
                {
                    return this.wallBoundaryTilePrefab;
                }
            }
        }
    }
}
