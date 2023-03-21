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
        private Vector2Int currentStartIndices;
        private Vector2Int currentTargetIndices;

        public PathfindingTask()
        {
            this.currentPathIndices = new List<Vector2Int>();
            this.originalPathCost = 0;
        }

        public List<Vector2> RequestPathPoints(
            TiledArea tiledArea,
            Vector2Int rectDimensions,
            TraversableTerrain traversableTerrain,
            Vector2 startPosition, 
            Vector2 targetPosition,
            Vector2 exitExtents,
            float percentPathExtensionForPathFind
            )
        {
            bool inputParamsChanged = this.currentTiledArea != tiledArea || this.currentRectDimensions != rectDimensions || this.currentTraversableTerrain != traversableTerrain;
            if (inputParamsChanged)
            {
                this.currentTiledArea = tiledArea;
                this.currentRectDimensions = rectDimensions;
                this.currentTraversableTerrain = traversableTerrain;
            }

            Vector2 topLeftCornerPosition = TilePositioning.RectPositionFromIndices(this.currentRectDimensions - Vector2Int.one, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2 bottomRightCornerPosition = TilePositioning.RectPositionFromIndices(this.currentTiledArea.MainAreaDimensions - Vector2Int.one, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);

            Vector2 newStartPosition = new Vector2(Mathf.Clamp(startPosition.x, topLeftCornerPosition.x, bottomRightCornerPosition.x), Mathf.Clamp(startPosition.y, bottomRightCornerPosition.y, topLeftCornerPosition.y));
            Vector2 newTargetPosition = new Vector2(Mathf.Clamp(targetPosition.x, topLeftCornerPosition.x, bottomRightCornerPosition.x), Mathf.Clamp(targetPosition.y, bottomRightCornerPosition.y, topLeftCornerPosition.y));
            Vector2Int newStartIndices = TilePositioning.RectIndicesFromPosition(newStartPosition, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2Int newTargetIndices = TilePositioning.RectIndicesFromPosition(newTargetPosition, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2 previousStartingPosition = TilePositioning.RectPositionFromIndices(this.currentStartIndices, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2 previousTargetPosition = TilePositioning.RectPositionFromIndices(this.currentTargetIndices, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions);
            Vector2Int previousStartIndices = this.currentStartIndices;
            Vector2Int previousTargetIndices = this.currentTargetIndices;

            List<Vector2> ConstructPathPositions()
            {
                List<Vector2> pathPositions = new List<Vector2>();
                foreach (Vector2Int pathIndices in this.currentPathIndices)
                {
                    pathPositions.Add(TilePositioning.RectPositionFromIndices(pathIndices, this.currentTiledArea.CenterPosition, this.currentTiledArea.MainAreaDimensions, this.currentRectDimensions));
                }
                return pathPositions;
            }

            if (!inputParamsChanged)
            {
                if (previousStartIndices == newStartIndices && previousTargetIndices == newTargetIndices)
                {
                    return ConstructPathPositions();
                }

                if (this.currentPathIndices.Count > 0 &&
                    this.currentTiledArea.NavigationGrid.IsTraversable(newStartIndices, this.currentRectDimensions, this.currentTraversableTerrain) &&
                    this.currentTiledArea.NavigationGrid.IsTraversable(newTargetIndices, this.currentRectDimensions, this.currentTraversableTerrain))
                {
                    bool startTooFar = true;
                    if (Mathf.Abs(newStartPosition.x - previousStartingPosition.x) > GameConstants.UNIT_WORLD_SIZE / 2 + exitExtents.x ||
                        Mathf.Abs(newStartPosition.y - previousStartingPosition.y) > GameConstants.UNIT_WORLD_SIZE / 2 + exitExtents.y)
                    {
                        List<Vector2Int> adjacentIndicesToNewStart = this.currentTiledArea.NavigationGrid.GetAdjacentTraversableIndices(newStartIndices, this.currentRectDimensions, this.currentTraversableTerrain);
                        for (int i = this.currentPathIndices.Count - 1; i >= 0; i--)
                        {
                            // Checks if any of the current path is the new start or adjacent to it.
                            if (this.currentPathIndices[i] == newStartIndices)
                            {
                                this.currentPathIndices.RemoveRange(0, i);
                                this.currentStartIndices = newStartIndices;
                                startTooFar = false;
                                break;
                            }
                            else if (adjacentIndicesToNewStart.Contains(this.currentPathIndices[i]))
                            {
                                this.currentPathIndices.RemoveRange(0, i);
                                this.currentPathIndices.Insert(0, newStartIndices);
                                this.currentStartIndices = newStartIndices;
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
                    if (Mathf.Abs(newTargetPosition.x - previousTargetPosition.x) > GameConstants.UNIT_WORLD_SIZE / 2 + exitExtents.x ||
                        Mathf.Abs(newTargetPosition.y - previousTargetPosition.y) > GameConstants.UNIT_WORLD_SIZE / 2 + exitExtents.y)
                    {
                        List<Vector2Int> adjacentIndicesToNewTarget = this.currentTiledArea.NavigationGrid.GetAdjacentTraversableIndices(newTargetIndices, this.currentRectDimensions, this.currentTraversableTerrain);
                        for (int i = 0; i < this.currentPathIndices.Count; i++)
                        {
                            // Checks if any of the current path is the new target or adjacent to it.
                            if (this.currentPathIndices[i] == newTargetIndices)
                            {
                                this.currentPathIndices.RemoveRange(i + 1, this.currentPathIndices.Count - i - 1);
                                this.currentTargetIndices = newTargetIndices;
                                targetTooFar = false;
                                break;
                            }
                            else if (adjacentIndicesToNewTarget.Contains(this.currentPathIndices[i]))
                            {
                                this.currentPathIndices.RemoveRange(i + 1, this.currentPathIndices.Count - i - 1);
                                this.currentPathIndices.Add(newTargetIndices);
                                this.currentTargetIndices = newTargetIndices;
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
            }

            this.currentPathIndices = this.currentTiledArea.NavigationGrid.FindPathIndices(newStartIndices, newTargetIndices, this.currentRectDimensions, this.currentTraversableTerrain);
            this.originalPathCost = 0;
            for (int i = 1; i < this.currentPathIndices.Count; i++)
            {
                this.originalPathCost += Vector2Int.Distance(this.currentPathIndices[i], this.currentPathIndices[i - 1]);
            }
            this.currentStartIndices = newStartIndices;
            this.currentTargetIndices = newTargetIndices;

            return ConstructPathPositions();
        }
    }
}
