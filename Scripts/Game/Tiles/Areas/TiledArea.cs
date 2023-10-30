using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledArea : FrigidMonoBehaviour
    {
        public const int MaxWallDepth = 4;

        private static SceneVariable<HashSet<TiledArea>> spawnedAreas;
        private static Action<TiledArea, TiledAreaBlueprint> onAreaSpawned;
        private static SceneVariable<List<TiledArea>> areasOrderedByDurationOpened;
        private static Action onFocusedAreaChanged;

        private const float TransitionDuration = 0.75f;

        [SerializeField]
        private Transform contentsTransform;
        [SerializeField]
        private WallColliders wallColliders;
        [SerializeField]
        private TiledAreaTransitioner transitioner;
        [SerializeField]
        private List<TiledAreaAtmosphere> atmospheres;

        private TiledLevel containedLevel;

        private NavigationGrid navigationGrid;

        private Vector2Int mainAreaDimensions;
        private Vector2Int wallAreaDimensions;
        private bool hasVisibleWalls;

        private Action onOpened;
        private Action onClosed;
        private Action onTransitionStarted;
        private Action onTransitionFinished;
        private bool isOpened;
        private bool isTransitioning;

        private HashSet<TiledEntrance> containingEntrances;

        static TiledArea()
        {
            spawnedAreas = new SceneVariable<HashSet<TiledArea>>(() => new HashSet<TiledArea>());
            areasOrderedByDurationOpened = new SceneVariable<List<TiledArea>>(() => new List<TiledArea>());
        }

        public static HashSet<TiledArea> SpawnedAreas
        {
            get
            {
                return spawnedAreas.Current;
            }
        }

        public static Action<TiledArea, TiledAreaBlueprint> OnAreaSpawned
        {
            get
            {
                return onAreaSpawned;
            }
            set
            {
                onAreaSpawned = value;
            }
        }

        public static Action OnFocusedAreaChanged
        {
            get
            {
                return onFocusedAreaChanged;
            }
            set
            {
                onFocusedAreaChanged = value;
            }
        }

        public Transform ContentsTransform
        {
            get
            {
                return this.contentsTransform;
            }
        }

        public TiledLevel ContainedLevel
        {
            get
            {
                return this.containedLevel;
            }
        }

        public NavigationGrid NavigationGrid
        {
            get
            {
                return this.navigationGrid;
            }
        }

        public Vector2 CenterPosition
        {
            get
            {
                return this.contentsTransform.position;
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
                return this.wallAreaDimensions;
            }
        }

        public bool HasVisibleWalls
        {
            get
            {
                return this.hasVisibleWalls;
            }
        }

        public Action OnOpened
        {
            get
            {
                return this.onOpened;
            }
            set
            {
                this.onOpened = value;
            }
        }

        public Action OnClosed
        {
            get
            {
                return this.onClosed;
            }
            set
            {
                this.onClosed = value;
            }
        }

        public Action OnTransitionStarted
        {
            get
            {
                return this.onTransitionStarted;
            }
            set
            {
                this.onTransitionStarted = value;
            }
        }

        public Action OnTransitionFinished
        {
            get
            {
                return this.onTransitionFinished;
            }
            set
            {
                this.onTransitionFinished = value;
            }
        }

        public bool IsOpened
        {
            get
            {
                return this.isOpened;
            }
        }

        public bool IsTransitioning
        {
            get
            {
                return this.isTransitioning;
            }
        }

        public HashSet<TiledEntrance> ContainingEntrances
        {
            get
            {
                return this.containingEntrances;
            }
        }

        public static bool TryGetAreaAtPosition(Vector2 position, out TiledArea area)
        {
            area = null;
            foreach (TiledArea spawnedArea in spawnedAreas.Current)
            {
                if (AreaTiling.TilePositionWithinBounds(position, spawnedArea.CenterPosition, spawnedArea.WallAreaDimensions))
                {
                    area = spawnedArea;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetFocusedArea(out TiledArea focusedArea)
        {
            focusedArea = null;
            if (areasOrderedByDurationOpened.Current.Count > 0)
            {
                focusedArea = areasOrderedByDurationOpened.Current[0];
                return true;
            }
            return false;
        }

        public void Spawn(TiledAreaBlueprint blueprint, bool isFirstArea, TiledLevel containedLevel)
        {
            if (spawnedAreas.Current.Add(this))
            {
                this.isOpened = false;
                this.containingEntrances = new HashSet<TiledEntrance>();

                this.containedLevel = containedLevel;

                this.mainAreaDimensions = blueprint.MainAreaDimensions;
                this.wallAreaDimensions = blueprint.WallAreaDimensions;
                this.hasVisibleWalls = blueprint.HasVisibleWalls;
                this.navigationGrid = new NavigationGrid(blueprint);
                this.wallColliders.PositionColliders(blueprint.MainAreaDimensions);
                this.transitioner.SetDimensions(blueprint.WallAreaDimensions);

                this.PopulateTerrainTiles(blueprint);
                this.PopulateWallTiles(blueprint);
                this.PopulateTerrainContent(blueprint);
                this.PopulateWallContent(blueprint);

                onAreaSpawned?.Invoke(this, blueprint);
                if (isFirstArea) this.AreaOpen();
                else this.AreaClose();
            }
            else
            {
                Debug.Log("Spawn called twice on TiledArea " + this.name + ".");
            }
        }

        public void TransitionTo(TiledAreaTransition transition, Vector2 entryPosition)
        {
            if (this.isTransitioning)
            {
                Debug.LogError("Tried transitioning to TiledArea when already transitioning.");
                return;
            }

            FrigidCoroutine.Run(
                Tween.Delay(
                    TransitionDuration,
                    () =>
                    {
                        this.AreaOpen();
                        this.isTransitioning = true;
                        this.onTransitionStarted?.Invoke();
                        this.transitioner.PlayTransitionTo(transition, TransitionDuration, entryPosition);
                        FrigidCoroutine.Run(
                            Tween.Delay(
                                TransitionDuration,
                                () =>
                                {
                                    this.onTransitionFinished?.Invoke();
                                    this.isTransitioning = false;
                                }
                                ),
                            this.gameObject
                            );
                    }
                    ),
                this.gameObject
                );
        }

        public void TransitionAway(TiledAreaTransition transition, Vector2 exitPosition)
        {
            if (this.isTransitioning)
            {
                Debug.LogError("Tried transitioning from TiledArea when already transitioning.");
                return;
            }

            this.isTransitioning = true;
            this.onTransitionStarted?.Invoke();
            this.transitioner.PlayTransitionAway(transition, TransitionDuration, exitPosition);
            FrigidCoroutine.Run(
                Tween.Delay(
                    TransitionDuration,
                    () =>
                    {
                        this.onTransitionFinished?.Invoke();
                        this.isTransitioning = false;
                        this.AreaClose();
                    }
                    ),
                this.gameObject
                );
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void AreaOpen()
        {
            this.contentsTransform.gameObject.SetActive(true);
            bool previouslyOpened = this.isOpened;
            this.isOpened = true;
            if (!previouslyOpened)
            {
                foreach (TiledAreaAtmosphere atmosphere in this.atmospheres)
                {
                    atmosphere.StartAtmosphere(this.mainAreaDimensions, this.contentsTransform);
                }

                this.onOpened?.Invoke();
            }

            bool focusChanged = areasOrderedByDurationOpened.Current.Count == 0;
            areasOrderedByDurationOpened.Current.Add(this);
            if (focusChanged)
            {
                onFocusedAreaChanged?.Invoke();
            }
        }

        private void AreaClose()
        {
            bool previouslyOpened = this.isOpened;
            this.isOpened = false;
            if (previouslyOpened)
            {
                foreach (TiledAreaAtmosphere atmosphere in this.atmospheres)
                {
                    atmosphere.StopAtmosphere();
                }

                this.onClosed?.Invoke();
            }
            this.contentsTransform.gameObject.SetActive(false);

            bool focusChanged = areasOrderedByDurationOpened.Current[0] == this;
            areasOrderedByDurationOpened.Current.Remove(this);
            if (focusChanged)
            {
                onFocusedAreaChanged?.Invoke();
            }
        }

        private void PopulateTerrainTiles(TiledAreaBlueprint blueprint)
        {
            for (int x = 0; x < blueprint.MainAreaDimensions.x; x++)
            {
                for (int y = 0; y < blueprint.MainAreaDimensions.y; y++)
                {
                    Vector2Int originTileIndexPosition = new Vector2Int(x, y);
                    TerrainTileAsset originTerrainTileAsset = blueprint.GetTerrainTileAssetAt(originTileIndexPosition);
                    if (originTerrainTileAsset == null) continue;
                    List<Vector2Int> terrainTileCorners = new List<Vector2Int>() { new Vector2Int(2, 2), new Vector2Int(2, -2), new Vector2Int(-2, -2), new Vector2Int(-2, 2) };

                    Vector2Int currentPosition = Vector2Int.zero;
                    Vector2Int previousPosition;
                    TerrainTileAsset[] visitedTerrainTileAssets = new TerrainTileAsset[3];
                    bool pushEnabled = false;

                    for (int i = 0; i < 10; i++)
                    {
                        previousPosition = currentPosition;
                        visitedTerrainTileAssets[0] = visitedTerrainTileAssets[1];
                        visitedTerrainTileAssets[1] = visitedTerrainTileAssets[2];

                        float currentDirection = i * Mathf.PI / 4;
                        currentPosition = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(currentDirection)), Mathf.RoundToInt(Mathf.Sin(currentDirection)));

                        if (AreaTiling.TileIndexPositionWithinBounds(new Vector2Int(x, y) + currentPosition, blueprint.MainAreaDimensions))
                        {
                            visitedTerrainTileAssets[2] = blueprint.GetTerrainTileAssetAt(new Vector2Int(x + currentPosition.x, y + currentPosition.y));
                        }
                        else
                        {
                            visitedTerrainTileAssets[2] = null;
                        }

                        if (i >= 2)
                        {
                            Vector2Int crossoverTileIndexPosition = new Vector2Int(x + previousPosition.x, y + previousPosition.y);
                            Vector2 crossoverDirection = ((crossoverTileIndexPosition - originTileIndexPosition) * new Vector2(1, -1)).normalized;
                            pushEnabled |= visitedTerrainTileAssets[1] != null && visitedTerrainTileAssets[1].Terrain != originTerrainTileAsset.Terrain;
                            if (AreaTiling.TileIndexPositionWithinBounds(crossoverTileIndexPosition, blueprint.MainAreaDimensions))
                            {
                                if (i % 2 == 0)
                                {
                                    if (visitedTerrainTileAssets[0] != originTerrainTileAsset && visitedTerrainTileAssets[2] != originTerrainTileAsset)
                                    {
                                        int cornerIndex = 4 - (i % 8 / 2);

                                        float rightAngleCornerAngle = ((cornerIndex + 1) * 90 - 45) * Mathf.Deg2Rad;
                                        Vector2Int rightAngleCornerPoint = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(rightAngleCornerAngle)) * 2, Mathf.RoundToInt(Mathf.Sin(rightAngleCornerAngle)) * 2);
                                        int startingIndex = terrainTileCorners.IndexOf(rightAngleCornerPoint);
                                        terrainTileCorners.RemoveAt(startingIndex);

                                        float firstSlantAngle = rightAngleCornerAngle + 45 * Mathf.Deg2Rad;
                                        float secondSlantAngle = rightAngleCornerAngle - 45 * Mathf.Deg2Rad;
                                        Vector2Int firstSlantPoint = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(firstSlantAngle)) * 2 + Mathf.RoundToInt(Mathf.Sin(firstSlantAngle)), Mathf.RoundToInt(Mathf.Sin(firstSlantAngle)) * 2 - Mathf.RoundToInt(Mathf.Cos(firstSlantAngle)));
                                        Vector2Int secondSlantPoint = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(secondSlantAngle)) * 2 - Mathf.RoundToInt(Mathf.Sin(secondSlantAngle)), Mathf.RoundToInt(Mathf.Sin(secondSlantAngle)) * 2 + Mathf.RoundToInt(Mathf.Cos(secondSlantAngle)));

                                        if (!terrainTileCorners.Contains(firstSlantPoint))
                                        {
                                            terrainTileCorners.Insert(startingIndex, firstSlantPoint);
                                            startingIndex++;
                                        }

                                        if (!terrainTileCorners.Contains(secondSlantPoint))
                                        {
                                            terrainTileCorners.Insert(startingIndex, secondSlantPoint);
                                        }
                                    }

                                    if (visitedTerrainTileAssets[0] != visitedTerrainTileAssets[2])
                                    {
                                        if (visitedTerrainTileAssets[0] != originTerrainTileAsset && visitedTerrainTileAssets[2] != originTerrainTileAsset)
                                        {
                                            SpawnTerrainCrossoverTile(originTerrainTileAsset, visitedTerrainTileAssets[1], crossoverTileIndexPosition, false, crossoverDirection);
                                        }
                                    }
                                    else
                                    {
                                        SpawnTerrainCrossoverTile(visitedTerrainTileAssets[0], visitedTerrainTileAssets[1], crossoverTileIndexPosition, true, crossoverDirection);
                                        if (visitedTerrainTileAssets[0] != originTerrainTileAsset)
                                        {
                                            SpawnTerrainCrossoverTile(originTerrainTileAsset, visitedTerrainTileAssets[0], crossoverTileIndexPosition, false, crossoverDirection);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((visitedTerrainTileAssets[0] != originTerrainTileAsset && visitedTerrainTileAssets[2] != originTerrainTileAsset) ||
                                        (visitedTerrainTileAssets[0] == null && visitedTerrainTileAssets[2] != originTerrainTileAsset) ||
                                        (visitedTerrainTileAssets[0] != originTerrainTileAsset && visitedTerrainTileAssets[2] == null))
                                    {
                                        SpawnTerrainCrossoverTile(originTerrainTileAsset, visitedTerrainTileAssets[1], crossoverTileIndexPosition, false, crossoverDirection);
                                    }
                                }
                            }
                        }
                    }

                    SpawnTerrainTile(originTerrainTileAsset, originTileIndexPosition, pushEnabled, terrainTileCorners);
                }
            }

            void SpawnTerrainTile(TerrainTileAsset terrainTileAsset, Vector2Int tileIndexPosition, bool pushEnabled, List<Vector2Int> cornerPoints)
            {
                TerrainTile spawnedTerrainTile = CreateInstance<TerrainTile>(
                    terrainTileAsset.TerrainTilePrefab,
                    AreaTiling.TilePositionFromIndexPosition(tileIndexPosition, this.CenterPosition, blueprint.MainAreaDimensions),
                    this.contentsTransform
                    );
                spawnedTerrainTile.Populate(pushEnabled, cornerPoints, this.navigationGrid, tileIndexPosition);
            }

            void SpawnTerrainCrossoverTile(TerrainTileAsset terrainTileAsset, TerrainTileAsset crossoverTerrainTileAsset, Vector2Int crossoverTileIndexPosition, bool isInner, Vector2 direction)
            {
                if (terrainTileAsset.TryGetTerrainCrossoverTilePrefab(crossoverTerrainTileAsset, out TerrainCrossoverTile terrainCrossoverTilePrefab))
                {
                    TerrainCrossoverTile spawnedCrossoverTile = CreateInstance<TerrainCrossoverTile>(
                        terrainCrossoverTilePrefab,
                        AreaTiling.TilePositionFromIndexPosition(crossoverTileIndexPosition, this.CenterPosition, blueprint.MainAreaDimensions) + Vector2.down * terrainTileAsset.ElevationFudgeFactor * FrigidConstants.WorldSizeEpsilon,
                        this.contentsTransform
                        );
                    spawnedCrossoverTile.Populated(direction, isInner);
                }
            }
        }


        private void PopulateWallTiles(TiledAreaBlueprint blueprint)
        {
            if (!this.HasVisibleWalls) return;

            for (int layer = 0; layer <= MaxWallDepth; layer++)
            {
                Vector2Int layerBoundsDimensions = new Vector2Int(blueprint.MainAreaDimensions.x + 2 * (layer - 1), blueprint.MainAreaDimensions.y + 2 * (layer - 1));
                Vector2Int[] wallIndexDirections = WallTiling.GetAllWallIndexDirections();

                for (int i = 0; i < wallIndexDirections.Length; i++)
                {
                    Vector2Int currWallIndexDirection = wallIndexDirections[i];
                    Vector2Int nextWallIndexDirection = wallIndexDirections[(i + 1) % wallIndexDirections.Length];
                    float rotationDeg = (i - 1) * 90;

                    for (int tileIndex = 0; tileIndex < WallTiling.GetEdgeLength(currWallIndexDirection, layerBoundsDimensions); tileIndex++)
                    {
                        WallTileAsset edgeWallTileAsset = blueprint.GetWallTileAssetAt(currWallIndexDirection, Mathf.Clamp(tileIndex - layer + 1, 0, WallTiling.GetEdgeLength(currWallIndexDirection, this.MainAreaDimensions) - 1));
                        if (layer > edgeWallTileAsset.Depth) continue;

                        Vector2 edgePosition = WallTiling.EdgeTilePositionFromWallIndexDirectionAndTileIndex(currWallIndexDirection, tileIndex, this.CenterPosition, layerBoundsDimensions);
                        if (layer == 0)
                        {
                            Vector2Int tileIndexPosition = AreaTiling.TileIndexPositionFromPosition(edgePosition, this.CenterPosition, this.MainAreaDimensions);
                            if (edgeWallTileAsset.TryGetWallBoundaryTilePrefab(blueprint.GetTerrainTileAssetAt(tileIndexPosition), out WallBoundaryTile wallBoundaryTilePrefab))
                            {
                                WallBoundaryTile spawnedEdgeWallBoundaryTile = CreateInstance<WallBoundaryTile>(wallBoundaryTilePrefab, edgePosition, Quaternion.Euler(0, 0, rotationDeg), this.contentsTransform);
                                spawnedEdgeWallBoundaryTile.Populate(true);
                            }
                        }
                        else
                        {
                            WallTile spawnedEdgeWallTile = CreateInstance<WallTile>(edgeWallTileAsset.GetWallTilePrefab(layer), edgePosition, Quaternion.Euler(0, 0, rotationDeg), this.contentsTransform);
                            spawnedEdgeWallTile.Populate(true);
                        }
                    }

                    WallTileAsset cornerWallTileAsset = blueprint.GetWallTileAssetAt(currWallIndexDirection, 0);
                    if (layer > cornerWallTileAsset.Depth) continue;

                    Vector2 cornerPosition = WallTiling.CornerTilePositionFromWallIndexDirections(currWallIndexDirection, nextWallIndexDirection, this.CenterPosition, layerBoundsDimensions);
                    if (layer == 0)
                    {
                        Vector2Int tileIndexPosition = AreaTiling.TileIndexPositionFromPosition(cornerPosition, this.CenterPosition, this.mainAreaDimensions);
                        if (cornerWallTileAsset.TryGetWallBoundaryTilePrefab(blueprint.GetTerrainTileAssetAt(tileIndexPosition), out WallBoundaryTile wallBoundaryTilePrefab))
                        {
                            WallBoundaryTile spawnedCornerWallBoundaryTile = CreateInstance<WallBoundaryTile>(wallBoundaryTilePrefab, cornerPosition, Quaternion.Euler(0, 0, rotationDeg), this.contentsTransform);
                            spawnedCornerWallBoundaryTile.Populate(false);
                        }
                    }
                    else
                    {
                        WallTile spawnedCornerWallTile = CreateInstance<WallTile>(cornerWallTileAsset.GetWallTilePrefab(layer), WallTiling.CornerTilePositionFromWallIndexDirections(currWallIndexDirection, nextWallIndexDirection, this.CenterPosition, layerBoundsDimensions), Quaternion.Euler(0, 0, rotationDeg), this.contentsTransform);
                        spawnedCornerWallTile.Populate(false);
                    }
                }
            }
        }

        private void PopulateTerrainContent(TiledAreaBlueprint blueprint)
        {
            for (int y = 0; y < blueprint.MainAreaDimensions.y; y++)
            {
                int[] numSpawnedOnRowAtHeight = new int[(int)TerrainContentHeight.Count];
                for (int x = 0; x < blueprint.MainAreaDimensions.x; x++)
                {
                    for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                    {
                        TerrainContentHeight height = (TerrainContentHeight)i;
                        Vector2Int tileIndexPosition = new Vector2Int(x, y);
                        if (blueprint.TryGetTerrainContentAssetAndOrientationAt(height, tileIndexPosition, out TerrainContentAsset terrainContentAsset, out Vector2 orientationDirection) &&
                            terrainContentAsset.TryGetTerrainContentPrefab(blueprint.GetTerrainTileAssetAt(tileIndexPosition).Terrain, out TerrainContent terrainContentPrefab))
                        {
                            Vector2Int dimensions = terrainContentAsset.GetDimensions(orientationDirection);
                            Vector2 tilePosition = AreaTiling.RectPositionFromIndexPosition(new Vector2Int(x, y), this.CenterPosition, blueprint.MainAreaDimensions, dimensions);
                            TerrainContent terrainContent = CreateInstance<TerrainContent>(terrainContentPrefab, tilePosition + Vector2.up * (numSpawnedOnRowAtHeight[i] % 2 * FrigidConstants.WorldSizeEpsilon), this.contentsTransform);
                            List<Vector2Int> tileIndexPositions = new List<Vector2Int>();
                            AreaTiling.VisitTileIndexPositionsInTileRect(new Vector2Int(x, y), dimensions, blueprint.MainAreaDimensions, (Vector2Int tileIndexPosition) => tileIndexPositions.Add(tileIndexPosition));
                            terrainContent.Populate(orientationDirection, this.navigationGrid, tileIndexPositions);
                            numSpawnedOnRowAtHeight[i]++;
                        }
                    }
                }
            }
        }

        private void PopulateWallContent(TiledAreaBlueprint blueprint)
        {
            if (!this.HasVisibleWalls) return;

            Vector2Int[] wallIndexDirections = WallTiling.GetAllWallIndexDirections();
            for (int i = 0; i < wallIndexDirections.Length; i++)
            {
                Vector2Int wallIndexDirection = wallIndexDirections[i];
                float rotationDeg = (i - 1) * 90;
                for (int tileIndex = 0; tileIndex < WallTiling.GetEdgeLength(wallIndexDirections[i], this.MainAreaDimensions); tileIndex++)
                {
                    if (blueprint.TryGetWallContentAssetAndOrientationAt(wallIndexDirection, tileIndex, out WallContentAsset wallContentAsset, out Vector2 orientationDirection) &&
                        wallContentAsset.TryGetWallContentPrefab(blueprint.GetTerrainTileAssetAt(WallTiling.WallIndexDirectionAndTileIndexToInnerTileIndexPosition(wallIndexDirection, tileIndex, blueprint.MainAreaDimensions)).Terrain, out WallContent wallContentPrefab))
                    {
                        WallContent spawnedWallContent = CreateInstance<WallContent>(
                            wallContentPrefab,
                            WallTiling.EdgeExtentPositionFromWallIndexDirectionAndExtentIndex(wallIndexDirection, tileIndex, this.CenterPosition, this.mainAreaDimensions, wallContentAsset.GetWidth(orientationDirection)),
                            Quaternion.Euler(0, 0, rotationDeg),
                            this.contentsTransform
                            );
                        spawnedWallContent.Populate(orientationDirection);
                    }
                }
            }
        }
    }
}
