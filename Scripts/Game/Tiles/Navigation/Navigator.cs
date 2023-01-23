using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class Navigator : FrigidMonoBehaviour
    {
        private const float NAVIGATOR_MAX_AVOIDANCE_DISTANCE = 3f;
        private const float NAVIGATOR_MIN_AVOIDANCE_DISTANCE = 1.5f;

        [SerializeField]
        private TiledAreaOccupier tiledAreaOccupier;
        [SerializeField]
        private bool pathAroundMe;
        [SerializeField]
        private bool pathAroundOthers;
        [SerializeField]
        private Vector2SerializedReference exitExtents;
        [SerializeField]
        [Range(0, 1)]
        private float distanceToTargetPreferenceRatio;

        private Vector2 currentTileAbsolutePosition;
        private Dictionary<TraversableTerrain, Dictionary<Vector2Int, Vector2>> savedPathDirections;

        public bool PathAroundMe
        {
            get
            {
                return this.pathAroundMe;
            }
        }

        public bool PathAroundOthers
        {
            get
            {
                return this.pathAroundOthers;
            }
        }

        public void AttendMyNavigationGrid()
        {
            if (this.tiledAreaOccupier.TryGetCurrentTiledArea(out TiledArea navigatingTiledArea))
            {
                // navigatingTiledArea.NavigationGrid.AddNavigator(this);
            }
            this.tiledAreaOccupier.OnCurrentTiledAreaChanged += RenewAttendanceInNavigationGrid;
        }

        public void UnattendMyNavigationGrid()
        {
            if (this.tiledAreaOccupier.TryGetCurrentTiledArea(out TiledArea navigatingTiledArea))
            {
                // navigatingTiledArea.NavigationGrid.RemoveNavigator(this);
            }
            this.tiledAreaOccupier.OnCurrentTiledAreaChanged -= RenewAttendanceInNavigationGrid;
        }

        public bool TryNavigate(Vector2 targetAbsolutePosition, TraversableTerrain traversableTerrain, out Vector2 navigationDirection)
        {
            if (!this.tiledAreaOccupier.TryGetCurrentTiledArea(out TiledArea navigatingTiledArea))
            {
                navigationDirection = Vector2.zero;
                return false;
            }

            if (TryToUpdateCurrentTilePosition()) this.savedPathDirections.Clear();

            if (!this.savedPathDirections.ContainsKey(traversableTerrain))
            {
                this.savedPathDirections.Add(traversableTerrain, new Dictionary<Vector2Int, Vector2>());
            }

            Vector2Int startIndices = TilePositioning.TileIndicesFromAbsolutePosition(
                this.currentTileAbsolutePosition, 
                navigatingTiledArea.AbsoluteCenterPosition, 
                navigatingTiledArea.MainAreaDimensions
                );
            Vector2Int targetIndices = TilePositioning.TileIndicesFromAbsolutePosition(
                targetAbsolutePosition,
                navigatingTiledArea.AbsoluteCenterPosition,
                navigatingTiledArea.MainAreaDimensions
                );

            Vector2 pathDirection;
            if (this.savedPathDirections[traversableTerrain].ContainsKey(targetIndices)) 
            {
                pathDirection = this.savedPathDirections[traversableTerrain][targetIndices];
            }
            else 
            {
                List<Vector2Int> path = new List<Vector2Int>();
                    // Pathfinder.FindPathIndices(navigatingTiledArea.NavigationGrid, traversableTerrain, startIndices, targetIndices, this.distanceToTargetPreferenceRatio);
                if (path.Count == 0)
                {
                    pathDirection = Vector2.zero;
                }
                else
                {
                    Vector2 nextTileAbsolutePosition = TilePositioning.TileAbsolutePositionFromIndices(
                        path[0],
                        navigatingTiledArea.AbsoluteCenterPosition,
                        navigatingTiledArea.MainAreaDimensions
                        );
                    pathDirection = (nextTileAbsolutePosition - (Vector2)this.transform.position).normalized;
                }
                this.savedPathDirections[traversableTerrain].Add(targetIndices, pathDirection);
            }

            Vector2 summedDirection = pathDirection;
            if (summedDirection.magnitude > 0)
            {
                if (this.pathAroundOthers)
                {
                    /*
                    navigatingTiledArea.NavigationGrid.VisitNavigators(
                        (Navigator navigator) =>
                        {
                            if (navigator != this && navigator.PathAroundMe)
                            {
                                Vector2 offsetDirection = navigator.transform.position - this.transform.position;
                                float angle = Vector2.Angle(offsetDirection, pathDirection);

                                if (angle <= 90 && offsetDirection.magnitude <= NAVIGATOR_MAX_AVOIDANCE_DISTANCE)
                                {
                                    float weightMultiplier = 1 - Mathf.Abs(Vector2.Dot(offsetDirection, pathDirection));
                                    Vector2 avoidanceDirection = 
                                        weightMultiplier * -offsetDirection.normalized * 
                                        (NAVIGATOR_MAX_AVOIDANCE_DISTANCE - offsetDirection.magnitude) / 
                                        (NAVIGATOR_MAX_AVOIDANCE_DISTANCE - NAVIGATOR_MIN_AVOIDANCE_DISTANCE);
                                    summedDirection += avoidanceDirection;
                                    summedDirection.Normalize();
                                }
                            }
                        }
                        );
                    */
                }
            }
            navigationDirection = summedDirection;
            return summedDirection.magnitude != 0;
        }

        protected override void Awake()
        {
            base.Awake();
            this.savedPathDirections = new Dictionary<TraversableTerrain, Dictionary<Vector2Int, Vector2>>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCurrentTilePosition();
            this.tiledAreaOccupier.OnCurrentTiledAreaChanged += RenewSavedPathDirections;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.tiledAreaOccupier.OnCurrentTiledAreaChanged -= RenewSavedPathDirections;
        }

        private void RenewSavedPathDirections(bool hadOldTiledArea, TiledArea oldTiledArea, bool hasNewTiledArea, TiledArea newTiledArea)
        {
            this.savedPathDirections.Clear();
        }

        private void RenewAttendanceInNavigationGrid(bool hadOldTiledArea, TiledArea oldTiledArea, bool hasNewTiledArea, TiledArea newTiledArea)
        {
            // if (hadOldTiledArea) oldTiledArea.NavigationGrid.RemoveNavigator(this);
            // if (hasNewTiledArea) newTiledArea.NavigationGrid.AddNavigator(this);
        }

        private bool TryToUpdateCurrentTilePosition()
        {
            if (Mathf.Abs(this.transform.position.x - this.currentTileAbsolutePosition.x) > GameConstants.UNIT_WORLD_SIZE / 2 + this.exitExtents.ImmutableValue.x ||
                Mathf.Abs(this.transform.position.y - this.currentTileAbsolutePosition.y) > GameConstants.UNIT_WORLD_SIZE / 2 + this.exitExtents.ImmutableValue.x)
            {
                UpdateCurrentTilePosition();
                return true;
            }
            return false;
        }

        private void UpdateCurrentTilePosition()
        {
            if (this.tiledAreaOccupier.TryGetCurrentTiledArea(out TiledArea navigatingTiledArea))
            {
                Vector2Int currentTileIndices = TilePositioning.TileIndicesFromAbsolutePosition(
                    this.transform.position,
                    navigatingTiledArea.AbsoluteCenterPosition,
                    navigatingTiledArea.MainAreaDimensions
                    );
                this.currentTileAbsolutePosition = TilePositioning.TileAbsolutePositionFromIndices(
                    currentTileIndices,
                    navigatingTiledArea.AbsoluteCenterPosition,
                    navigatingTiledArea.MainAreaDimensions
                    );
            }
        }
    }
}
