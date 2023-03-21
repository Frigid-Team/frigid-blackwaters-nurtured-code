using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledAreaBlueprint", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TiledAreaBlueprint")]
    public class TiledAreaBlueprint : FrigidScriptableObject
    {
        [SerializeField]
        private Vector2Int mainAreaDimensions;
        [SerializeField]
        private List<TerrainTileAsset> terrainTileAssets;
        [SerializeField]
        private WallTileAsset wallTileAsset;
        [SerializeField]
        private Nested2DList<TerrainContentAsset> terrainContentAssets;
        [SerializeField]
        private Nested2DList<Vector2> terrainContentOrientationDirections;
        [SerializeField]
        private Nested2DList<WallContentAsset> wallContentAssets;
        [SerializeField]
        private Nested2DList<Vector2> wallContentOrientationDirections;
        [SerializeField]
        private TileTerrain[] entranceTerrains;
        [SerializeField]
        private List<TiledAreaMobGenerationPreset> mobGenerationPresets;

        public void Setup(Vector2Int mainAreaDimensions)
        {
            this.mainAreaDimensions = mainAreaDimensions;
            this.terrainTileAssets = new List<TerrainTileAsset>(new TerrainTileAsset[mainAreaDimensions.x * mainAreaDimensions.y]);
            this.wallTileAsset = null;
            this.terrainContentAssets = new Nested2DList<TerrainContentAsset>();
            for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
            {
                this.terrainContentAssets.Add(new Nested1DList<TerrainContentAsset>(new TerrainContentAsset[mainAreaDimensions.x * mainAreaDimensions.y]));
            }
            this.wallContentAssets = new Nested2DList<WallContentAsset>();
            this.wallContentAssets.Add(new Nested1DList<WallContentAsset>(new WallContentAsset[this.mainAreaDimensions.y]));
            this.wallContentAssets.Add(new Nested1DList<WallContentAsset>(new WallContentAsset[this.mainAreaDimensions.x]));
            this.wallContentAssets.Add(new Nested1DList<WallContentAsset>(new WallContentAsset[this.mainAreaDimensions.y]));
            this.wallContentAssets.Add(new Nested1DList<WallContentAsset>(new WallContentAsset[this.mainAreaDimensions.x]));
            this.entranceTerrains = new TileTerrain[4];
            this.mobGenerationPresets = new List<TiledAreaMobGenerationPreset>();
        }

        public Vector2Int MainAreaDimensions
        {
            get
            {
                return this.mainAreaDimensions;
            }
        }

        public Vector2Int WallAreaDimensions
        {
            get
            {
                return new Vector2Int(this.mainAreaDimensions.x + WallsPopulator.MAX_WALL_DEPTH * 2, this.mainAreaDimensions.y + WallsPopulator.MAX_WALL_DEPTH * 2);
            }
        }

        public TileTerrain[] EntranceTerrains
        {
            get
            {
                return this.entranceTerrains;
            }
        }

        public List<TiledAreaMobGenerationPreset> MobGenerationPresets
        {
            get
            {
                return this.mobGenerationPresets;
            }
        }

        public TerrainTileAsset GetTerrainTileAssetAt(Vector2Int tileIndices)
        {
            return this.terrainTileAssets[tileIndices.y * this.mainAreaDimensions.x + tileIndices.x];
        }

        public void SetTerrainTileAssetAt(Vector2Int tileIndices, TerrainTileAsset terrainTileAsset)
        {
            this.terrainTileAssets[tileIndices.y * this.mainAreaDimensions.x + tileIndices.x] = terrainTileAsset;
        }

        public TerrainContentAsset GetTerrainContentAssetAt(TerrainContentHeight height, Vector2Int tileIndices)
        {
            return this.terrainContentAssets[(int)height][tileIndices.y * this.mainAreaDimensions.x + tileIndices.x];
        }

        public void SetTerrainContentAssetAt(TerrainContentHeight height, Vector2Int tileIndices, TerrainContentAsset terrainContentAsset)
        {
            this.terrainContentAssets[(int)height][tileIndices.y * this.mainAreaDimensions.x + tileIndices.x] = terrainContentAsset;
        }

        public Vector2 GetTerrainContentOrientationDirectionAt(Vector2Int tileIndices, TerrainContentHeight height)
        {
            return this.terrainContentOrientationDirections[(int)height][tileIndices.y * this.mainAreaDimensions.x + tileIndices.x];
        }

        public void SetTerrainContentOrientationDirectionAt(Vector2Int tileIndices, TerrainContentHeight height, Vector2 orientationDirection)
        {
            this.terrainContentOrientationDirections[(int)height][tileIndices.y * this.mainAreaDimensions.x + tileIndices.x] = orientationDirection;
        }

        public WallTileAsset GetWallTileAsset()
        {
            return this.wallTileAsset;
        }

        public void SetWallTileAsset(WallTileAsset wallTileAsset)
        {
            this.wallTileAsset = wallTileAsset;
        }

        public WallContentAsset GetWallContentAssetAt(Vector2Int wallDirection, int tileIndex)
        {
            return this.wallContentAssets[TilePositioning.WallArrayIndex(wallDirection)][tileIndex];
        }

        public void SetWallContentAssetAt(Vector2Int wallDirection, int tileIndex, WallContentAsset wallContentAsset)
        {
            this.wallContentAssets[TilePositioning.WallArrayIndex(wallDirection)][tileIndex] = wallContentAsset;
        }

        public Vector2 GetWallContentOrientationDirectionAt(Vector2Int wallDirection, int tileIndex)
        {
            return this.wallContentOrientationDirections[TilePositioning.WallArrayIndex(wallDirection)][tileIndex];
        }

        public void SetWallContentOrientationDirectionAt(Vector2Int wallDirection, int tileIndex, Vector2 orientationDirection)
        {
            this.wallContentOrientationDirections[TilePositioning.WallArrayIndex(wallDirection)][tileIndex] = orientationDirection;
        }

        public TileTerrain GetEntranceTerrain(Vector2Int wallDirection)
        {
            return this.entranceTerrains[TilePositioning.WallArrayIndex(wallDirection)];
        }

        public void SetEntranceTerrain(TileTerrain terrain, Vector2Int wallDirection)
        {
            this.entranceTerrains[TilePositioning.WallArrayIndex(wallDirection)] = terrain;
        }
    }
}
