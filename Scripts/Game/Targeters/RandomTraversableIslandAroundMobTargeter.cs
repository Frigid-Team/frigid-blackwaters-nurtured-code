using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RandomTraversableIslandAroundMobTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private FloatSerializedReference avoidanceDistance;
        [SerializeField]
        private List<Targeter> targetersToAvoid;
        [SerializeField]
        private List<MobQuery> mobsToAvoidQueries;

        protected override Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currentPositions;
            }

            HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
            List<List<Vector2Int>> islands = new List<List<Vector2Int>>();

            List<Vector2> positionsToAvoid = new List<Vector2>();
            foreach (Targeter targeterToAvoid in this.targetersToAvoid)
            {
                positionsToAvoid.Add(targeterToAvoid.Retrieve(mob.Position, elapsedDuration, elapsedDurationDelta));
            }
            foreach (MobQuery mobsToAvoidQuery in this.mobsToAvoidQueries)
            {
                foreach (Mob mobToAvoid in mobsToAvoidQuery.Execute())
                {
                    positionsToAvoid.Add(mobToAvoid.Position);
                }
            }

            for (int x = 0; x < mob.TiledArea.MainAreaDimensions.x; x++)
            {
                for (int y = 0; y < mob.TiledArea.MainAreaDimensions.y; y++)
                {
                    Vector2Int tileIndexPosition = new Vector2Int(x, y);

                    if (!visitedTiles.Contains(tileIndexPosition))
                    {
                        List<Vector2Int> reachableIndexPositions = mob.TiledArea.NavigationGrid.FindReachableIndexPositions(tileIndexPosition, mob.TileSize, mob.TraversableTerrain, Resistance.None);
                        foreach (Vector2Int reachableIndexPosition in reachableIndexPositions)
                        {
                            visitedTiles.Add(reachableIndexPosition);
                        }
                        reachableIndexPositions.RemoveAll(
                            (Vector2Int reachableIndexPosition) =>
                            {
                                Vector2 reachableTilePosition = AreaTiling.RectPositionFromIndexPosition(reachableIndexPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize);
                                foreach (Vector2 positionToAvoid in positionsToAvoid)
                                {
                                    if (Vector2.Distance(positionToAvoid, reachableTilePosition) < this.avoidanceDistance.ImmutableValue) return true;
                                }
                                return false;
                            }
                            );
                        if (reachableIndexPositions.Count > 0)
                        {
                            islands.Add(reachableIndexPositions);
                        }
                    }
                    visitedTiles.Add(tileIndexPosition);
                }
            }

            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                if (islands.Count > 0)
                {
                    List<Vector2Int> reachableIndexPositions = islands[Random.Range(0, islands.Count)];
                    positions[i] = AreaTiling.RectPositionFromIndexPosition(reachableIndexPositions[Random.Range(0, reachableIndexPositions.Count)], mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize);
                }
            }
            return positions;
        }
    }
}
