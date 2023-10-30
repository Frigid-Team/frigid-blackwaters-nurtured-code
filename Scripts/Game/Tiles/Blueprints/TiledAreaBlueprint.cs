using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledAreaBlueprint", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "TiledAreaBlueprint")]
    public class TiledAreaBlueprint : FrigidScriptableObject
    {
        [SerializeField]
        [ReadOnly]
        private TiledArea areaPrefab;
        [SerializeField]
        [ReadOnly]
        private TiledEntranceAsset[] wallEntranceAssets;
        [SerializeField]
        [ReadOnly]
        private int[] wallEntranceTileIndexes;
        [SerializeField]
        [ReadOnly]
        private int[] wallEntranceWidths;
        [SerializeField]
        [ReadOnly]
        private Vector2Int mainAreaDimensions;
        [SerializeField]
        [ReadOnly]
        private bool hasVisibleWalls;
        [SerializeField]
        [ReadOnly]
        private List<TerrainTileAsset> terrainTileAssets;
        [SerializeField]
        [ReadOnly]
        private Nested2DList<WallTileAsset> wallTileAssets;
        [SerializeField]
        [ReadOnly]
        private Nested2DList<TerrainContentAsset> terrainContentAssets;
        [SerializeField]
        [ReadOnly]
        private Nested2DList<Vector2> terrainContentOrientationDirections;
        [SerializeField]
        [ReadOnly]
        private Nested2DList<WallContentAsset> wallContentAssets;
        [SerializeField]
        [ReadOnly]
        private Nested2DList<Vector2> wallContentOrientationDirections;
        [SerializeField]
        [ReadOnly]
        private List<TiledAreaMobSpawnerSerializedReference> mobSpawners;
        [SerializeField]
        [ReadOnly]
        private Nested2DList<TiledAreaMobSpawnPoint> mobSpawnPoints;

#if UNITY_EDITOR
        public static TiledAreaBlueprint CreateEmpty(string assetPath, TiledArea areaPrefab, Vector2Int mainAreaDimensions, TerrainTileAsset terrainTileAsset, WallTileAsset wallTileAsset)
        {
            Debug.Assert(terrainTileAsset != null && wallTileAsset != null, "Tried to create empty TiledAreaBlueprint with null assets.");
            Debug.Assert(mainAreaDimensions.x > 0 && mainAreaDimensions.y > 0, "Tried to create empty TiledAreaBlueprint with invalid dimensions.");

            TiledAreaBlueprint emptyBlueprint = CreateInstance<TiledAreaBlueprint>();
            emptyBlueprint.areaPrefab = areaPrefab;
            emptyBlueprint.wallEntranceWidths = Enumerable.Repeat<int>(-1, 4).ToArray();
            emptyBlueprint.wallEntranceTileIndexes = Enumerable.Repeat<int>(-1, 4).ToArray();
            emptyBlueprint.wallEntranceAssets = Enumerable.Repeat<TiledEntranceAsset>(null, 4).ToArray();
            emptyBlueprint.mainAreaDimensions = mainAreaDimensions;
            emptyBlueprint.hasVisibleWalls = true;
            emptyBlueprint.terrainTileAssets = new List<TerrainTileAsset>(Enumerable.Repeat<TerrainTileAsset>(terrainTileAsset, mainAreaDimensions.x * mainAreaDimensions.y).ToArray());
            emptyBlueprint.wallTileAssets = new Nested2DList<WallTileAsset>();
            emptyBlueprint.wallTileAssets.Add(new Nested1DList<WallTileAsset>(Enumerable.Repeat<WallTileAsset>(wallTileAsset, mainAreaDimensions.y)));
            emptyBlueprint.wallTileAssets.Add(new Nested1DList<WallTileAsset>(Enumerable.Repeat<WallTileAsset>(wallTileAsset, mainAreaDimensions.x)));
            emptyBlueprint.wallTileAssets.Add(new Nested1DList<WallTileAsset>(Enumerable.Repeat<WallTileAsset>(wallTileAsset, mainAreaDimensions.y)));
            emptyBlueprint.wallTileAssets.Add(new Nested1DList<WallTileAsset>(Enumerable.Repeat<WallTileAsset>(wallTileAsset, mainAreaDimensions.x)));
            emptyBlueprint.terrainContentAssets = new Nested2DList<TerrainContentAsset>();
            emptyBlueprint.terrainContentOrientationDirections = new Nested2DList<Vector2>();
            for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
            {
                emptyBlueprint.terrainContentAssets.Add(new Nested1DList<TerrainContentAsset>(Enumerable.Repeat<TerrainContentAsset>(null, mainAreaDimensions.x * mainAreaDimensions.y).ToArray()));
                emptyBlueprint.terrainContentOrientationDirections.Add(new Nested1DList<Vector2>(Enumerable.Repeat(Vector2.zero, mainAreaDimensions.x * mainAreaDimensions.y).ToArray()));
            }
            emptyBlueprint.wallContentAssets = new Nested2DList<WallContentAsset>();
            emptyBlueprint.wallContentAssets.Add(new Nested1DList<WallContentAsset>(Enumerable.Repeat<WallContentAsset>(null, mainAreaDimensions.y).ToArray()));
            emptyBlueprint.wallContentAssets.Add(new Nested1DList<WallContentAsset>(Enumerable.Repeat<WallContentAsset>(null, mainAreaDimensions.x).ToArray()));
            emptyBlueprint.wallContentAssets.Add(new Nested1DList<WallContentAsset>(Enumerable.Repeat<WallContentAsset>(null, mainAreaDimensions.y).ToArray()));
            emptyBlueprint.wallContentAssets.Add(new Nested1DList<WallContentAsset>(Enumerable.Repeat<WallContentAsset>(null, mainAreaDimensions.x).ToArray()));
            emptyBlueprint.wallContentOrientationDirections = new Nested2DList<Vector2>();
            emptyBlueprint.wallContentOrientationDirections.Add(new Nested1DList<Vector2>(Enumerable.Repeat(Vector2.zero, mainAreaDimensions.y).ToArray()));
            emptyBlueprint.wallContentOrientationDirections.Add(new Nested1DList<Vector2>(Enumerable.Repeat(Vector2.zero, mainAreaDimensions.x).ToArray()));
            emptyBlueprint.wallContentOrientationDirections.Add(new Nested1DList<Vector2>(Enumerable.Repeat(Vector2.zero, mainAreaDimensions.y).ToArray()));
            emptyBlueprint.wallContentOrientationDirections.Add(new Nested1DList<Vector2>(Enumerable.Repeat(Vector2.zero, mainAreaDimensions.x).ToArray()));
            emptyBlueprint.mobSpawners = new List<TiledAreaMobSpawnerSerializedReference>();
            emptyBlueprint.mobSpawnPoints = new Nested2DList<TiledAreaMobSpawnPoint>();
            AssetDatabase.CreateAsset(emptyBlueprint, assetPath);
            return emptyBlueprint;
        }
#endif

        public TiledArea AreaPrefab
        {
            get
            {
                return this.areaPrefab;
            }
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
                return new Vector2Int(this.mainAreaDimensions.x + TiledArea.MaxWallDepth * 2, this.mainAreaDimensions.y + TiledArea.MaxWallDepth * 2);
            }
        }

        public bool HasVisibleWalls
        {
            get
            {
                return this.hasVisibleWalls;
            }
            set
            {
                if (this.hasVisibleWalls != value)
                {
                    FrigidEdit.RecordChanges(this);
                    this.hasVisibleWalls = value;
                }
            }
        }

        public void Expand(Vector2Int wallIndexDirection)
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");

            Vector2Int newMainAreaDimensions = this.mainAreaDimensions + new Vector2Int(Mathf.Abs(wallIndexDirection.x), Mathf.Abs(wallIndexDirection.y));
            if (newMainAreaDimensions.x < 1 || newMainAreaDimensions.y < 1 ||
                newMainAreaDimensions.x < Mathf.Max(this.wallEntranceWidths[1], this.wallEntranceWidths[3]) || newMainAreaDimensions.y < Mathf.Max(this.wallEntranceWidths[0], this.wallEntranceWidths[2]))
            {
                return;
            }

            FrigidEdit.RecordChanges(this);
            if (wallIndexDirection == Vector2Int.right)
            {
                for (int y = this.mainAreaDimensions.y -1; y >= 0; y--)
                {
                    int rightMostIndex = y * this.mainAreaDimensions.x + this.mainAreaDimensions.x - 1;
                    this.terrainTileAssets.Insert(rightMostIndex + 1, this.terrainTileAssets[rightMostIndex]);
                    for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                    {
                        this.terrainContentAssets[i].Insert(rightMostIndex + 1, null);
                        this.terrainContentOrientationDirections[i].Insert(rightMostIndex + 1, Vector2.zero);
                    }
                }
                this.wallTileAssets[1].Insert(this.mainAreaDimensions.x, this.wallTileAssets[1][this.mainAreaDimensions.x - 1]);
                this.wallTileAssets[3].Insert(0, this.wallTileAssets[3][0]);
                this.wallContentAssets[1].Insert(this.mainAreaDimensions.x, null);
                this.wallContentAssets[3].Insert(0, null);
                this.wallContentOrientationDirections[1].Insert(this.mainAreaDimensions.x, Vector2.zero);
                this.wallContentOrientationDirections[3].Insert(0, Vector2.zero);
                this.wallEntranceTileIndexes[3]++;
                this.wallEntranceTileIndexes[1] = Mathf.Clamp(this.wallEntranceTileIndexes[1], this.wallEntranceWidths[1] - 1, newMainAreaDimensions.x - 1);
                this.wallEntranceTileIndexes[3] = Mathf.Clamp(this.wallEntranceTileIndexes[3], this.wallEntranceWidths[3] - 1, newMainAreaDimensions.x - 1);
            }
            else if (wallIndexDirection == Vector2Int.up)
            {
                this.terrainTileAssets.InsertRange(0, this.terrainTileAssets.GetRange(0, this.mainAreaDimensions.x));
                for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                {
                    this.terrainContentAssets[i].InsertRange(0, Enumerable.Repeat<TerrainContentAsset>(null, this.mainAreaDimensions.x));
                    this.terrainContentOrientationDirections[i].InsertRange(0, Enumerable.Repeat<Vector2>(Vector2.zero, this.mainAreaDimensions.x));
                }
                this.wallTileAssets[0].Insert(0, this.wallTileAssets[0][0]);
                this.wallTileAssets[2].Insert(this.mainAreaDimensions.y, this.wallTileAssets[2][this.mainAreaDimensions.y - 1]);
                this.wallContentAssets[0].Insert(0, null);
                this.wallContentAssets[2].Insert(this.mainAreaDimensions.y, null);
                this.wallContentOrientationDirections[0].Insert(0, Vector2.zero);
                this.wallContentOrientationDirections[2].Insert(this.mainAreaDimensions.y, Vector2.zero);
                this.wallEntranceTileIndexes[0]++;
                this.wallEntranceTileIndexes[0] = Mathf.Clamp(this.wallEntranceTileIndexes[0], this.wallEntranceWidths[0] - 1, newMainAreaDimensions.y - 1);
                this.wallEntranceTileIndexes[2] = Mathf.Clamp(this.wallEntranceTileIndexes[2], this.wallEntranceWidths[2] - 1, newMainAreaDimensions.y - 1);
            }
            else if (wallIndexDirection == Vector2Int.left)
            {
                for (int y = this.mainAreaDimensions.y - 1; y >= 0; y--)
                {
                    int leftMostIndex = y * this.mainAreaDimensions.x;
                    this.terrainTileAssets.Insert(leftMostIndex, this.terrainTileAssets[leftMostIndex]);
                    for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                    {
                        this.terrainContentAssets[i].Insert(leftMostIndex, null);
                        this.terrainContentOrientationDirections[i].Insert(leftMostIndex, Vector2.zero);
                    }
                }
                this.wallTileAssets[1].Insert(0, this.wallTileAssets[1][0]);
                this.wallTileAssets[3].Insert(this.mainAreaDimensions.x, this.wallTileAssets[3][this.mainAreaDimensions.x - 1]);
                this.wallContentAssets[1].Insert(0, null);
                this.wallContentAssets[3].Insert(this.mainAreaDimensions.x, null);
                this.wallContentOrientationDirections[1].Insert(0, Vector2.zero);
                this.wallContentOrientationDirections[3].Insert(this.mainAreaDimensions.x, Vector2.zero);
                this.wallEntranceTileIndexes[1]++;
                this.wallEntranceTileIndexes[1] = Mathf.Clamp(this.wallEntranceTileIndexes[1], this.wallEntranceWidths[1] - 1, newMainAreaDimensions.x - 1);
                this.wallEntranceTileIndexes[3] = Mathf.Clamp(this.wallEntranceTileIndexes[3], this.wallEntranceWidths[3] - 1, newMainAreaDimensions.x - 1);
            }
            else
            {
                this.terrainTileAssets.InsertRange(this.mainAreaDimensions.y * this.mainAreaDimensions.x, this.terrainTileAssets.GetRange((this.mainAreaDimensions.y - 1) * this.mainAreaDimensions.x, this.mainAreaDimensions.x));
                for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                {
                    this.terrainContentAssets[i].InsertRange(this.mainAreaDimensions.y * this.mainAreaDimensions.x, Enumerable.Repeat<TerrainContentAsset>(null, this.mainAreaDimensions.x));
                    this.terrainContentOrientationDirections[i].InsertRange(this.mainAreaDimensions.y * this.mainAreaDimensions.x, Enumerable.Repeat<Vector2>(Vector2.zero, this.mainAreaDimensions.x));
                }
                this.wallTileAssets[0].Insert(this.mainAreaDimensions.y, this.wallTileAssets[0][this.mainAreaDimensions.y - 1]);
                this.wallTileAssets[2].Insert(0, this.wallTileAssets[2][0]);
                this.wallContentAssets[0].Insert(this.mainAreaDimensions.y, null);
                this.wallContentAssets[2].Insert(0, null);
                this.wallContentOrientationDirections[0].Insert(this.mainAreaDimensions.y, Vector2.zero);
                this.wallContentOrientationDirections[2].Insert(0, Vector2.zero);
                this.wallEntranceTileIndexes[2]++;
                this.wallEntranceTileIndexes[0] = Mathf.Clamp(this.wallEntranceTileIndexes[0], this.wallEntranceWidths[0] - 1, newMainAreaDimensions.y - 1);
                this.wallEntranceTileIndexes[2] = Mathf.Clamp(this.wallEntranceTileIndexes[2], this.wallEntranceWidths[2] - 1, newMainAreaDimensions.y - 1);
            }
            this.mainAreaDimensions = newMainAreaDimensions;
        }

        public void Shrink(Vector2Int wallIndexDirection)
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");

            Vector2Int newMainAreaDimensions = this.mainAreaDimensions - new Vector2Int(Mathf.Abs(wallIndexDirection.x), Mathf.Abs(wallIndexDirection.y));
            if (newMainAreaDimensions.x < 1 || newMainAreaDimensions.y < 1 || 
                newMainAreaDimensions.x < Mathf.Max(this.wallEntranceWidths[1], this.wallEntranceWidths[3]) || newMainAreaDimensions.y < Mathf.Max(this.wallEntranceWidths[0], this.wallEntranceWidths[2]))
            {
                return;
            }

            FrigidEdit.RecordChanges(this);
            if (wallIndexDirection == Vector2Int.right)
            {
                for (int y = this.mainAreaDimensions.y - 1; y >= 0; y--)
                {
                    int rightMostIndex = y * this.mainAreaDimensions.x + this.mainAreaDimensions.x - 1;
                    this.terrainTileAssets.RemoveAt(rightMostIndex);
                    for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                    {
                        this.terrainContentAssets[i].RemoveAt(rightMostIndex);
                        this.terrainContentOrientationDirections[i].RemoveAt(rightMostIndex);
                    }
                }
                this.wallTileAssets[1].RemoveAt(this.mainAreaDimensions.x - 1);
                this.wallTileAssets[3].RemoveAt(0);
                this.wallContentAssets[1].RemoveAt(this.mainAreaDimensions.x - 1);
                this.wallContentAssets[3].RemoveAt(0);
                this.wallContentOrientationDirections[1].RemoveAt(this.mainAreaDimensions.x - 1);
                this.wallContentOrientationDirections[3].RemoveAt(0);
                this.wallEntranceTileIndexes[3]--;
                this.wallEntranceTileIndexes[1] = Mathf.Clamp(this.wallEntranceTileIndexes[1], this.wallEntranceWidths[1] - 1, newMainAreaDimensions.x - 1);
                this.wallEntranceTileIndexes[3] = Mathf.Clamp(this.wallEntranceTileIndexes[3], this.wallEntranceWidths[3] - 1, newMainAreaDimensions.x - 1);
            }
            else if (wallIndexDirection == Vector2Int.up)
            {
                this.terrainTileAssets.RemoveRange(0, this.mainAreaDimensions.x);
                for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                {
                    this.terrainContentAssets[i].RemoveRange(0, this.mainAreaDimensions.x);
                    this.terrainContentOrientationDirections[i].RemoveRange(0, this.mainAreaDimensions.x);
                }
                this.wallTileAssets[0].RemoveAt(0);
                this.wallTileAssets[2].RemoveAt(this.mainAreaDimensions.y - 1);
                this.wallContentAssets[0].RemoveAt(0);
                this.wallContentAssets[2].RemoveAt(this.mainAreaDimensions.y - 1);
                this.wallContentOrientationDirections[0].RemoveAt(0);
                this.wallContentOrientationDirections[2].RemoveAt(this.mainAreaDimensions.y - 1);
                this.wallEntranceTileIndexes[0]--;
                this.wallEntranceTileIndexes[0] = Mathf.Clamp(this.wallEntranceTileIndexes[0], this.wallEntranceWidths[0] - 1, newMainAreaDimensions.y - 1);
                this.wallEntranceTileIndexes[2] = Mathf.Clamp(this.wallEntranceTileIndexes[2], this.wallEntranceWidths[2] - 1, newMainAreaDimensions.y - 1);
            }
            else if (wallIndexDirection == Vector2Int.left)
            {
                for (int y = this.mainAreaDimensions.y - 1; y >= 0; y--)
                {
                    int leftMostIndex = y * this.mainAreaDimensions.x;
                    this.terrainTileAssets.RemoveAt(leftMostIndex);
                    for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                    {
                        this.terrainContentAssets[i].RemoveAt(leftMostIndex);
                        this.terrainContentOrientationDirections[i].RemoveAt(leftMostIndex);
                    }
                }
                this.wallTileAssets[1].RemoveAt(0);
                this.wallTileAssets[3].RemoveAt(this.mainAreaDimensions.x - 1);
                this.wallContentAssets[1].RemoveAt(0);
                this.wallContentAssets[3].RemoveAt(this.mainAreaDimensions.x - 1);
                this.wallContentOrientationDirections[1].RemoveAt(0);
                this.wallContentOrientationDirections[3].RemoveAt(this.mainAreaDimensions.x - 1);
                this.wallEntranceTileIndexes[1]--;
                this.wallEntranceTileIndexes[1] = Mathf.Clamp(this.wallEntranceTileIndexes[1], this.wallEntranceWidths[1] - 1, newMainAreaDimensions.x - 1);
                this.wallEntranceTileIndexes[3] = Mathf.Clamp(this.wallEntranceTileIndexes[3], this.wallEntranceWidths[3] - 1, newMainAreaDimensions.x - 1);
            }
            else
            {
                this.terrainTileAssets.RemoveRange((this.mainAreaDimensions.y - 1) * this.mainAreaDimensions.x, this.mainAreaDimensions.x);
                for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                {
                    this.terrainContentAssets[i].RemoveRange((this.mainAreaDimensions.y - 1) * this.mainAreaDimensions.x, this.mainAreaDimensions.x);
                    this.terrainContentOrientationDirections[i].RemoveRange((this.mainAreaDimensions.y - 1) * this.mainAreaDimensions.x, this.mainAreaDimensions.x);
                }
                this.wallTileAssets[0].RemoveAt(this.mainAreaDimensions.y - 1);
                this.wallTileAssets[2].RemoveAt(0);
                this.wallContentAssets[0].RemoveAt(this.mainAreaDimensions.y - 1);
                this.wallContentAssets[2].RemoveAt(0);
                this.wallContentOrientationDirections[0].RemoveAt(this.mainAreaDimensions.y - 1);
                this.wallContentOrientationDirections[2].RemoveAt(0);
                this.wallEntranceTileIndexes[2]--;
                this.wallEntranceTileIndexes[0] = Mathf.Clamp(this.wallEntranceTileIndexes[0], this.wallEntranceWidths[0] - 1, newMainAreaDimensions.y - 1);
                this.wallEntranceTileIndexes[2] = Mathf.Clamp(this.wallEntranceTileIndexes[2], this.wallEntranceWidths[2] - 1, newMainAreaDimensions.y - 1);
            }
            this.mainAreaDimensions = newMainAreaDimensions;
        }

        public bool TryGetWallEntranceAssetAndIndexAndWidth(Vector2Int wallIndexDirection, out TiledEntranceAsset entranceAsset, out int tileIndex, out int width)
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");

            int wallIndex = WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection);
            entranceAsset = this.wallEntranceAssets[wallIndex];
            tileIndex = this.wallEntranceTileIndexes[wallIndex];
            width = this.wallEntranceWidths[wallIndex];
            return entranceAsset != null;
        }

        public void SetWallEntranceAssetAndIndexAndWidth(Vector2Int wallIndexDirection, TiledEntranceAsset entranceAsset, int tileIndex, int width)
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");
            Debug.Assert(entranceAsset != null, "TiledEntranceAsset cannot be null");
            Debug.Assert(width >= entranceAsset.MinWidth && width <= entranceAsset.MaxWidth && WallTiling.EdgeExtentIndexWithinBounds(wallIndexDirection, tileIndex, this.MainAreaDimensions, width), "Index out of bounds or cannot fit.");

            int wallIndex = WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection);
            if (entranceAsset != this.wallEntranceAssets[wallIndex] || tileIndex != this.wallEntranceTileIndexes[wallIndex] || width != this.wallEntranceWidths[wallIndex])
            {
                FrigidEdit.RecordChanges(this);
                this.wallEntranceAssets[wallIndex] = entranceAsset;
                this.wallEntranceTileIndexes[wallIndex] = tileIndex;
                this.wallEntranceWidths[wallIndex] = width;
            }
        }

        public void ClearWallEntranceAsset(Vector2Int wallIndexDirection)
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");

            int wallIndex = WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection);
            if (this.wallEntranceAssets[wallIndex] != null)
            {
                FrigidEdit.RecordChanges(this);
                this.wallEntranceAssets[wallIndex] = null;
                this.wallEntranceWidths[wallIndex] = -1;
                this.wallEntranceTileIndexes[wallIndex] = -1;
            }
        }

        public TerrainTileAsset GetTerrainTileAssetAt(Vector2Int tileIndexPosition)
        {
            Debug.Assert(AreaTiling.TileIndexPositionWithinBounds(tileIndexPosition, this.MainAreaDimensions), "Index position out of bounds.");

            return this.terrainTileAssets[tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x];
        }

        public void SetTerrainTileAssetAt(Vector2Int tileIndexPosition, TerrainTileAsset terrainTileAsset)
        {
            Debug.Assert(terrainTileAsset != null, "TerrainTileAsset cannot be null.");
            Debug.Assert(AreaTiling.TileIndexPositionWithinBounds(tileIndexPosition, this.MainAreaDimensions), "Index position out of bounds.");

            if (this.terrainTileAssets[tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] != terrainTileAsset)
            {
                FrigidEdit.RecordChanges(this);
                this.terrainTileAssets[tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] = terrainTileAsset;
            }
        }

        public bool TryGetTerrainContentAssetAndOrientationAt(TerrainContentHeight height, Vector2Int tileIndexPosition, out TerrainContentAsset terrainContentAsset, out Vector2 orientationDirection)
        {
            Debug.Assert(AreaTiling.TileIndexPositionWithinBounds(tileIndexPosition, this.MainAreaDimensions), "Index position out of bounds.");

            terrainContentAsset = this.terrainContentAssets[(int)height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x];
            orientationDirection = this.terrainContentOrientationDirections[(int)height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x];
            return terrainContentAsset != null;
        }

        public void SetTerrainContentAssetAndOrientationAt(Vector2Int tileIndexPosition, TerrainContentAsset terrainContentAsset, Vector2 orientationDirection)
        {
            Debug.Assert(terrainContentAsset != null, "TerrainContentAsset cannot be null.");
            Debug.Assert(AreaTiling.RectIndexPositionWithinBounds(tileIndexPosition, this.MainAreaDimensions, terrainContentAsset.GetDimensions(orientationDirection)), "Index position out of bounds or cannot fit.");

            if ((this.terrainContentAssets[(int)terrainContentAsset.Height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] != terrainContentAsset ||
                this.terrainContentOrientationDirections[(int)terrainContentAsset.Height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] != orientationDirection) &&
                tileIndexPosition.x >= terrainContentAsset.GetDimensions(orientationDirection).x - 1 && tileIndexPosition.y >= terrainContentAsset.GetDimensions(orientationDirection).y - 1)
            {
                for (int x = 0; x < this.MainAreaDimensions.x; x++)
                {
                    for (int y = 0; y < this.mainAreaDimensions.y; y++)
                    {
                        Vector2Int otherTileIndexPosition = new Vector2Int(x, y);
                        if (otherTileIndexPosition == tileIndexPosition) continue;

                        if (this.TryGetTerrainContentAssetAndOrientationAt(terrainContentAsset.Height, otherTileIndexPosition, out TerrainContentAsset otherTerrainContentAsset, out Vector2 otherOrientationDirection) && 
                            AreaTiling.AreRectIndexPositionsOverlapping(tileIndexPosition, terrainContentAsset.GetDimensions(orientationDirection), otherTileIndexPosition, otherTerrainContentAsset.GetDimensions(otherOrientationDirection)))
                        {
                            this.ClearTerrainContentAssetAt(terrainContentAsset.Height, otherTileIndexPosition);
                        }
                    }
                }

                FrigidEdit.RecordChanges(this);
                this.terrainContentAssets[(int)terrainContentAsset.Height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] = terrainContentAsset;
                this.terrainContentOrientationDirections[(int)terrainContentAsset.Height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] = orientationDirection;
            }
        }

        public void ClearTerrainContentAssetAt(TerrainContentHeight height, Vector2Int tileIndexPosition)
        {
            Debug.Assert(AreaTiling.TileIndexPositionWithinBounds(tileIndexPosition, this.MainAreaDimensions), "Index position out of bounds.");

            if (this.terrainContentAssets[(int)height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] != null ||
                this.terrainContentOrientationDirections[(int)height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] != Vector2.zero)
            {
                FrigidEdit.RecordChanges(this);
                this.terrainContentAssets[(int)height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] = null;
                this.terrainContentOrientationDirections[(int)height][tileIndexPosition.y * this.mainAreaDimensions.x + tileIndexPosition.x] = Vector2.zero;
            }
        }

        public WallTileAsset GetWallTileAssetAt(Vector2Int wallIndexDirection, int tileIndex)
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");
            Debug.Assert(WallTiling.EdgeTileIndexWithinBounds(wallIndexDirection, tileIndex, this.MainAreaDimensions), "Index out of bounds.");

            return this.wallTileAssets[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex];
        }

        public void SetWallTileAssetAt(Vector2Int wallIndexDirection, int tileIndex, WallTileAsset wallTileAsset)
        {
            Debug.Assert(wallTileAsset != null, "WallTileAsset cannot be null.");
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");
            Debug.Assert(WallTiling.EdgeTileIndexWithinBounds(wallIndexDirection, tileIndex, this.MainAreaDimensions), "Invalid wall direction.");

            if (this.wallTileAssets[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] != wallTileAsset)
            {
                FrigidEdit.RecordChanges(this);
                this.wallTileAssets[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] = wallTileAsset;
            }
        }

        public bool TryGetWallContentAssetAndOrientationAt(Vector2Int wallIndexDirection, int tileIndex, out WallContentAsset wallContentAsset, out Vector2 orientationDirection)
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");
            Debug.Assert(WallTiling.EdgeTileIndexWithinBounds(wallIndexDirection, tileIndex, this.MainAreaDimensions), "Index out of bounds.");

            wallContentAsset = this.wallContentAssets[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex];
            orientationDirection = this.wallContentOrientationDirections[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex];
            return wallContentAsset != null;
        }

        public void SetWallContentAssetAndOrientationAt(Vector2Int wallIndexDirection, int tileIndex, WallContentAsset wallContentAsset, Vector2 orientationDirection)
        {
            Debug.Assert(wallContentAsset != null, "WallContentAsset cannot be null.");
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");
            Debug.Assert(WallTiling.EdgeExtentIndexWithinBounds(wallIndexDirection, tileIndex, this.MainAreaDimensions, wallContentAsset.GetWidth(orientationDirection)), "Index out of bounds or cannot fit.");

            if (this.wallContentAssets[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] != wallContentAsset ||
                this.wallContentOrientationDirections[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] != orientationDirection)
            {
                for (int otherTileIndex = 0; otherTileIndex < WallTiling.GetEdgeLength(wallIndexDirection, this.MainAreaDimensions); otherTileIndex++)
                {
                    if (otherTileIndex == tileIndex) continue;

                    if (this.TryGetWallContentAssetAndOrientationAt(wallIndexDirection, otherTileIndex, out WallContentAsset otherWallContentAsset, out Vector2 otherOrientationDirection) &&
                        WallTiling.AreEdgeExtentIndexesOverlapping(tileIndex, wallContentAsset.GetWidth(orientationDirection), otherTileIndex, otherWallContentAsset.GetWidth(otherOrientationDirection)))
                    {
                        this.ClearWallContentAssetAt(wallIndexDirection, otherTileIndex);
                    }
                }

                FrigidEdit.RecordChanges(this);
                this.wallContentAssets[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] = wallContentAsset;
                this.wallContentOrientationDirections[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] = orientationDirection;
            }
        }

        public void ClearWallContentAssetAt(Vector2Int wallIndexDirection, int tileIndex)
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(wallIndexDirection), "Invalid wall direction.");
            Debug.Assert(WallTiling.EdgeTileIndexWithinBounds(wallIndexDirection, tileIndex, this.MainAreaDimensions), "Index out of bounds.");

            if (this.wallContentAssets[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] != null ||
                this.wallContentOrientationDirections[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] != Vector2.zero)
            {
                FrigidEdit.RecordChanges(this);
                this.wallContentAssets[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] = null;
                this.wallContentOrientationDirections[WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection)][tileIndex] = Vector2.zero;
            }
        }

        public int GetNumberMobSpawners()
        {
            return this.mobSpawners.Count;
        }

        public TiledAreaMobSpawnerSerializedReference GetMobSpawnerByReference(int spawnerIndex)
        {
            return this.mobSpawners[spawnerIndex];
        }

        public void SetMobSpawnerByReference(int spawnerIndex, TiledAreaMobSpawnerSerializedReference mobSpawner)
        {
            if (this.mobSpawners[spawnerIndex] != mobSpawner)
            {
                FrigidEdit.RecordChanges(this);
                this.mobSpawners[spawnerIndex] = mobSpawner;
            }
        }

        public void AddMobSpawner(int spawnerIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.mobSpawners.Insert(spawnerIndex, new TiledAreaMobSpawnerSerializedReference());
            this.mobSpawnPoints.Insert(spawnerIndex, new Nested1DList<TiledAreaMobSpawnPoint>());
        }

        public void RemoveMobSpawner(int spawnerIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.mobSpawners.RemoveAt(spawnerIndex);
            this.mobSpawnPoints.RemoveAt(spawnerIndex);
        }

        public int GetNumberMobSpawnPoints(int spawnerIndex)
        {
            return this.mobSpawnPoints[spawnerIndex].Count;
        }

        public TiledAreaMobSpawnPoint GetMobSpawnPoint(int spawnerIndex, int spawnPointIndex)
        {
            return this.mobSpawnPoints[spawnerIndex][spawnPointIndex];
        }

        public void SetMobSpawnPoint(int spawnerIndex, int spawnPointIndex, TiledAreaMobSpawnPoint mobSpawnPoint)
        {
            if (this.mobSpawnPoints[spawnerIndex][spawnPointIndex] != mobSpawnPoint)
            {
                FrigidEdit.RecordChanges(this);
                this.mobSpawnPoints[spawnerIndex][spawnPointIndex] = mobSpawnPoint;
            }
        }

        public void AddMobSpawnPoint(int spawnerIndex, int spawnPointIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.mobSpawnPoints[spawnerIndex].Insert(spawnPointIndex, new TiledAreaMobSpawnPoint(Vector2.zero));
        }

        public void RemoveMobSpawnPoint(int spawnerIndex, int spawnPointIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.mobSpawnPoints[spawnerIndex].RemoveAt(spawnPointIndex);
        }
    }
}
