using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledAreaBlueprint", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TiledAreaBlueprint")]
    public class TiledAreaBlueprint : FrigidScriptableObject
    {
        [SerializeField]
        private Vector2Int mainAreaDimensions;
        [SerializeField]
        private TileTerrain[] terrains;
        [SerializeField]
        private string[] terrainTileIds;
        [SerializeField]
        private string wallTileId;
        [SerializeField]
        private TerrainContentIDsPerHeight[] terrainContentIdsPerHeight;
        [SerializeField]
        private TerrainContentOrientationsPerHeight[] terrainContentOrientationsPerHeight;
        [SerializeField]
        private WallContentIDsPerWall[] wallContentIdsPerWall;
        [SerializeField]
        private WallContentOrientationsPerWall[] wallContentOrientationsPerWall;
        [SerializeField]
        private TileTerrain[] entranceTerrains;
        [SerializeField]
        private List<TiledAreaMobGenerationPreset> mobGenerationPresets;

        public void Setup(Vector2Int mainAreaDimensions)
        {
            this.mainAreaDimensions = mainAreaDimensions;
            this.terrains = new TileTerrain[mainAreaDimensions.x * mainAreaDimensions.y];
            this.terrainTileIds = new string[mainAreaDimensions.x * mainAreaDimensions.y];
            this.terrainContentIdsPerHeight = new TerrainContentIDsPerHeight[(int)TerrainContentHeight.Count];
            this.terrainContentOrientationsPerHeight = new TerrainContentOrientationsPerHeight[(int)TerrainContentHeight.Count];
            for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
            {
                this.terrainContentIdsPerHeight[i] = new TerrainContentIDsPerHeight(mainAreaDimensions);
                this.terrainContentOrientationsPerHeight[i] = new TerrainContentOrientationsPerHeight(mainAreaDimensions);
            }
            this.wallContentIdsPerWall = new WallContentIDsPerWall[4];
            this.wallContentOrientationsPerWall = new WallContentOrientationsPerWall[4];
            this.wallContentIdsPerWall[0] = new WallContentIDsPerWall(this.mainAreaDimensions.y);
            this.wallContentOrientationsPerWall[0] = new WallContentOrientationsPerWall(this.mainAreaDimensions.y);
            this.wallContentIdsPerWall[1] = new WallContentIDsPerWall(this.mainAreaDimensions.x);
            this.wallContentOrientationsPerWall[1] = new WallContentOrientationsPerWall(this.mainAreaDimensions.x);
            this.wallContentIdsPerWall[2] = new WallContentIDsPerWall(this.mainAreaDimensions.y);
            this.wallContentOrientationsPerWall[2] = new WallContentOrientationsPerWall(this.mainAreaDimensions.y);
            this.wallContentIdsPerWall[3] = new WallContentIDsPerWall(this.mainAreaDimensions.x);
            this.wallContentOrientationsPerWall[3] = new WallContentOrientationsPerWall(this.mainAreaDimensions.x);
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

        public string WallTileID
        {
            get
            {
                return this.wallTileId;
            }
            set
            {
                this.wallTileId = value;
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

        public TileTerrain GetTerrainAtTile(Vector2Int positionIndices)
        {
            return this.terrains[positionIndices.y * this.mainAreaDimensions.x + positionIndices.x];
        }

        public void SetTerrainAtTile(Vector2Int positionIndices, TileTerrain newTerrain)
        {
            this.terrains[positionIndices.y * this.mainAreaDimensions.x + positionIndices.x] = newTerrain;
        }

        public string GetTerrainTileIDAtTile(Vector2Int positionIndices)
        {
            return this.terrainTileIds[positionIndices.y * this.mainAreaDimensions.x + positionIndices.x];
        }

        public void SetTerrainTileIDAtTile(Vector2Int positionIndices, string newId)
        {
            this.terrainTileIds[positionIndices.y * this.mainAreaDimensions.x + positionIndices.x] = newId;
        }

        public string GetTerrainContentIDAtTile(Vector2Int positionIndices, TerrainContentHeight height)
        {
            return this.terrainContentIdsPerHeight[(int)height].TerrainContentIds[positionIndices.y * this.mainAreaDimensions.x + positionIndices.x];
        }

        public void SetTerrainContentIDAtTile(Vector2Int positionIndices, TerrainContentHeight height, string newId)
        {
            this.terrainContentIdsPerHeight[(int)height].TerrainContentIds[positionIndices.y * this.mainAreaDimensions.x + positionIndices.x] = newId;
        }

        public Vector2 GetTerrainContentOrientationDirectionAtTile(Vector2Int positionIndices, TerrainContentHeight height)
        {
            return this.terrainContentOrientationsPerHeight[(int)height].TerrainContentOrientationDirections[positionIndices.y * this.mainAreaDimensions.x + positionIndices.x];
        }

        public void SetTerrainContentOrientationDirectionAtTile(Vector2Int positionIndices, TerrainContentHeight height, Vector2 orientationDirection)
        {
            this.terrainContentOrientationsPerHeight[(int)height].TerrainContentOrientationDirections[positionIndices.y * this.mainAreaDimensions.x + positionIndices.x] = orientationDirection;
        }

        public string GetWallContentIDAtTile(Vector2Int wallDirection, int tileIndex)
        {
            return this.wallContentIdsPerWall[TilePositioning.WallArrayIndex(wallDirection)].WallContentIDs[tileIndex];
        }

        public void SetWallContentIDAtTile(Vector2Int wallDirection, int tileIndex, string newId)
        {
            this.wallContentIdsPerWall[TilePositioning.WallArrayIndex(wallDirection)].WallContentIDs[tileIndex] = newId;
        }

        public Vector2 GetWallContentOrientationDirectionAtTile(Vector2Int wallDirection, int tileIndex)
        {
            return this.wallContentOrientationsPerWall[TilePositioning.WallArrayIndex(wallDirection)].WallContentOrientationDirections[tileIndex];
        }

        public void SetWallContentOrientationDirectionAtTile(Vector2Int wallDirection, int tileIndex, Vector2 orientationDirection)
        {
            this.wallContentOrientationsPerWall[TilePositioning.WallArrayIndex(wallDirection)].WallContentOrientationDirections[tileIndex] = orientationDirection;
        }

        public TileTerrain GetEntranceTerrain(Vector2Int wallDirection)
        {
            return this.entranceTerrains[TilePositioning.WallArrayIndex(wallDirection)];
        }

        public void SetEntranceTerrain(TileTerrain terrain, Vector2Int wallDirection)
        {
            this.entranceTerrains[TilePositioning.WallArrayIndex(wallDirection)] = terrain;
        }

        [Serializable]
        private struct TerrainContentIDsPerHeight
        {
            [SerializeField]
            private string[] terrainContentIds;

            public TerrainContentIDsPerHeight(Vector2Int dimensions)
            {
                this.terrainContentIds = new string[dimensions.x * dimensions.y];
            }

            public string[] TerrainContentIds
            {
                get
                {
                    return this.terrainContentIds;
                }
            }
        }


        [Serializable]
        private struct TerrainContentOrientationsPerHeight
        {
            [SerializeField]
            private Vector2[] terrainContentOrientationDirections;

            public TerrainContentOrientationsPerHeight(Vector2Int dimensions)
            {
                this.terrainContentOrientationDirections = new Vector2[dimensions.x * dimensions.y];
            }

            public Vector2[] TerrainContentOrientationDirections
            {
                get
                {
                    return this.terrainContentOrientationDirections;
                }
            }
        }

        [Serializable]
        private struct WallContentIDsPerWall
        {
            [SerializeField]
            private string[] wallContentIds;

            public WallContentIDsPerWall(int wallLength)
            {
                this.wallContentIds = new string[wallLength];
            }
            
            public string[] WallContentIDs
            {
                get
                {
                    return this.wallContentIds;
                }
            }
        }

        [Serializable]
        private struct WallContentOrientationsPerWall
        {
            [SerializeField]
            private Vector2[] wallContentOrientationDirections;

            public WallContentOrientationsPerWall(int wallLength)
            {
                this.wallContentOrientationDirections = new Vector2[wallLength];
            }

            public Vector2[] WallContentOrientationDirections
            {
                get
                {
                    return this.wallContentOrientationDirections;
                }
            }
        }
    }
}
