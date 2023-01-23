using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class PathfindingTask
    {
        private TiledArea currentTiledArea;
        private Vector2Int currentRectDimensions;
        private TraversableTerrain currentTraversableTerrain;

        private List<Vector2Int> currentPathIndices;
        private float originalPathCost;

        public PathfindingTask()
        {
            this.currentPathIndices = new List<Vector2Int>();
            this.originalPathCost = 0;
        }

        public List<Vector2> RequestPathPoints(
            TiledArea tiledArea,
            Vector2Int rectDimensions,
            TraversableTerrain traversableTerrain,
            Vector2 startAbsolutePosition, 
            Vector2 targetAbsolutePosition,
            float exitExtent,
            float percentPathExtensionForPathFind
            )
        {
            Vector2 topLeftCornerAbsolutePosition = TilePositioning.RectAbsolutePositionFromIndices(this.currentRectDimensions - Vector2Int.one, this.currentTiledArea.AbsoluteCenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2 bottomRightCornerAbsolutePosition = TilePositioning.RectAbsolutePositionFromIndices(this.currentTiledArea.MainAreaDimensions - Vector2Int.one, this.currentTiledArea.AbsoluteCenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);

            Vector2 newStartAbsolutePosition = new Vector2(Mathf.Clamp(startAbsolutePosition.x, topLeftCornerAbsolutePosition.x, bottomRightCornerAbsolutePosition.x), Mathf.Clamp(startAbsolutePosition.y, bottomRightCornerAbsolutePosition.y, topLeftCornerAbsolutePosition.y));
            Vector2 newTargetAbsolutePosition = new Vector2(Mathf.Clamp(targetAbsolutePosition.x, topLeftCornerAbsolutePosition.x, bottomRightCornerAbsolutePosition.x), Mathf.Clamp(targetAbsolutePosition.y, bottomRightCornerAbsolutePosition.y, topLeftCornerAbsolutePosition.y));
            Vector2Int newStartIndices = TilePositioning.RectIndicesFromAbsolutePosition(newStartAbsolutePosition, this.currentTiledArea.AbsoluteCenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2Int newTargetIndices = TilePositioning.RectIndicesFromAbsolutePosition(newTargetAbsolutePosition, this.currentTiledArea.AbsoluteCenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);

            List<Vector2> ConstructPathPositions()
            {
                List<Vector2> pathPositions = new List<Vector2>();
                foreach (Vector2Int pathIndices in this.currentPathIndices)
                {
                    pathPositions.Add(TilePositioning.RectAbsolutePositionFromIndices(pathIndices, this.currentTiledArea.AbsoluteCenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions));
                }
                return pathPositions;
            }

            if (this.currentTiledArea == tiledArea &&
                this.currentRectDimensions == rectDimensions &&
                this.currentTraversableTerrain == traversableTerrain &&
                this.currentPathIndices.Count > 0 && 
                this.currentTiledArea.NavigationGrid.IsTraversable(newStartIndices, this.currentRectDimensions, this.currentTraversableTerrain) &&
                this.currentTiledArea.NavigationGrid.IsTraversable(newTargetIndices, this.currentRectDimensions, this.currentTraversableTerrain))
            {
                Vector2Int currentStartIndices = this.currentPathIndices[0];
                Vector2Int currentTargetIndices = this.currentPathIndices[this.currentPathIndices.Count - 1];
                Vector2 currentStartingPosition = TilePositioning.RectAbsolutePositionFromIndices(currentStartIndices, this.currentTiledArea.AbsoluteCenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
                Vector2 currentTargetAbsolutePosition = TilePositioning.RectAbsolutePositionFromIndices(currentTargetIndices, this.currentTiledArea.AbsoluteCenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);

                bool startTooFar = true;
                if (Mathf.Abs(newStartAbsolutePosition.x - currentStartingPosition.x) > GameConstants.UNIT_WORLD_SIZE / 2 + exitExtent ||
                    Mathf.Abs(newStartAbsolutePosition.y - currentStartingPosition.y) > GameConstants.UNIT_WORLD_SIZE / 2 + exitExtent)
                {
                    List<Vector2Int> adjacentIndicesToNewStart = this.currentTiledArea.NavigationGrid.GetAdjacentTraversableIndices(newStartIndices, this.currentRectDimensions, this.currentTraversableTerrain);
                    for (int i = this.currentPathIndices.Count - 1; i >= 0; i--)
                    {
                        // Checks if any of the current path is the new start or adjacent to it.
                        if (this.currentPathIndices[i] == newStartIndices)
                        {
                            this.currentPathIndices.RemoveRange(0, i);
                            startTooFar = false;
                            break;
                        }
                        else if (adjacentIndicesToNewStart.Contains(this.currentPathIndices[i]))
                        {
                            this.currentPathIndices.RemoveRange(0, i);
                            this.currentPathIndices.Insert(0, newStartIndices);
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
                if (Mathf.Abs(newTargetAbsolutePosition.x - currentTargetAbsolutePosition.x) > GameConstants.UNIT_WORLD_SIZE / 2 + exitExtent ||
                    Mathf.Abs(newTargetAbsolutePosition.y - currentTargetAbsolutePosition.y) > GameConstants.UNIT_WORLD_SIZE / 2 + exitExtent)
                {
                    List<Vector2Int> adjacentIndicesToNewTarget = this.currentTiledArea.NavigationGrid.GetAdjacentTraversableIndices(newTargetIndices, this.currentRectDimensions, this.currentTraversableTerrain);
                    for (int i = 0; i < this.currentPathIndices.Count; i++)
                    {
                        // Checks if any of the current path is the new target or adjacent to it.
                        if (this.currentPathIndices[i] == newTargetIndices)
                        {
                            this.currentPathIndices.RemoveRange(i + 1, this.currentPathIndices.Count - i - 1);
                            targetTooFar = false;
                            break;
                        }
                        else if (adjacentIndicesToNewTarget.Contains(this.currentPathIndices[i]))
                        {
                            this.currentPathIndices.RemoveRange(i + 1, this.currentPathIndices.Count - i - 1);
                            this.currentPathIndices.Add(newTargetIndices);
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
                for (int i = 1; i < this.currentPathIndices.Count; i++)
                {
                    currentPathCost += Vector2Int.Distance(this.currentPathIndices[i], this.currentPathIndices[i - 1]);
                }

                if (!startTooFar && !targetTooFar && currentPathCost / this.originalPathCost < 1f + percentPathExtensionForPathFind)
                {
                    return ConstructPathPositions();
                }
            }

            this.currentTiledArea = tiledArea;
            this.currentRectDimensions = rectDimensions;
            this.currentTraversableTerrain = traversableTerrain;

            this.currentPathIndices = this.currentTiledArea.NavigationGrid.FindPathIndices(newStartIndices, newTargetIndices, this.currentRectDimensions, this.currentTraversableTerrain);
            this.originalPathCost = 0;
            for (int i = 1; i < this.currentPathIndices.Count; i++)
            {
                this.originalPathCost += Vector2Int.Distance(this.currentPathIndices[i], this.currentPathIndices[i - 1]);
            }

            return ConstructPathPositions();
        }
    }
}
