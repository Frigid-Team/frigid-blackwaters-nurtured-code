using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RandomTraversableIslandAroundMobTargeter : Targeter
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private FloatSerializedReference avoidanceDistance;
        [SerializeField]
        private List<Targeter> targetersToAvoid;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            return new Vector2[currentPositions.Length];
        }

            // MOBS_V2_TODO: Fix later
            /*
            protected override Vector2 GetTarget(float elapsedDuration, float elapsedDurationDelta)
            {
            HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
            List<List<Vector2Int>> islands = new List<List<Vector2Int>>();

            List<Vector2> avoidAbsolutePositions = new List<Vector2>();
            foreach (Targeter targeterToAvoid in this.targetersToAvoid)
            {
                avoidAbsolutePositions.Add(targeterToAvoid.CalculateTarget(elapsedDuration));
            }

            if (this.mob.TiledAreaOccupier.TryGetCurrentTiledArea(out TiledArea tiledArea))
            {
                for (int x = 0; x < tiledArea.MainAreaDimensions.x; x++)
                {
                    for (int y = 0; y < tiledArea.MainAreaDimensions.y; y++)
                    {
                        Vector2Int tileIndices = new Vector2Int(x, y);

                        if (!visitedTiles.Contains(tileIndices))
                        {
                            List<Vector2Int> reachableTileIndices = tiledArea.NavigationGrid.FindReachableTileIndices(this.mob.TraversableTerrain, absoluteTilePosition);
                            foreach (Vector2Int visitedTileIndices in reachableTileIndices)
                            {
                                visitedTiles.Add(visitedTileIndices);
                            }
                            reachableTileIndices.RemoveAll(
                                (Vector2Int reachableTileIndices) => 
                                {
                                    Vector2 absoluteReachableTilePosition = TilePositioning.AbsolutePositionFromIndices(reachableTileIndices, tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions);
                                    foreach (Vector2 avoidAbsolutePosition in avoidAbsolutePositions)
                                    {
                                        if (Vector2.Distance(avoidAbsolutePosition, absoluteReachableTilePosition) < this.avoidanceDistance.ImmutableValue) return true;
                                    }
                                    return false;
                                }
                                );
                            if (reachableTileIndices.Count > 0)
                            {
                                islands.Add(reachableTileIndices);
                            }
                        }
                        visitedTiles.Add(tileIndices);
                    }
                }

                if (islands.Count > 0)
                {
                    List<Vector2Int> reachableTileIndices = islands[Random.Range(0, islands.Count)];
                    return TilePositioning.AbsolutePositionFromIndices(reachableTileIndices[Random.Range(0, reachableTileIndices.Count)], tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions);
                }
            }
            return this.mob.transform.position;
            }
            */
    }
}
