using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledLevel : FrigidMonoBehaviour
    {
        private static SceneVariable<HashSet<TiledLevel>> spawnedTiledLevels;
        private static Action<TiledLevel> onTiledLevelSpawned;
        private static SceneVariable<Queue<TiledLevel>> levelSpawnQueue;
        private static TiledLevel currentFocusedTiledLevel;
        private static Action onFocusedTiledLevelChanged;

        [SerializeField]
        private TiledLevelPlannerSerializedReference tiledLevelPlanner;
        [SerializeField]
        private List<TiledAreaEntrance> subLevelEntrances;
        [SerializeField]
        [Tooltip("Only check this for levels that you expect to be large.")]
        private bool onlyQueueLevelSpawnWhenAccessible;

        private const int LOCAL_SPAWN_POSITION_PADDING_X = 2;
        private const int LOCAL_SPAWN_POSITION_PADDING_Y = 2;

        private TiledLevelPlan tiledLevelPlan;
        private Dictionary<TiledAreaEntrance, TiledArea> subLevelEntrancesAndContainedAreas;
        private Dictionary<TiledLevelPlanArea, TiledArea> spawnedAreaPerPlanAreas;
        private Dictionary<TiledLevelPlanEntrance, TiledAreaEntrance> spawnedEntrancePerPlanEntrances;
        private Bounds wallBounds;

        static TiledLevel()
        {
            spawnedTiledLevels = new SceneVariable<HashSet<TiledLevel>>(() => { return new HashSet<TiledLevel>(); });
            levelSpawnQueue = new SceneVariable<Queue<TiledLevel>>(() => { return new Queue<TiledLevel>(); });
            currentFocusedTiledLevel = null;
            TiledArea.OnFocusedTiledAreaChanged += 
                () => 
                { 
                    if (TryGetFocusedTiledLevel(out TiledLevel focusedTiledLevel) && focusedTiledLevel != currentFocusedTiledLevel)
                    {
                        currentFocusedTiledLevel = focusedTiledLevel;
                        onFocusedTiledLevelChanged?.Invoke();
                    }
                };
        }

        public static HashSet<TiledLevel> SpawnedTiledLevels
        {
            get
            {
                return spawnedTiledLevels.Current;
            }
        }

        public static Action<TiledLevel> OnTiledLevelSpawned
        {
            get
            {
                return onTiledLevelSpawned;
            }
            set
            {
                onTiledLevelSpawned = value;
            }
        }

        public static Action OnFocusedTiledLevelChanged
        {
            get
            {
                return onFocusedTiledLevelChanged;
            }
            set
            {
                onFocusedTiledLevelChanged = value;
            }
        }

        public TiledLevelPlan TiledLevelPlan
        {
            get
            {
                return this.tiledLevelPlan;
            }
        }

        public Dictionary<TiledLevelPlanArea, TiledArea> SpawnedAreaPerPlanAreas
        {
            get
            {
                return this.spawnedAreaPerPlanAreas;
            }
        }

        public Dictionary<TiledLevelPlanEntrance, TiledAreaEntrance> SpawnedEntrancePerPlanEntrances
        {
            get
            {
                return this.spawnedEntrancePerPlanEntrances;
            }
        }

        public Vector2 AbsoluteCenterPosition
        {
            get
            {
                return this.wallBounds.center;
            }
        }

        public Vector2 WallBoundsSize
        {
            get
            {
                return this.wallBounds.size;
            }
        }

        public static bool TryGetTiledLevelAtPosition(Vector2 currentPosition, out TiledLevel tiledLevel)
        {
            tiledLevel = null;
            foreach (TiledLevel spawnedTiledLevel in spawnedTiledLevels.Current)
            {
                if (spawnedTiledLevel.wallBounds.Contains(currentPosition))
                {
                    tiledLevel = spawnedTiledLevel;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetFocusedTiledLevel(out TiledLevel focusedTiledLevel)
        {
            focusedTiledLevel = null;
            return 
                TiledArea.TryGetFocusedTiledArea(out TiledArea focusedTiledArea) && 
                TryGetTiledLevelAtPosition(focusedTiledArea.AbsoluteCenterPosition, out focusedTiledLevel);
        }
        
        protected override void Awake()
        {
            base.Awake();
            this.transform.SetParent(null);
        }

        protected override void Start()
        {
            base.Start();
            FindContainedAreaForEachSubLevelEntrance();
            QueueForLevelSpawn();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void QueueForLevelSpawn()
        {
            levelSpawnQueue.Current.Enqueue(this);
            if (levelSpawnQueue.Current.Peek() == this)
            {
                ContinueLevelSpawnQueue();
            }
        }

        private void ContinueLevelSpawnQueue()
        {
            if (this.onlyQueueLevelSpawnWhenAccessible)
            {
                bool anyOpen = false;
                foreach (TiledArea containedArea in this.subLevelEntrancesAndContainedAreas.Values) containedArea.OnOpened -= QueueForLevelSpawn;
                foreach (TiledArea containedArea in this.subLevelEntrancesAndContainedAreas.Values) anyOpen |= containedArea.IsOpened;
                if (!anyOpen)
                {
                    foreach (TiledArea containedArea in this.subLevelEntrancesAndContainedAreas.Values) containedArea.OnOpened += QueueForLevelSpawn;
                    levelSpawnQueue.Current.Dequeue();
                    if (levelSpawnQueue.Current.Count > 0) levelSpawnQueue.Current.Peek().ContinueLevelSpawnQueue();
                    return;
                }
            }

            LoadingOverlay.RequestLoad(
                () =>
                {
                    FrigidCoroutine.Run(
                        SpawnLevel(
                            () =>
                            {
                                spawnedTiledLevels.Current.Add(this);
                                onTiledLevelSpawned?.Invoke(this);
                                if (TiledArea.TryGetFocusedTiledArea(out TiledArea focusedTiledArea) && this.wallBounds.Contains(focusedTiledArea.AbsoluteCenterPosition))
                                {
                                    currentFocusedTiledLevel = this;
                                    onFocusedTiledLevelChanged?.Invoke();
                                }
                                levelSpawnQueue.Current.Dequeue();
                                if (levelSpawnQueue.Current.Count > 0) levelSpawnQueue.Current.Peek().ContinueLevelSpawnQueue();
                                LoadingOverlay.ReleaseLoad();
                            }
                            ),
                        this.gameObject
                        );
                }
                );
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.DrawWireCube(this.wallBounds.center, this.wallBounds.size);
        }
#endif

        private IEnumerator<FrigidCoroutine.Delay> SpawnLevel(Action onComplete)
        {
            this.tiledLevelPlan = this.tiledLevelPlanner.ImmutableValue.CreateLevelPlan(this.subLevelEntrancesAndContainedAreas);
            Dictionary<TiledLevelPlanArea, Vector2> localSpawnPositionPerPlanAreas = CalculateLocalSpawnPositionPerPlanAreas(this.tiledLevelPlan);
            Bounds localBounds = CalculateLocalBounds(localSpawnPositionPerPlanAreas);
            Vector2 levelPosition = CalculateLevelPosition(localBounds);

            this.transform.position = levelPosition;
            this.wallBounds = new Bounds(localBounds.center + (Vector3)levelPosition, localBounds.size);
            this.spawnedAreaPerPlanAreas = new Dictionary<TiledLevelPlanArea, TiledArea>();

            yield return null;

            foreach (TiledLevelPlanArea planArea in tiledLevelPlan.Areas)
            {
                TiledArea spawnedTiledArea = SpawnTiledArea(
                    planArea.TiledAreaPrefab,
                    planArea.ChosenBlueprint,
                    localSpawnPositionPerPlanAreas[planArea],
                    planArea == this.tiledLevelPlan.StartingArea && !this.tiledLevelPlan.IsSubLevel
                    );
                this.spawnedAreaPerPlanAreas.Add(planArea, spawnedTiledArea);
                
                // TODO: renable after this.tiledLevelPlan.MobGenerator.GenerateMobs(planArea, spawnedTiledArea);
                // Stagger spawns of areas per frame.
                yield return null;
            }

            this.spawnedEntrancePerPlanEntrances = new Dictionary<TiledLevelPlanEntrance, TiledAreaEntrance>();
            foreach (TiledLevelPlanConnection planConnection in tiledLevelPlan.Connections)
            {
                for (int i = 0; i < planConnection.NumberEntrances; i++)
                {
                    if (planConnection.PlanEntrances[i].IsSubLevelEntrance)
                    {
                        if (this.subLevelEntrancesAndContainedAreas.TryGetValue(planConnection.PlanEntrances[i].SublevelEntrance, out TiledArea containedArea))
                        {
                            TiledAreaEntrance subLevelEntrance = InitializeSubLevelEntrance(
                                containedArea,
                                planConnection.PlanEntrances[i].SublevelEntrance,
                                planConnection.EntryDirections[i],
                                planConnection.ConnectionTerrain
                                );
                            this.spawnedEntrancePerPlanEntrances.Add(planConnection.PlanEntrances[i], subLevelEntrance);
                        }
                        else
                        {
                            Debug.LogError("Should never reach here, either the level added a new sub level entrance or there was an error at a previous step.");
                            break;
                        }
                    }
                    else
                    {
                        TiledLevelPlanArea entrancePlanArea = planConnection.PlanEntrances[i].Area;
                        TiledArea spawnedTiledArea = this.spawnedAreaPerPlanAreas[planConnection.PlanEntrances[i].Area];
                        TileTerrain entranceTerrain = entrancePlanArea.ChosenBlueprint.GetEntranceTerrain(planConnection.EntryDirections[i]);
                        TiledAreaEntrance spawnedEntrance = SpawnWallEntrance(
                            spawnedTiledArea,
                            planConnection.PlanEntrances[i].EntrancePrefab,
                            planConnection.EntryDirections[i],
                            entranceTerrain
                            );
                        this.spawnedEntrancePerPlanEntrances.Add(planConnection.PlanEntrances[i], spawnedEntrance);
                    }
                }

                foreach (TiledLevelPlanEntrance planEntrance in planConnection.PlanEntrances)
                {
                    foreach (TiledLevelPlanEntrance otherPlanEntrance in planConnection.PlanEntrances)
                    {
                        if (planEntrance != otherPlanEntrance) this.spawnedEntrancePerPlanEntrances[planEntrance].AddConnectedEntrance(this.spawnedEntrancePerPlanEntrances[otherPlanEntrance]);
                    }
                }

                // Stagger spawns of entrances per frame.
                yield return null;
            }

            onComplete.Invoke();
        }

        private void FindContainedAreaForEachSubLevelEntrance()
        {
            this.subLevelEntrancesAndContainedAreas = new Dictionary<TiledAreaEntrance, TiledArea>();
            foreach (TiledAreaEntrance subLevelEntrance in this.subLevelEntrances)
            {
                if (TiledArea.TryGetTiledAreaAtPosition(subLevelEntrance.transform.position, out TiledArea tiledArea))
                {
                    this.subLevelEntrancesAndContainedAreas.Add(subLevelEntrance, tiledArea);
                }
                else
                {
                    Debug.LogError("Sublevel entrance " + subLevelEntrance.name + " is not in a pre-existing tiled area!");
                }
            }
        }

        private Dictionary<TiledLevelPlanArea, Vector2> CalculateLocalSpawnPositionPerPlanAreas(TiledLevelPlan tiledLevelPlan)
        {
            Dictionary<TiledLevelPlanArea, Vector2> localSpawnPositionPerPlanAreas = new Dictionary<TiledLevelPlanArea, Vector2>();
            Queue<TiledLevelPlanArea> queuedPlanAreas = new Queue<TiledLevelPlanArea>();
            queuedPlanAreas.Enqueue(tiledLevelPlan.StartingArea);
            localSpawnPositionPerPlanAreas.Add(tiledLevelPlan.StartingArea, Vector2.zero);

            int wallAreaDimensionsMaxX = 0;
            int wallAreaDimensionsMaxY = 0;
            foreach (TiledLevelPlanArea planArea in tiledLevelPlan.Areas)
            {
                if (planArea.ChosenBlueprint.WallAreaDimensions.x > wallAreaDimensionsMaxX)
                {
                    wallAreaDimensionsMaxX = planArea.ChosenBlueprint.WallAreaDimensions.x;
                }
                if (planArea.ChosenBlueprint.WallAreaDimensions.y > wallAreaDimensionsMaxY)
                {
                    wallAreaDimensionsMaxY = planArea.ChosenBlueprint.WallAreaDimensions.y;
                }
            }
            wallAreaDimensionsMaxX += LOCAL_SPAWN_POSITION_PADDING_X;
            wallAreaDimensionsMaxY += LOCAL_SPAWN_POSITION_PADDING_Y;

            while (queuedPlanAreas.Count > 0)
            {
                TiledLevelPlanArea dequeuedPlanArea = queuedPlanAreas.Dequeue();
                foreach (TiledLevelPlanConnection planConnection in tiledLevelPlan.Connections)
                {
                    if (planConnection.IsSubLevelConnection) continue;

                    bool isConnected = false;

                    for (int i = 0; i < planConnection.NumberEntrances; i++)
                    {
                        if (planConnection.PlanEntrances[i].Area == dequeuedPlanArea)
                        {
                            isConnected = true;
                        }
                    }

                    if (isConnected)
                    {
                        for (int i = 0; i < planConnection.NumberEntrances; i++)
                        {
                            TiledLevelPlanArea adjacentPlanArea = planConnection.PlanEntrances[i].Area;
                            if (!localSpawnPositionPerPlanAreas.ContainsKey(adjacentPlanArea))
                            {
                                Vector2Int adjacentDirection = -planConnection.EntryDirections[i];
                                queuedPlanAreas.Enqueue(adjacentPlanArea);
                                localSpawnPositionPerPlanAreas.Add(
                                    adjacentPlanArea,
                                    localSpawnPositionPerPlanAreas[dequeuedPlanArea] + new Vector2(adjacentDirection.x * wallAreaDimensionsMaxX, adjacentDirection.y * wallAreaDimensionsMaxY) * GameConstants.UNIT_WORLD_SIZE
                                    );
                            }
                        }
                    }
                }
            }
            return localSpawnPositionPerPlanAreas;
        }

        private Bounds CalculateLocalBounds(Dictionary<TiledLevelPlanArea, Vector2> localSpawnPositionPerPlanAreas)
        {
            Bounds localBounds = new Bounds();
            foreach (KeyValuePair<TiledLevelPlanArea, Vector2> localSpawnPositionPerPlanArea in localSpawnPositionPerPlanAreas)
            {
                Vector2 wallExtents = new Vector2(localSpawnPositionPerPlanArea.Key.ChosenBlueprint.WallAreaDimensions.x, localSpawnPositionPerPlanArea.Key.ChosenBlueprint.WallAreaDimensions.y) * GameConstants.UNIT_WORLD_SIZE / 2;
                localBounds.Encapsulate(localSpawnPositionPerPlanArea.Value + wallExtents);
                localBounds.Encapsulate(localSpawnPositionPerPlanArea.Value - wallExtents);
            }
            return localBounds;
        }

        private Vector2 CalculateLevelPosition(Bounds localBounds)
        {
            Vector2 levelPosition = Vector2.zero;
            if (spawnedTiledLevels.Current.Count > 0)
            {
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                foreach (TiledLevel tiledLevel in spawnedTiledLevels.Current)
                {
                    if (tiledLevel.AbsoluteCenterPosition.x - tiledLevel.WallBoundsSize.x / 2 < minX)
                    {
                        minX = tiledLevel.AbsoluteCenterPosition.x - tiledLevel.WallBoundsSize.x / 2;
                    }
                    if (tiledLevel.AbsoluteCenterPosition.x + tiledLevel.WallBoundsSize.x / 2 > maxX)
                    {
                        maxX = tiledLevel.AbsoluteCenterPosition.x + tiledLevel.WallBoundsSize.x / 2;
                    }
                }

                if (Mathf.Abs(maxX) > Mathf.Abs(minX))
                {
                    levelPosition = new Vector2(-localBounds.extents.x + minX, 0);
                }
                else
                {
                    levelPosition = new Vector2(localBounds.extents.x + maxX, 0);
                }
            }
            levelPosition.x -= localBounds.center.x;
            return levelPosition;
        }

        private TiledArea SpawnTiledArea(TiledArea tiledAreaPrefab, TiledAreaBlueprint blueprint, Vector2 localSpawnPosition, bool isFirstTiledArea)
        {
            TiledArea spawnedTiledArea = FrigidInstancing.CreateInstance<TiledArea>(tiledAreaPrefab, this.transform.position + (Vector3)localSpawnPosition, this.transform);
            spawnedTiledArea.Populate(blueprint, isFirstTiledArea);
            return spawnedTiledArea;
        }

        private TiledAreaEntrance SpawnWallEntrance(TiledArea tiledArea, TiledAreaEntrance entrancePrefab, Vector2Int entryDirection, TileTerrain entranceTerrain)
        {
            float zRotation = Mathf.Atan2(entryDirection.y, entryDirection.x) * Mathf.Rad2Deg - 90;
            Vector2 localSpawnPosition = TilePositioning.LocalWallCenterPosition(entryDirection, tiledArea.MainAreaDimensions + Vector2Int.one);
            TiledAreaEntrance spawnedEntrance = FrigidInstancing.CreateInstance<TiledAreaEntrance>(entrancePrefab, tiledArea.transform.position + (Vector3)localSpawnPosition, Quaternion.Euler(0, 0, zRotation), tiledArea.ContentsTransform);
            spawnedEntrance.Populate(entryDirection, entranceTerrain);
            tiledArea.PlaceEntrance(spawnedEntrance);
            return spawnedEntrance;
        }

        private TiledAreaEntrance InitializeSubLevelEntrance(TiledArea tiledArea, TiledAreaEntrance subLevelEntrance, Vector2Int entryDirection, TileTerrain entranceTerrain)
        {
            subLevelEntrance.Populate(entryDirection, entranceTerrain);
            tiledArea.PlaceEntrance(subLevelEntrance);
            return subLevelEntrance;
        }
    }
}
