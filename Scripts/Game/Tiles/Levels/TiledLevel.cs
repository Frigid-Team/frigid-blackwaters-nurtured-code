using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledLevel : FrigidMonoBehaviour
    {
        private static SceneVariable<HashSet<TiledLevel>> spawnedLevels;
        private static Action<TiledLevel, TiledLevelPlanner> onLevelSpawned;
        private static SceneVariable<Queue<TiledLevel>> levelSpawnQueue;
        private static TiledLevel currentFocusedLevel;
        private static Action onFocusedLevelChanged;

        [SerializeField]
        private TiledLevelPlannerSerializedReference levelPlanner;
        [SerializeField]
        private List<TiledEntrance> subLevelEntrances;
        [SerializeField]
        [Tooltip("Only check this for levels that you expect to be large.")]
        private bool onlyQueueLevelSpawnWhenAccessible;

        private const int LOCAL_SPAWN_POSITION_PADDING_X = 2;
        private const int LOCAL_SPAWN_POSITION_PADDING_Y = 2;

        private Dictionary<TiledEntrance, TiledArea> subLevelEntrancesAndContainedAreas;
        private Bounds wallBounds;
        private HashSet<TiledArea> containingAreas;
        private HashSet<(TiledEntrance, TiledEntrance)> containingConnections;

        static TiledLevel()
        {
            spawnedLevels = new SceneVariable<HashSet<TiledLevel>>(() => new HashSet<TiledLevel>());
            levelSpawnQueue = new SceneVariable<Queue<TiledLevel>>(() => new Queue<TiledLevel>());
            currentFocusedLevel = null;
            TiledArea.OnFocusedAreaChanged += 
                () => 
                { 
                    if (TryGetFocusedLevel(out TiledLevel focusedLevel) && focusedLevel != currentFocusedLevel)
                    {
                        currentFocusedLevel = focusedLevel;
                        onFocusedLevelChanged?.Invoke();
                    }
                };
        }

        public static HashSet<TiledLevel> SpawnedLevels
        {
            get
            {
                return spawnedLevels.Current;
            }
        }

        public static Action<TiledLevel, TiledLevelPlanner> OnLevelSpawned
        {
            get
            {
                return onLevelSpawned;
            }
            set
            {
                onLevelSpawned = value;
            }
        }

        public static Action OnFocusedLevelChanged
        {
            get
            {
                return onFocusedLevelChanged;
            }
            set
            {
                onFocusedLevelChanged = value;
            }
        }

        public Vector2 CenterPosition
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

        public HashSet<TiledArea> ContainingAreas
        {
            get
            {
                return this.containingAreas;
            }
        }

        public HashSet<(TiledEntrance, TiledEntrance)> ContainingConnections
        {
            get
            {
                return this.containingConnections;
            }
        }

        public static bool TryGetLevelAtPosition(Vector2 position, out TiledLevel level)
        {
            level = null;
            foreach (TiledLevel spawnedTiledLevel in spawnedLevels.Current)
            {
                if (spawnedTiledLevel.wallBounds.Contains(position))
                {
                    level = spawnedTiledLevel;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetFocusedLevel(out TiledLevel focusedLevel)
        {
            focusedLevel = null;
            return 
                TiledArea.TryGetFocusedArea(out TiledArea focusedArea) && 
                TryGetLevelAtPosition(focusedArea.CenterPosition, out focusedLevel);
        }
        
        protected override void Awake()
        {
            base.Awake();
            this.transform.SetParent(null);
        }

        protected override void Start()
        {
            base.Start();
            this.FindContainedAreaForEachSubLevelEntrance();
            this.QueueForLevelSpawn();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void QueueForLevelSpawn()
        {
            levelSpawnQueue.Current.Enqueue(this);
            if (levelSpawnQueue.Current.Peek() == this)
            {
                this.ContinueLevelSpawnQueue();
            }
        }

        private void ContinueLevelSpawnQueue()
        {
            if (this.onlyQueueLevelSpawnWhenAccessible)
            {
                bool anyOpen = false;
                foreach (TiledArea containedArea in this.subLevelEntrancesAndContainedAreas.Values) containedArea.OnOpened -= this.QueueForLevelSpawn;
                foreach (TiledArea containedArea in this.subLevelEntrancesAndContainedAreas.Values) anyOpen |= containedArea.IsOpened;
                if (!anyOpen)
                {
                    foreach (TiledArea containedArea in this.subLevelEntrancesAndContainedAreas.Values) containedArea.OnOpened += this.QueueForLevelSpawn;
                    levelSpawnQueue.Current.Dequeue();
                    if (levelSpawnQueue.Current.Count > 0) levelSpawnQueue.Current.Peek().ContinueLevelSpawnQueue();
                    return;
                }
            }

            LoadingOverlay.RequestLoad(
                () =>
                {
                    TiledLevelPlanner levelPlanner = this.levelPlanner.MutableValue;
                    FrigidCoroutine.Run(
                        this.Spawn(
                            levelPlanner.CreateLevelPlan(this.subLevelEntrancesAndContainedAreas),
                            () =>
                            {
                                if (spawnedLevels.Current.Add(this))
                                {
                                    onLevelSpawned?.Invoke(this, levelPlanner);
                                    if (TiledArea.TryGetFocusedArea(out TiledArea focusedArea) && this.wallBounds.Contains(focusedArea.CenterPosition))
                                    {
                                        currentFocusedLevel = this;
                                        onFocusedLevelChanged?.Invoke();
                                    }
                                    levelSpawnQueue.Current.Dequeue();
                                    if (levelSpawnQueue.Current.Count > 0) levelSpawnQueue.Current.Peek().ContinueLevelSpawnQueue();
                                    LoadingOverlay.ReleaseLoad();
                                }
                                else
                                {
                                    Debug.LogError("TiledLevel " + this.name + " seems to have been spawned twice.");
                                }
                            }
                            ),
                        this.gameObject
                        );
                }
                );
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(this.wallBounds.center, this.wallBounds.size);
        }
#endif

        private IEnumerator<FrigidCoroutine.Delay> Spawn(TiledLevelPlan plan, Action onComplete)
        {
            Dictionary<TiledLevelPlanArea, Vector2> localSpawnPositionPerPlanAreas = this.CalculateLocalSpawnPositionPerPlanAreas(plan);
            Bounds localBounds = this.CalculateLocalBounds(localSpawnPositionPerPlanAreas);
            Vector2 levelPosition = this.CalculateLevelPosition(localBounds);

            this.transform.position = levelPosition;
            this.wallBounds = new Bounds(localBounds.center + (Vector3)levelPosition, localBounds.size);
            this.containingAreas = new HashSet<TiledArea>();
            this.containingConnections = new HashSet<(TiledEntrance, TiledEntrance)>();
            Dictionary<TiledLevelPlanArea, TiledArea> spawnedAreasPerPlanAreas = new Dictionary<TiledLevelPlanArea, TiledArea>();
            Dictionary<TiledLevelPlanEntrance, TiledEntrance> entrancesPerPlanEntrances = new Dictionary<TiledLevelPlanEntrance, TiledEntrance>();

            yield return null;

            foreach (TiledLevelPlanArea planArea in plan.Areas)
            {
                TiledArea spawnedArea = CreateInstance<TiledArea>(planArea.ChosenBlueprint.AreaPrefab, this.transform.position + (Vector3)localSpawnPositionPerPlanAreas[planArea], this.transform);
                spawnedArea.Spawn(planArea.ChosenBlueprint, planArea == plan.StartingArea && !plan.IsSubLevel, this);
                spawnedAreasPerPlanAreas.Add(planArea, spawnedArea);
                this.ContainingAreas.Add(spawnedArea);
                yield return null;
            }

            foreach (TiledLevelPlanConnection planConnection in plan.Connections)
            {
                TiledEntrance firstEntrancePrefab = null;
                int firstEntranceTileIndex = -1;
                int firstEntranceWidth = -1;
                TiledEntrance secondEntrancePrefab = null;
                int secondEntranceTileIndex = -1;
                int secondEntranceWidth = -1;

                if (!planConnection.FirstEntrance.IsSubLevelEntrance && 
                    (!planConnection.FirstEntrance.Area.ChosenBlueprint.TryGetWallEntranceAssetAndIndexAndWidth(planConnection.IndexDirection, out TiledEntranceAsset firstEntranceAsset, out firstEntranceTileIndex, out firstEntranceWidth) || 
                    !firstEntranceAsset.TryGetEntrancePrefab(planConnection.ConnectionTerrain, planConnection.FirstEntrance.Area.BlueprintGroup, planConnection.SecondEntrance.IsSubLevelEntrance ? null : planConnection.SecondEntrance.Area.BlueprintGroup, out firstEntrancePrefab)) ||
                    !planConnection.SecondEntrance.IsSubLevelEntrance &&
                    (!planConnection.SecondEntrance.Area.ChosenBlueprint.TryGetWallEntranceAssetAndIndexAndWidth(-planConnection.IndexDirection, out TiledEntranceAsset secondEntranceAsset, out secondEntranceTileIndex, out secondEntranceWidth) ||
                    !secondEntranceAsset.TryGetEntrancePrefab(planConnection.ConnectionTerrain, planConnection.SecondEntrance.Area.BlueprintGroup, planConnection.FirstEntrance.IsSubLevelEntrance ? null : planConnection.FirstEntrance.Area.BlueprintGroup, out secondEntrancePrefab)))
                {
                    Debug.LogWarning("Tried to spawn TiledEntrance where prefab is not available.");
                    continue;
                }

                // Instantantiate/initialize first entrance
                if (planConnection.FirstEntrance.IsSubLevelEntrance)
                {
                    if (this.subLevelEntrancesAndContainedAreas.TryGetValue(planConnection.FirstEntrance.SublevelEntrance, out TiledArea containedArea))
                    {
                        planConnection.FirstEntrance.SublevelEntrance.Spawn(planConnection.ConnectionTerrain, containedArea);
                        containedArea.ContainingEntrances.Add(planConnection.FirstEntrance.SublevelEntrance);
                        entrancesPerPlanEntrances.Add(planConnection.FirstEntrance, planConnection.FirstEntrance.SublevelEntrance);
                    }
                    else
                    {
                        Debug.LogError("Should never reach here, there was an error at a previous step.");
                        break;
                    }
                }
                else
                {
                    TiledArea containedArea = spawnedAreasPerPlanAreas[planConnection.FirstEntrance.Area];

                    float zRotation = planConnection.IndexDirection.CartesianAngle() - 90;
                    Vector2 spawnPosition = WallTiling.EdgeExtentPositionFromWallIndexDirectionAndExtentIndex(planConnection.IndexDirection, firstEntranceTileIndex, containedArea.CenterPosition, containedArea.MainAreaDimensions, firstEntranceWidth);
                    TiledEntrance spawnedEntrance = CreateInstance<TiledEntrance>(firstEntrancePrefab, spawnPosition, Quaternion.Euler(0, 0, zRotation), containedArea.ContentsTransform);
                    spawnedEntrance.Spawn(planConnection.ConnectionTerrain, containedArea);
                    containedArea.ContainingEntrances.Add(spawnedEntrance);

                    entrancesPerPlanEntrances.Add(planConnection.FirstEntrance, spawnedEntrance);
                }

                // Instantantiate/initialize second entrance
                if (planConnection.SecondEntrance.IsSubLevelEntrance)
                {
                    if (this.subLevelEntrancesAndContainedAreas.TryGetValue(planConnection.SecondEntrance.SublevelEntrance, out TiledArea containedArea))
                    {
                        planConnection.SecondEntrance.SublevelEntrance.Spawn(planConnection.ConnectionTerrain, containedArea);
                        containedArea.ContainingEntrances.Add(planConnection.SecondEntrance.SublevelEntrance);
                        entrancesPerPlanEntrances.Add(planConnection.SecondEntrance, planConnection.SecondEntrance.SublevelEntrance);
                    }
                    else
                    {
                        Debug.LogError("Should never reach here, there was an error at a previous step.");
                        break;
                    }
                }
                else
                {
                    TiledArea containedArea = spawnedAreasPerPlanAreas[planConnection.SecondEntrance.Area];

                    float zRotation = (-planConnection.IndexDirection).CartesianAngle() - 90;
                    Vector2 localSpawnPosition = WallTiling.EdgeExtentLocalPositionFromWallIndexDirectionAndExtentIndex(-planConnection.IndexDirection, secondEntranceTileIndex, containedArea.MainAreaDimensions, secondEntranceWidth);
                    TiledEntrance spawnedEntrance = CreateInstance<TiledEntrance>(secondEntrancePrefab, containedArea.transform.position + (Vector3)localSpawnPosition, Quaternion.Euler(0, 0, zRotation), containedArea.ContentsTransform);
                    spawnedEntrance.Spawn(planConnection.ConnectionTerrain, containedArea);
                    containedArea.ContainingEntrances.Add(spawnedEntrance);

                    entrancesPerPlanEntrances.Add(planConnection.SecondEntrance, spawnedEntrance);
                }

                TiledEntrance firstEntrance = entrancesPerPlanEntrances[planConnection.FirstEntrance];
                TiledEntrance secondEntrance = entrancesPerPlanEntrances[planConnection.SecondEntrance];
                firstEntrance.ConnectTo(secondEntrance);
                this.ContainingConnections.Add((firstEntrance, secondEntrance));
                yield return null;
            }

            foreach (TiledLevelPlanArea planArea in plan.Areas)
            {
                for (int spawnerIndex = 0; spawnerIndex < planArea.ChosenBlueprint.GetNumberMobSpawners(); spawnerIndex++)
                {
                    HashSet<TiledAreaMobSpawnPoint> mobSpawnPoints = new HashSet<TiledAreaMobSpawnPoint>();
                    for (int spawnPointIndex = 0; spawnPointIndex < planArea.ChosenBlueprint.GetNumberMobSpawnPoints(spawnerIndex); spawnPointIndex++)
                    {
                        mobSpawnPoints.Add(planArea.ChosenBlueprint.GetMobSpawnPoint(spawnerIndex, spawnPointIndex));
                    }
                    planArea.ChosenBlueprint.GetMobSpawnerByReference(spawnerIndex).MutableValue.SpawnMobs(planArea, spawnedAreasPerPlanAreas[planArea], mobSpawnPoints);
                }
                yield return null;
            }

            onComplete.Invoke();
        }

        private void FindContainedAreaForEachSubLevelEntrance()
        {
            this.subLevelEntrancesAndContainedAreas = new Dictionary<TiledEntrance, TiledArea>();
            foreach (TiledEntrance subLevelEntrance in this.subLevelEntrances)
            {
                if (TiledArea.TryGetAreaAtPosition(subLevelEntrance.EntryPosition, out TiledArea containedArea))
                {
                    this.subLevelEntrancesAndContainedAreas.Add(subLevelEntrance, containedArea);
                }
                else
                {
                    throw new Exception("Sublevel entrance " + subLevelEntrance.name + " is not in a pre-existing TiledArea!");
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
                    TiledLevelPlanArea connectedPlanArea = null;
                    Vector2Int adjacentDirection = Vector2Int.zero;
                    if (planConnection.FirstEntrance.Area == dequeuedPlanArea) isConnected = true;
                    else
                    {
                        connectedPlanArea = planConnection.FirstEntrance.Area;
                        adjacentDirection = -planConnection.IndexDirection;
                    }

                    if (planConnection.SecondEntrance.Area == dequeuedPlanArea) isConnected = true;
                    else
                    {
                        connectedPlanArea = planConnection.SecondEntrance.Area;
                        adjacentDirection = planConnection.IndexDirection;
                    }

                    if (isConnected)
                    {
                        if (!localSpawnPositionPerPlanAreas.ContainsKey(connectedPlanArea))
                        {
                            queuedPlanAreas.Enqueue(connectedPlanArea);
                            localSpawnPositionPerPlanAreas.Add(
                                connectedPlanArea,
                                localSpawnPositionPerPlanAreas[dequeuedPlanArea] + new Vector2(adjacentDirection.x * wallAreaDimensionsMaxX, adjacentDirection.y * wallAreaDimensionsMaxY) * FrigidConstants.UNIT_WORLD_SIZE
                                );
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
                Vector2 wallExtents = new Vector2(localSpawnPositionPerPlanArea.Key.ChosenBlueprint.WallAreaDimensions.x, localSpawnPositionPerPlanArea.Key.ChosenBlueprint.WallAreaDimensions.y) * FrigidConstants.UNIT_WORLD_SIZE / 2;
                localBounds.Encapsulate(localSpawnPositionPerPlanArea.Value + wallExtents);
                localBounds.Encapsulate(localSpawnPositionPerPlanArea.Value - wallExtents);
            }
            return localBounds;
        }

        private Vector2 CalculateLevelPosition(Bounds localBounds)
        {
            Vector2 levelPosition = Vector2.zero;
            if (spawnedLevels.Current.Count > 0)
            {
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                foreach (TiledLevel tiledLevel in spawnedLevels.Current)
                {
                    if (tiledLevel.CenterPosition.x - tiledLevel.WallBoundsSize.x / 2 < minX)
                    {
                        minX = tiledLevel.CenterPosition.x - tiledLevel.WallBoundsSize.x / 2;
                    }
                    if (tiledLevel.CenterPosition.x + tiledLevel.WallBoundsSize.x / 2 > maxX)
                    {
                        maxX = tiledLevel.CenterPosition.x + tiledLevel.WallBoundsSize.x / 2;
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
    }
}
