using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class NavigationGrid
    {
        private NavigationTile[][] grid;
        private Vector2Int dimensions;

        public NavigationGrid(TiledAreaBlueprint blueprint)
        {
            this.dimensions = blueprint.MainAreaDimensions;
            this.grid = new NavigationTile[this.dimensions.x][];
            for (int x = 0; x < this.dimensions.x; x++)
            {
                this.grid[x] = new NavigationTile[this.dimensions.y];
                for (int y = 0; y < this.dimensions.y; y++)
                {
                    NavigationTile tile = new NavigationTile(blueprint.GetTerrainTileAssetAt(new Vector2Int(x, y)).Terrain);
                    this.grid[x][y] = tile;
                }
            }
        }

        public NavigationTile this[Vector2Int indexPosition]
        {
            get
            {
                return this.grid[indexPosition.x][indexPosition.y];
            }
        }

        public Vector2Int Dimensions
        {
            get
            {
                return this.dimensions;
            }
        }

        public bool IsOnTerrain(Vector2Int indexPosition, Vector2Int rectDimensions, TraversableTerrain traversableTerrain)
        {
            if (!AreaTiling.RectIndexPositionWithinBounds(indexPosition, this.Dimensions, rectDimensions)) return false;
            bool onTerrain = true;
            return AreaTiling.VisitTileIndexPositionsInTileRect(
                indexPosition,
                rectDimensions,
                this.Dimensions,
                (Vector2Int indexPosition) =>
                {
                    NavigationTile navigationTile = this[indexPosition];
                    onTerrain &= traversableTerrain.Includes(navigationTile.Terrain);
                }
                ) && onTerrain;
        }

        public bool IsTraversable(Vector2Int indexPosition, Vector2Int rectDimensions, TraversableTerrain traversableTerrain, Resistance traversableResistance)
        {
            if (!AreaTiling.RectIndexPositionWithinBounds(indexPosition, this.Dimensions, rectDimensions)) return false;
            bool isTraversable = true;
            return AreaTiling.VisitTileIndexPositionsInTileRect(
                indexPosition,
                rectDimensions,
                this.Dimensions,
                (Vector2Int indexPosition) =>
                {
                    NavigationTile navigationTile = this[indexPosition];
                    isTraversable &= (navigationTile.Unobstructed || traversableResistance >= navigationTile.HighestObstructiveResistance) && traversableTerrain.Includes(navigationTile.Terrain);
                }
                ) && isTraversable;
        }

        public List<Vector2Int> GetAdjacentTraversableIndexPositions(Vector2Int originIndexPosition, Vector2Int rectDimensions, TraversableTerrain traversableTerrain, Resistance traversableResistance)
        {
            List<Vector2Int> returnTiles = new List<Vector2Int>();

            bool rightTraversable = this.IsTraversable(originIndexPosition + Vector2Int.right, new Vector2Int(1, rectDimensions.y), traversableTerrain, traversableResistance);
            bool upTraversable = this.IsTraversable(originIndexPosition + Vector2Int.down * rectDimensions.y, new Vector2Int(rectDimensions.x, 1), traversableTerrain, traversableResistance);
            bool leftTraversable = this.IsTraversable(originIndexPosition + Vector2Int.left * rectDimensions.x, new Vector2Int(1, rectDimensions.y), traversableTerrain, traversableResistance);
            bool downTraversable = this.IsTraversable(originIndexPosition + Vector2Int.up, new Vector2Int(rectDimensions.x, 1), traversableTerrain, traversableResistance);

            if (rightTraversable) 
                returnTiles.Add(originIndexPosition + Vector2Int.right);
            if (upTraversable) 
                returnTiles.Add(originIndexPosition + Vector2Int.down);
            if (leftTraversable) 
                returnTiles.Add(originIndexPosition + Vector2Int.left);
            if (downTraversable) 
                returnTiles.Add(originIndexPosition + Vector2Int.up);
            if (rightTraversable && upTraversable && this.IsTraversable(originIndexPosition + Vector2Int.right + Vector2Int.down * rectDimensions.y, Vector2Int.one, traversableTerrain, traversableResistance)) 
                returnTiles.Add(originIndexPosition + Vector2Int.right + Vector2Int.down);
            if (upTraversable && leftTraversable && this.IsTraversable(originIndexPosition + Vector2Int.down * rectDimensions.y + Vector2Int.left * rectDimensions.x, Vector2Int.one, traversableTerrain, traversableResistance)) 
                returnTiles.Add(originIndexPosition + Vector2Int.down + Vector2Int.left);
            if (leftTraversable && downTraversable && this.IsTraversable(originIndexPosition + Vector2Int.left * rectDimensions.x + Vector2Int.up, Vector2Int.one, traversableTerrain, traversableResistance))
                returnTiles.Add(originIndexPosition + Vector2Int.left + Vector2Int.up);
            if (downTraversable && rightTraversable && this.IsTraversable(originIndexPosition + Vector2Int.up + Vector2Int.right, Vector2Int.one, traversableTerrain, traversableResistance)) 
                returnTiles.Add(originIndexPosition + Vector2Int.up + Vector2Int.right);

            return returnTiles;
        }

        public List<Vector2Int> FindReachableIndexPositions(
            Vector2Int originIndexPosition,
            Vector2Int rectDimensions,
            TraversableTerrain traversableTerrain,
            Resistance traversableResistance,
            float minPathCost = 0,
            float maxPathCost = int.MaxValue
            )
        {
            List<Vector2Int> reachableTileIndexPositions = new List<Vector2Int>();

            if (!this.IsTraversable(originIndexPosition, rectDimensions, traversableTerrain, traversableResistance))
            {
                return reachableTileIndexPositions;
            }

            Dictionary<Vector2Int, float> pathCosts = new Dictionary<Vector2Int, float>();
            Queue<Vector2Int> nextTileIndexPositions = new Queue<Vector2Int>();

            nextTileIndexPositions.Enqueue(originIndexPosition);
            pathCosts.Add(originIndexPosition, 0);

            while (nextTileIndexPositions.Count != 0)
            {
                Vector2Int currentIndexPosition = nextTileIndexPositions.Dequeue();
                float currentPathCost = pathCosts[currentIndexPosition];
                if (currentPathCost < maxPathCost)
                {
                    foreach (Vector2Int adjacentIndexPosition in this.GetAdjacentTraversableIndexPositions(currentIndexPosition, rectDimensions, traversableTerrain, traversableResistance))
                    {
                        if (!pathCosts.ContainsKey(adjacentIndexPosition))
                        {
                            nextTileIndexPositions.Enqueue(adjacentIndexPosition);
                            pathCosts.Add(adjacentIndexPosition, currentPathCost + Vector2Int.Distance(currentIndexPosition, adjacentIndexPosition));
                        }
                    }
                }

                if (currentPathCost <= maxPathCost && currentPathCost >= minPathCost)
                {
                    reachableTileIndexPositions.Add(currentIndexPosition);
                }
            }

            return reachableTileIndexPositions;
        }

        public List<Vector2Int> GetClosestTraversableIndexPositions(Vector2Int originIndexPosition, Vector2Int rectDimensions, TraversableTerrain traversableTerrain, Resistance traversableResistance)
        {
            float lowestPathCost = float.MaxValue;
            List<Vector2Int> closestIndexPositions = new List<Vector2Int>();
            Queue<Vector2Int> nextIndexPositions = new Queue<Vector2Int>();
            Dictionary<Vector2Int, float> pathCosts = new Dictionary<Vector2Int, float>();

            nextIndexPositions.Enqueue(originIndexPosition);
            pathCosts.Add(originIndexPosition, 0);
            while (nextIndexPositions.Count > 0)
            {
                Vector2Int currIndexPosition = nextIndexPositions.Dequeue();
                float pathCost = pathCosts[currIndexPosition];

                if (pathCost <= lowestPathCost + Mathf.Epsilon)
                {
                    if (this.IsTraversable(currIndexPosition, rectDimensions, traversableTerrain, traversableResistance))
                    {
                        lowestPathCost = Mathf.Min(pathCost, lowestPathCost);
                        closestIndexPositions.Add(currIndexPosition);
                    }

                    foreach (Vector2Int adjacentIndexPosition in this.GetAdjacentTraversableIndexPositions(currIndexPosition, rectDimensions, TraversableTerrain.All, traversableResistance))
                    {
                        if (!pathCosts.ContainsKey(adjacentIndexPosition))
                        {
                            pathCosts.Add(adjacentIndexPosition, pathCost + Vector2Int.Distance(currIndexPosition, adjacentIndexPosition));
                            nextIndexPositions.Enqueue(adjacentIndexPosition);
                        }
                    }
                }
            }

            return closestIndexPositions;
        }

        public List<Vector2Int> FindPathIndexPositions(Vector2Int startIndexPosition, Vector2Int targetIndexPosition, Vector2Int rectDimensions, TraversableTerrain traversableTerrain, Resistance traversableResistance)
        {
            if (!this.IsTraversable(startIndexPosition, rectDimensions, traversableTerrain, traversableResistance))
            {
                return new List<Vector2Int>();
            }

            Dictionary<Vector2Int, (float gCost, float fCost, Vector2Int? parentIndexPosition)> nodes = new Dictionary<Vector2Int, (float gCost, float fCost, Vector2Int? parentIndexPosition)>();

            List<Vector2Int> ConstructPath(Vector2Int endIndexPosition)
            {
                List<Vector2Int> pathIndexPositions = new List<Vector2Int>();
                Vector2Int currIndexPosition = endIndexPosition;
                while (nodes[currIndexPosition].parentIndexPosition.HasValue)
                {
                    pathIndexPositions.Add(currIndexPosition);
                    currIndexPosition = nodes[currIndexPosition].parentIndexPosition.Value;
                }
                pathIndexPositions.Add(currIndexPosition);
                pathIndexPositions.Reverse();
                return pathIndexPositions;
            }

            HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            float startToTargetDistance = Vector2Int.Distance(startIndexPosition, targetIndexPosition);
            nodes.Add(startIndexPosition, (0, startToTargetDistance, null));

            float lowestDistanceToTarget = startToTargetDistance;
            Vector2Int lowestDistanceToTargetIndexPosition = startIndexPosition;

            openSet.Add(startIndexPosition);

            while (openSet.Count > 0)
            {
                float lowestFCost = float.MaxValue;
                Vector2Int currentIndexPosition = startIndexPosition;
                foreach (Vector2Int openIndexPosition in openSet)
                {
                    if (nodes[openIndexPosition].fCost < lowestFCost)
                    {
                        lowestFCost = nodes[openIndexPosition].fCost;
                        currentIndexPosition = openIndexPosition;
                    }
                }

                if (currentIndexPosition == targetIndexPosition)
                {
                    return ConstructPath(targetIndexPosition);
                }

                openSet.Remove(currentIndexPosition);
                closedSet.Add(currentIndexPosition);

                foreach (Vector2Int adjacentIndexPosition in this.GetAdjacentTraversableIndexPositions(currentIndexPosition, rectDimensions, traversableTerrain, traversableResistance))
                {
                    if (closedSet.Contains(adjacentIndexPosition)) continue;

                    float tentativeGCost = nodes[currentIndexPosition].gCost + Vector2Int.Distance(currentIndexPosition, adjacentIndexPosition);

                    if (!nodes.ContainsKey(adjacentIndexPosition) || tentativeGCost < nodes[adjacentIndexPosition].gCost)
                    {
                        float distanceToTarget = Vector2Int.Distance(adjacentIndexPosition, targetIndexPosition);
                        nodes[adjacentIndexPosition] = (tentativeGCost, tentativeGCost + distanceToTarget, currentIndexPosition);

                        if (distanceToTarget < lowestDistanceToTarget)
                        {
                            lowestDistanceToTarget = distanceToTarget;
                            lowestDistanceToTargetIndexPosition = adjacentIndexPosition;
                        }

                        if (!openSet.Contains(adjacentIndexPosition)) openSet.Add(adjacentIndexPosition);
                    }
                }
            }

            return ConstructPath(lowestDistanceToTargetIndexPosition);
        }
    }
}
