using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RandomTraversableIslandAroundMobTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private FloatSerializedReference avoidanceDistance;
        [SerializeField]
        private List<Targeter> targetersToAvoid;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Mob mob = this.mob.ImmutableValue;

            HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
            List<List<Vector2Int>> islands = new List<List<Vector2Int>>();

            List<Vector2> avoidAbsolutePositions = new List<Vector2>();
            foreach (Targeter targeterToAvoid in this.targetersToAvoid)
            {
                avoidAbsolutePositions.Add(targeterToAvoid.Calculate(mob.Position, elapsedDuration, elapsedDurationDelta));
            }

            for (int x = 0; x < mob.TiledArea.MainAreaDimensions.x; x++)
            {
                for (int y = 0; y < mob.TiledArea.MainAreaDimensions.y; y++)
                {
                    Vector2Int tileIndices = new Vector2Int(x, y);

                    if (!visitedTiles.Contains(tileIndices))
                    {
                        List<Vector2Int> reachableTileIndices = mob.TiledArea.NavigationGrid.FindReachableIndices(tileIndices, mob.TileSize, mob.TraversableTerrain);
                        foreach (Vector2Int visitedTileIndices in reachableTileIndices)
                        {
                            visitedTiles.Add(visitedTileIndices);
                        }
                        reachableTileIndices.RemoveAll(
                            (Vector2Int reachableTileIndices) =>
                            {
                                Vector2 absoluteReachableTilePosition = TilePositioning.RectPositionFromIndices(reachableTileIndices, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize);
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

            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                if (islands.Count > 0)
                {
                    List<Vector2Int> reachableTileIndices = islands[Random.Range(0, islands.Count)];
                    positions[i] = TilePositioning.RectPositionFromIndices(reachableTileIndices[Random.Range(0, reachableTileIndices.Count)], mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize);
                }
            }
            return positions;
        }
    }
}
