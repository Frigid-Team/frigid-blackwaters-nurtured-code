using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class PathfindingTask
    {
        private TiledArea currentTiledArea;
        private Vector2Int currentRectDimensions;
        private TraversableTerrain currentTraversableTerrain;
        private Resistance currentTraversableResistance;

        private List<Vector2Int> currentPathIndexPositions;
        private float originalPathCost;
        private Vector2Int currentStartIndexPosition;
        private Vector2Int currentTargetIndexPosition;

        public PathfindingTask()
        {
            this.currentPathIndexPositions = new List<Vector2Int>();
            this.originalPathCost = 0;
        }

        public List<Vector2> RequestPathPoints(
            TiledArea tiledArea,
            Vector2Int rectDimensions,
            TraversableTerrain traversableTerrain,
            Resistance traversableResistance,
            Vector2 startPosition, 
            Vector2 targetPosition,
            Vector2 exitExtents,
            float maxPercentPathLengthDelta
            )
        {
            bool inputParamsChanged = this.currentTiledArea != tiledArea || this.currentRectDimensions != rectDimensions || this.currentTraversableTerrain != traversableTerrain || traversableResistance != this.currentTraversableResistance;
            if (inputParamsChanged)
            {
                this.currentTiledArea = tiledArea;
                this.currentRectDimensions = rectDimensions;
                this.currentTraversableTerrain = traversableTerrain;
                this.currentTraversableResistance = traversableResistance;
            }

            Vector2 topLeftCornerPosition = AreaTiling.RectPositionFromIndexPosition(this.currentRectDimensions - Vector2Int.one, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2 bottomRightCornerPosition = AreaTiling.RectPositionFromIndexPosition(this.currentTiledArea.MainAreaDimensions - Vector2Int.one, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);

            Vector2 newStartPosition = new Vector2(Mathf.Clamp(startPosition.x, topLeftCornerPosition.x, bottomRightCornerPosition.x), Mathf.Clamp(startPosition.y, bottomRightCornerPosition.y, topLeftCornerPosition.y));
            Vector2 newTargetPosition = new Vector2(Mathf.Clamp(targetPosition.x, topLeftCornerPosition.x, bottomRightCornerPosition.x), Mathf.Clamp(targetPosition.y, bottomRightCornerPosition.y, topLeftCornerPosition.y));
            Vector2Int newStartIndexPosition = AreaTiling.RectIndexPositionFromPosition(newStartPosition, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2Int newTargetIndexPosition = AreaTiling.RectIndexPositionFromPosition(newTargetPosition, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2 previousStartingPosition = AreaTiling.RectPositionFromIndexPosition(this.currentStartIndexPosition, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2 previousTargetPosition = AreaTiling.RectPositionFromIndexPosition(this.currentTargetIndexPosition, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2Int previousStartIndexPosition = this.currentStartIndexPosition;
            Vector2Int previousTargetIndexPosition = this.currentTargetIndexPosition;

            List<Vector2> ConstructPathPositions()
            {
                List<Vector2> pathPositions = new List<Vector2>();
                foreach (Vector2Int currentPathIndexPosition in this.currentPathIndexPositions)
                {
                    pathPositions.Add(AreaTiling.RectPositionFromIndexPosition(currentPathIndexPosition, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions));
                }
                return pathPositions;
            }

            if (!inputParamsChanged)
            {
                if (previousStartIndexPosition == newStartIndexPosition && previousTargetIndexPosition == newTargetIndexPosition)
                {
                    return ConstructPathPositions();
                }

                if (this.currentPathIndexPositions.Count > 0 &&
                    this.currentTiledArea.NavigationGrid.IsTraversable(newStartIndexPosition, this.currentRectDimensions, this.currentTraversableTerrain, this.currentTraversableResistance) &&
                    this.currentTiledArea.NavigationGrid.IsTraversable(newTargetIndexPosition, this.currentRectDimensions, this.currentTraversableTerrain, this.currentTraversableResistance))
                {
                    bool startTooFar = true;
                    if (Mathf.Abs(newStartPosition.x - previousStartingPosition.x) > FrigidConstants.UnitWorldSize / 2 + exitExtents.x ||
                        Mathf.Abs(newStartPosition.y - previousStartingPosition.y) > FrigidConstants.UnitWorldSize / 2 + exitExtents.y)
                    {
                        List<Vector2Int> adjacentIndexPositionsToStart = this.currentTiledArea.NavigationGrid.GetAdjacentTraversableIndexPositions(newStartIndexPosition, this.currentRectDimensions, this.currentTraversableTerrain, this.currentTraversableResistance);
                        for (int i = this.currentPathIndexPositions.Count - 1; i >= 0; i--)
                        {
                            // Checks if any of the current path is the new start or adjacent to it.
                            if (this.currentPathIndexPositions[i] == newStartIndexPosition)
                            {
                                this.currentPathIndexPositions.RemoveRange(0, i);
                                this.currentStartIndexPosition = newStartIndexPosition;
                                startTooFar = false;
                                break;
                            }
                            else if (adjacentIndexPositionsToStart.Contains(this.currentPathIndexPositions[i]))
                            {
                                this.currentPathIndexPositions.RemoveRange(0, i);
                                this.currentPathIndexPositions.Insert(0, newStartIndexPosition);
                                this.currentStartIndexPosition = newStartIndexPosition;
                                startTooFar = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        startTooFar = false;
                    }

                    bool targetTooFar = true;
                    if (Mathf.Abs(newTargetPosition.x - previousTargetPosition.x) > FrigidConstants.UnitWorldSize / 2 + exitExtents.x ||
                        Mathf.Abs(newTargetPosition.y - previousTargetPosition.y) > FrigidConstants.UnitWorldSize / 2 + exitExtents.y)
                    {
                        List<Vector2Int> adjacentIndexPositionsToTarget = this.currentTiledArea.NavigationGrid.GetAdjacentTraversableIndexPositions(newTargetIndexPosition, this.currentRectDimensions, this.currentTraversableTerrain, this.currentTraversableResistance);
                        for (int i = 0; i < this.currentPathIndexPositions.Count; i++)
                        {
                            // Checks if any of the current path is the new target or adjacent to it.
                            if (this.currentPathIndexPositions[i] == newTargetIndexPosition)
                            {
                                this.currentPathIndexPositions.RemoveRange(i + 1, this.currentPathIndexPositions.Count - i - 1);
                                this.currentTargetIndexPosition = newTargetIndexPosition;
                                targetTooFar = false;
                                break;
                            }
                            else if (adjacentIndexPositionsToTarget.Contains(this.currentPathIndexPositions[i]))
                            {
                                this.currentPathIndexPositions.RemoveRange(i + 1, this.currentPathIndexPositions.Count - i - 1);
                                this.currentPathIndexPositions.Add(newTargetIndexPosition);
                                this.currentTargetIndexPosition = newTargetIndexPosition;
                                targetTooFar = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        targetTooFar = false;
                    }

                    float currentPathCost = 0;
                    for (int i = 1; i < this.currentPathIndexPositions.Count; i++)
                    {
                        currentPathCost += Vector2Int.Distance(this.currentPathIndexPositions[i], this.currentPathIndexPositions[i - 1]);
                    }
                    float pathCostChangePercent = currentPathCost / this.originalPathCost;
                    if (!startTooFar && !targetTooFar && Mathf.Abs(pathCostChangePercent - 1) < maxPercentPathLengthDelta)
                    {
                        return ConstructPathPositions();
                    }
                }
            }

            this.currentPathIndexPositions = this.currentTiledArea.NavigationGrid.FindPathIndexPositions(newStartIndexPosition, newTargetIndexPosition, this.currentRectDimensions, this.currentTraversableTerrain, this.currentTraversableResistance);
            this.originalPathCost = 0;
            for (int i = 1; i < this.currentPathIndexPositions.Count; i++)
            {
                this.originalPathCost += Vector2Int.Distance(this.currentPathIndexPositions[i], this.currentPathIndexPositions[i - 1]);
            }
            this.currentStartIndexPosition = newStartIndexPosition;
            this.currentTargetIndexPosition = newTargetIndexPosition;

            return ConstructPathPositions();
        }
    }
}
