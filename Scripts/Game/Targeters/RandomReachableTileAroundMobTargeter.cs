using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RandomReachableTileAroundMobTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private FloatSerializedReference minPathCost;
        [SerializeField]
        private FloatSerializedReference maxPathCost;
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

            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                List<Vector2Int> reachableIndexPositions = mob.TiledArea.NavigationGrid.FindReachableIndexPositions(mob.IndexPosition, mob.TileSize, mob.TraversableTerrain, Resistance.None, this.minPathCost.MutableValue, this.maxPathCost.MutableValue);
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
                    positions[i] = AreaTiling.RectPositionFromIndexPosition(
                        reachableIndexPositions[Random.Range(0, reachableIndexPositions.Count)],
                        mob.TiledArea.CenterPosition,
                        mob.TiledArea.MainAreaDimensions,
                        mob.TileSize
                        );
                }
                else
                {
                    positions[i] = mob.Position;
                }
            }
            return positions;
        }
    }
}
