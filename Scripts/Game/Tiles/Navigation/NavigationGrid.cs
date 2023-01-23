using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class NavigationGrid
    {
        private NavigationTile[][] navigationTileGrid;
        private Vector2Int gridDimensions;

        public NavigationGrid(TiledAreaBlueprint tiledAreaBlueprint)
        {
            this.navigationTileGrid = new NavigationTile[tiledAreaBlueprint.MainAreaDimensions.x][];
            this.gridDimensions = tiledAreaBlueprint.MainAreaDimensions;
            for (int x = 0; x < tiledAreaBlueprint.MainAreaDimensions.x; x++)
            {
                this.navigationTileGrid[x] = new NavigationTile[tiledAreaBlueprint.MainAreaDimensions.y];
                for (int y = 0; y < tiledAreaBlueprint.MainAreaDimensions.y; y++)
                {
                    this.navigationTileGrid[x][y] = new NavigationTile(tiledAreaBlueprint.GetTerrainAtTile(new Vector2Int(x, y)));
                }
            }
        }

        public TileTerrain TerrainAtTile(Vector2Int tileIndices)
        {
            return this.navigationTileGrid[tileIndices.x][tileIndices.y].Terrain;
        }

        public bool UnobstructedAtTile(Vector2Int tileIndices)
        {
            return this.navigationTileGrid[tileIndices.x][tileIndices.y].Unobstructed;
        }

        public void AddObstruction(Vector2Int tileIndices, Resistance resistance)
        {
            this.navigationTileGrid[tileIndices.x][tileIndices.y].AddObstruction(resistance);
        }

        public void RemoveObstruction(Vector2Int tileIndices, Resistance resistance)
        {
            this.navigationTileGrid[tileIndices.x][tileIndices.y].RemoveObstruction(resistance);
        }

        public bool IsOnTerrain(Vector2Int indices, Vector2Int rectDimensions, TraversableTerrain traversableTerrain)
        {
            if (!TilePositioning.RectIndicesWithinBounds(indices, this.gridDimensions, rectDimensions)) return false;
            bool onTerrain = true;
            TilePositioning.VisitTileIndicesInRect(
                indices,
                rectDimensions,
                this.gridDimensions,
                (Vector2Int indices) =>
                {
                    NavigationTile navigationTile = this.navigationTileGrid[indices.x][indices.y];
                    onTerrain &= traversableTerrain.Includes(navigationTile.Terrain);
                }
                );
            return onTerrain;
        }

        public bool IsTraversable(Vector2Int indices, Vector2Int rectDimensions, TraversableTerrain traversableTerrain, Resistance breakingResistance = Resistance.None)
        {
            if (!TilePositioning.RectIndicesWithinBounds(indices, this.gridDimensions, rectDimensions)) return false;
            bool isTraversable = true;
            TilePositioning.VisitTileIndicesInRect(
                indices,
                rectDimensions, 
                this.gridDimensions,
                (Vector2Int indices) =>
                {
                    NavigationTile navigationTile = this.navigationTileGrid[indices.x][indices.y];
                    BreakInfo resistInfo = new BreakInfo(breakingResistance, navigationTile.HighestObstructiveResistance);
                    isTraversable &= (navigationTile.Unobstructed || resistInfo.Broken) && traversableTerrain.Includes(navigationTile.Terrain);
                }
                );
            return isTraversable;
        }

        public List<Vector2Int> GetAdjacentTraversableIndices(Vector2Int originIndices, Vector2Int rectDimensions, TraversableTerrain traversableTerrain)
        {
            List<Vector2Int> returnTiles = new List<Vector2Int>();

            bool rightTraversable = IsTraversable(originIndices + Vector2Int.right, new Vector2Int(1, rectDimensions.y), traversableTerrain);
            bool upTraversable = IsTraversable(originIndices + Vector2Int.down * rectDimensions.y, new Vector2Int(rectDimensions.x, 1), traversableTerrain);
            bool leftTraversable = IsTraversable(originIndices + Vector2Int.left * rectDimensions.x, new Vector2Int(1, rectDimensions.y), traversableTerrain);
            bool downTraversable = IsTraversable(originIndices + Vector2Int.up, new Vector2Int(rectDimensions.x, 1), traversableTerrain);

            if (rightTraversable) 
                returnTiles.Add(originIndices + Vector2Int.right);
            if (upTraversable) 
                returnTiles.Add(originIndices + Vector2Int.down);
            if (leftTraversable) 
                returnTiles.Add(originIndices + Vector2Int.left);
            if (downTraversable) 
                returnTiles.Add(originIndices + Vector2Int.up);
            if (rightTraversable && upTraversable && IsTraversable(originIndices + Vector2Int.right + Vector2Int.down * rectDimensions.y, Vector2Int.one, traversableTerrain)) 
                returnTiles.Add(originIndices + Vector2Int.right + Vector2Int.down);
            if (upTraversable && leftTraversable && IsTraversable(originIndices + Vector2Int.down * rectDimensions.y + Vector2Int.left * rectDimensions.x, Vector2Int.one, traversableTerrain)) 
                returnTiles.Add(originIndices + Vector2Int.down + Vector2Int.left);
            if (leftTraversable && downTraversable && IsTraversable(originIndices + Vector2Int.left * rectDimensions.x + Vector2Int.up, Vector2Int.one, traversableTerrain))
                returnTiles.Add(originIndices + Vector2Int.left + Vector2Int.up);
            if (downTraversable && rightTraversable && IsTraversable(originIndices + Vector2Int.up + Vector2Int.right, Vector2Int.one, traversableTerrain)) 
                returnTiles.Add(originIndices + Vector2Int.up + Vector2Int.right);

            return returnTiles;
        }

        public List<Vector2Int> FindReachableIndices(
            Vector2Int originIndices,
            Vector2Int rectDimensions,
            TraversableTerrain traversableTerrain,
            float minPathCost = 0,
            float maxPathCost = int.MaxValue
            )
        {
            List<Vector2Int> reachableTiles = new List<Vector2Int>();

            if (IsTraversable(originIndices, rectDimensions, traversableTerrain))
            {
                return reachableTiles;
            }

            Dictionary<Vector2Int, float> pathCosts = new Dictionary<Vector2Int, float>();
            Queue<Vector2Int> tileQueue = new Queue<Vector2Int>();

            tileQueue.Enqueue(originIndices);
            pathCosts.Add(originIndices, 0);

            while (tileQueue.Count != 0)
            {
                Vector2Int currentIndices = tileQueue.Dequeue();
                float currentPathCost = pathCosts[currentIndices];
                if (currentPathCost < maxPathCost)
                {
                    foreach (Vector2Int adjacentIndices in GetAdjacentTraversableIndices(currentIndices, rectDimensions, traversableTerrain))
                    {
                        if (!pathCosts.ContainsKey(adjacentIndices))
                        {
                            tileQueue.Enqueue(adjacentIndices);
                            pathCosts.Add(adjacentIndices, currentPathCost + Vector2Int.Distance(currentIndices, adjacentIndices));
                        }
                    }
                }

                if (currentPathCost <= maxPathCost && currentPathCost >= minPathCost)
                {
                    reachableTiles.Add(currentIndices);
                }
            }

            return reachableTiles;
        }

        public List<Vector2Int> GetClosestTraversableIndices(Vector2Int originIndices, Vector2Int rectDimensions, TraversableTerrain traversableTerrain)
        {
            float lowestPathCost = float.MaxValue;
            List<Vector2Int> closestIndices = new List<Vector2Int>();
            Queue<Vector2Int> nextIndices = new Queue<Vector2Int>();
            Dictionary<Vector2Int, float> pathCosts = new Dictionary<Vector2Int, float>();

            nextIndices.Enqueue(originIndices);
            pathCosts.Add(originIndices, 0);
            while (nextIndices.Count > 0)
            {
                Vector2Int currIndices = nextIndices.Dequeue();
                float pathCost = pathCosts[currIndices];

                if (pathCost <= lowestPathCost + Mathf.Epsilon)
                {
                    if (IsTraversable(currIndices, rectDimensions, traversableTerrain))
                    {
                        lowestPathCost = Mathf.Min(pathCost, lowestPathCost);
                        closestIndices.Add(currIndices);
                    }

                    foreach (Vector2Int adjacentIndices in GetAdjacentTraversableIndices(currIndices, rectDimensions, TraversableTerrain.All))
                    {
                        if (!pathCosts.ContainsKey(adjacentIndices))
                        {
                            pathCosts.Add(adjacentIndices, pathCost + Vector2Int.Distance(currIndices, adjacentIndices));
                            nextIndices.Enqueue(adjacentIndices);
                        }
                    }
                }
            }

            return closestIndices;
        }

        public List<Vector2Int> FindPathIndices(Vector2Int startIndices, Vector2Int targetIndices, Vector2Int rectDimensions, TraversableTerrain traversableTerrain)
        {
            if (!IsTraversable(startIndices, rectDimensions, traversableTerrain) || !IsTraversable(targetIndices, rectDimensions, traversableTerrain))
            {
                return new List<Vector2Int>();
            }

            Dictionary<Vector2Int, (float gCost, float fCost, Vector2Int? parentIndices)> nodes = new Dictionary<Vector2Int, (float gCost, float fCost, Vector2Int? parentIndices)>();

            List<Vector2Int> ConstructPath(Vector2Int endIndices)
            {
                List<Vector2Int> pathIndices = new List<Vector2Int>();
                Vector2Int currIndices = endIndices;
                while (nodes[currIndices].parentIndices.HasValue)
                {
                    pathIndices.Add(currIndices);
                    currIndices = nodes[currIndices].parentIndices.Value;
                }
                pathIndices.Add(currIndices);
                pathIndices.Reverse();
                return pathIndices;
            }

            HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            float startToTargetDistance = Vector2Int.Distance(startIndices, targetIndices);
            nodes.Add(startIndices, (0, startToTargetDistance, null));

            float lowestDistanceToTarget = startToTargetDistance;
            Vector2Int lowestDistanceToTargetIndices = startIndices;

            openSet.Add(startIndices);

            while (openSet.Count > 0)
            {
                float lowestFCost = float.MaxValue;
                Vector2Int currentIndices = startIndices;
                foreach (Vector2Int openIndices in openSet)
                {
                    if (nodes[openIndices].fCost < lowestFCost)
                    {
                        lowestFCost = nodes[openIndices].fCost;
                        currentIndices = openIndices;
                    }
                }

                if (currentIndices == targetIndices)
                {
                    return ConstructPath(targetIndices);
                }

                openSet.Remove(currentIndices);
                closedSet.Add(currentIndices);

                foreach (Vector2Int adjacentIndices in GetAdjacentTraversableIndices(currentIndices, rectDimensions, traversableTerrain))
                {
                    if (closedSet.Contains(adjacentIndices)) continue;

                    float tentativeGCost = nodes[currentIndices].gCost + Vector2Int.Distance(currentIndices, adjacentIndices);

                    if (!nodes.ContainsKey(adjacentIndices) || tentativeGCost < nodes[adjacentIndices].gCost)
                    {
                        float distanceToTarget = Vector2Int.Distance(adjacentIndices, targetIndices);
                        nodes[adjacentIndices] = (tentativeGCost, tentativeGCost + distanceToTarget, currentIndices);

                        if (distanceToTarget < lowestDistanceToTarget)
                        {
                            lowestDistanceToTarget = distanceToTarget;
                            lowestDistanceToTargetIndices = adjacentIndices;
                        }

                        if (!openSet.Contains(adjacentIndices)) openSet.Add(adjacentIndices);
                    }
                }
            }

            return ConstructPath(lowestDistanceToTargetIndices);
        }
    }
}
