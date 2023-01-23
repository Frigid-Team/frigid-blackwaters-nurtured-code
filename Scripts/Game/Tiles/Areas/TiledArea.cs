using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledArea : FrigidMonoBehaviour
    {
        private static SceneVariable<HashSet<TiledArea>> spawnedTiledAreas;
        private static Action<TiledArea> onTiledAreaSpawned;
        private static SceneVariable<List<TiledArea>> tiledAreasOrderedByDurationOpened;
        private static Action onFocusedTiledAreaChanged;

        private const float TRANSITION_DURATION = 0.75f;

        [SerializeField]
        private Transform contentsTransform;
        [SerializeField]
        private WallsPopulator wallsPopulator;
        [SerializeField]
        private TerrainPopulator terrainPopulator;
        [SerializeField]
        private TerrainContentPopulator terrainContentPopulator;
        [SerializeField]
        private WallContentPopulator wallContentPopulator;
        [SerializeField]
        private TiledAreaTransitioner tiledAreaTransitioner;
        [SerializeField]
        private List<TiledAreaAtmosphere> atmospheres;

        private NavigationGrid navigationGrid;

        private Vector2Int mainAreaDimensions;
        private Vector2Int wallAreaDimensions;

        private Action onOpened;
        private Action onClosed;
        private Action onTransitionStarted;
        private Action onTransitionFinished;
        private bool isOpened;
        private bool isTransitioning;

        private HashSet<TiledAreaEntrance> placedEntrances;

        static TiledArea()
        {
            spawnedTiledAreas = new SceneVariable<HashSet<TiledArea>>(() => { return new HashSet<TiledArea>(); });
            tiledAreasOrderedByDurationOpened = new SceneVariable<List<TiledArea>>(() => { return new List<TiledArea>(); });
        }

        public static HashSet<TiledArea> SpawnedTiledAreas
        {
            get
            {
                return spawnedTiledAreas.Current;
            }
        }

        public static Action<TiledArea> OnTiledAreaSpawned
        {
            get
            {
                return onTiledAreaSpawned;
            }
            set
            {
                onTiledAreaSpawned = value;
            }
        }

        public static Action OnFocusedTiledAreaChanged
        {
            get
            {
                return onFocusedTiledAreaChanged;
            }
            set
            {
                onFocusedTiledAreaChanged = value;
            }
        }

        public Transform ContentsTransform
        {
            get
            {
                return this.contentsTransform;
            }
        }

        public NavigationGrid NavigationGrid
        {
            get
            {
                return this.navigationGrid;
            }
        }

        public Vector2 AbsoluteCenterPosition
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

        public HashSet<TiledAreaEntrance> PlacedEntrances
        {
            get
            {
                return this.placedEntrances;
            }
        }

        public static bool TryGetTiledAreaAtPosition(Vector2 currentPosition, out TiledArea tiledArea)
        {
            tiledArea = null;
            foreach (TiledArea spawnedTiledArea in spawnedTiledAreas.Current)
            {
                if (TilePositioning.TileAbsolutePositionWithinBounds(currentPosition, spawnedTiledArea.AbsoluteCenterPosition, spawnedTiledArea.WallAreaDimensions))
                {
                    tiledArea = spawnedTiledArea;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetFocusedTiledArea(out TiledArea focusedTiledArea)
        {
            focusedTiledArea = null;
            if (tiledAreasOrderedByDurationOpened.Current.Count > 0)
            {
                focusedTiledArea = tiledAreasOrderedByDurationOpened.Current[0];
                return true;
            }
            return false;
        }

        public void PlaceEntrance(TiledAreaEntrance tiledAreaEntrance)
        {
            if (this.placedEntrances.Add(tiledAreaEntrance))
            {
                tiledAreaEntrance.OnEntered += TransitionTo;
                tiledAreaEntrance.OnExited += TransitionAway;
            }
        }

        public void Populate(TiledAreaBlueprint tiledAreaBlueprint, bool isFirstTiledArea)
        {
            this.mainAreaDimensions = tiledAreaBlueprint.MainAreaDimensions;
            this.wallAreaDimensions = tiledAreaBlueprint.WallAreaDimensions;

            this.navigationGrid = new NavigationGrid(tiledAreaBlueprint);
            this.wallsPopulator.PopulateWalls(tiledAreaBlueprint, this.contentsTransform);
            this.terrainPopulator.PopulateTerrain(tiledAreaBlueprint, this.contentsTransform);
            this.terrainContentPopulator.PopulateTerrainContent(tiledAreaBlueprint, this.contentsTransform, this.navigationGrid);
            this.wallContentPopulator.PopulateWallContent(tiledAreaBlueprint, this.contentsTransform);
            this.tiledAreaTransitioner.SetDimensions(tiledAreaBlueprint.WallAreaDimensions);

            spawnedTiledAreas.Current.Add(this);
            onTiledAreaSpawned?.Invoke(this);
            if (isFirstTiledArea) OpenTiledArea();
            else CloseTiledArea();
        }

        public void TransitionTo(TiledAreaTransition transition, Vector2 entryPosition)
        {
            if (this.isTransitioning) return;

            FrigidCoroutine.Run(
                TweenCoroutine.DelayedCall(
                    TRANSITION_DURATION,
                    () =>
                    {
                        OpenTiledArea();
                        this.isTransitioning = true;
                        this.onTransitionStarted?.Invoke();
                        this.tiledAreaTransitioner.PlayTransitionTo(transition, TRANSITION_DURATION, entryPosition);
                        FrigidCoroutine.Run(
                            TweenCoroutine.DelayedCall(
                                TRANSITION_DURATION,
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
            if (this.isTransitioning) return;

            this.isTransitioning = true;
            this.onTransitionStarted?.Invoke();
            this.tiledAreaTransitioner.PlayTransitionAway(transition, TRANSITION_DURATION, exitPosition);
            FrigidCoroutine.Run(
                TweenCoroutine.DelayedCall(
                    TRANSITION_DURATION,
                    () =>
                    {
                        this.onTransitionFinished?.Invoke();
                        this.isTransitioning = false;
                        CloseTiledArea();
                    }
                    ),
                this.gameObject
                );
        }

        protected override void Awake()
        {
            base.Awake();
            this.isOpened = false;
            this.placedEntrances = new HashSet<TiledAreaEntrance>();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void OpenTiledArea()
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

            bool focusChanged = tiledAreasOrderedByDurationOpened.Current.Count == 0;
            tiledAreasOrderedByDurationOpened.Current.Add(this);
            if (focusChanged)
            {
                onFocusedTiledAreaChanged?.Invoke();
            }
        }

        private void CloseTiledArea()
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

            bool focusChanged = tiledAreasOrderedByDurationOpened.Current[0] == this;
            tiledAreasOrderedByDurationOpened.Current.Remove(this);
            if (focusChanged)
            {
                onFocusedTiledAreaChanged?.Invoke();
            }
        }
    }
}
