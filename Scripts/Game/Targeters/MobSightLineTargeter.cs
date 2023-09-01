using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobSightLineTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private Targeter sightTargeter;
        [SerializeField]
        private FloatSerializedReference blockingRadius;
        [SerializeField]
        private FloatSerializedReference sweetSpotMin;
        [SerializeField]
        private FloatSerializedReference sweetSpotMax;
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

            Vector2[] sightPositions = this.sightTargeter.Retrieve(currentPositions, elapsedDuration, elapsedDurationDelta);

            Vector2[] positions = new Vector2[currentPositions.Length];
            currentPositions.CopyTo(positions, 0);

            Vector2 centerPosition = mob.TiledArea.CenterPosition;
            Vector2Int areaDimensions = mob.TiledArea.MainAreaDimensions;

            for (int i = 0; i < positions.Length; i++)
            {
                List<Vector2Int> reachableIndexPositions = mob.TiledArea.NavigationGrid.FindReachableIndexPositions(mob.IndexPosition, mob.TileSize, mob.TraversableTerrain, 0, this.sweetSpotMax.ImmutableValue);
                reachableIndexPositions.Sort(
                    (Vector2Int firstIndexPosition, Vector2Int secondIndexPosition) => 
                    {
                        Vector2 firstPosition = AreaTiling.RectPositionFromIndexPosition(firstIndexPosition, centerPosition, areaDimensions, mob.TileSize);
                        Vector2 secondPosition = AreaTiling.RectPositionFromIndexPosition(secondIndexPosition, centerPosition, areaDimensions, mob.TileSize);
                        float firstDistance = Vector2.Distance(mob.Position, firstPosition);
                        float secondDistance = Vector2.Distance(mob.Position, secondPosition);
                        bool firstInSweetSpot = firstDistance >= this.sweetSpotMin.ImmutableValue && firstDistance <= this.sweetSpotMax.ImmutableValue;
                        bool secondInSweetSpot = secondDistance >= this.sweetSpotMin.ImmutableValue && secondDistance <= this.sweetSpotMax.ImmutableValue;
                        float firstAvoidanceDistScore = 0;
                        float secondAvoidanceDistScore = 0;
                        foreach (Vector2 positionToAvoid in positionsToAvoid)
                        {
                            firstAvoidanceDistScore += Vector2.Distance(positionToAvoid, firstPosition);
                            secondAvoidanceDistScore += Vector2.Distance(positionToAvoid, secondPosition);
                        }

                        if (firstInSweetSpot && secondInSweetSpot || !firstInSweetSpot && !secondInSweetSpot)
                        {
                            if (firstDistance + firstAvoidanceDistScore > secondDistance + secondAvoidanceDistScore) return -1;
                            return 1;
                        }
                        else if (firstInSweetSpot)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    );

                foreach (Vector2Int reachableIndexPosition in reachableIndexPositions)
                {
                    Vector2 nearbyTilePosition = AreaTiling.RectPositionFromIndexPosition(reachableIndexPosition, centerPosition, areaDimensions, mob.TileSize);
                    if (mob.CanSeeUnobstructed(nearbyTilePosition, sightPositions[i], this.blockingRadius.ImmutableValue))
                    {
                        positions[i] = nearbyTilePosition;
                        break;
                    }
                }
            }
            return positions;
        }
    }
}
