using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobSightLineTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private Targeter sightTargeter;
        [SerializeField]
        private FloatSerializedReference blockingRadius;
        [SerializeField]
        private FloatSerializedReference sweetSpotMin;
        [SerializeField]
        private FloatSerializedReference sweetSpotMax;
        [SerializeField]
        private List<Targeter> avoidanceTargeters;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Mob mob = this.mob.ImmutableValue;

            Vector2[] sightPositions = this.sightTargeter.Calculate(currentPositions, elapsedDuration, elapsedDurationDelta);
            List<Vector2[]> avoidancePositionsPerTargeter = new List<Vector2[]>();
            foreach (Targeter avoidanceTargeter in this.avoidanceTargeters) avoidancePositionsPerTargeter.Add(avoidanceTargeter.Calculate(currentPositions, elapsedDuration, elapsedDurationDelta));
            Vector2[] positions = new Vector2[currentPositions.Length];
            currentPositions.CopyTo(positions, 0);

            Vector2 centerPosition = mob.TiledArea.CenterPosition;
            Vector2Int areaDimensions = mob.TiledArea.MainAreaDimensions;

            for (int i = 0; i < positions.Length; i++)
            {
                List<Vector2Int> reachableTileIndices = mob.TiledArea.NavigationGrid.FindReachableIndices(mob.PositionIndices, mob.TileSize, mob.TraversableTerrain, 0, this.sweetSpotMax.ImmutableValue);
                reachableTileIndices.Sort(
                    (Vector2Int firstTileIndices, Vector2Int secondTileIndices) => 
                    {
                        Vector2 firstPosition = TilePositioning.RectPositionFromIndices(firstTileIndices, centerPosition, areaDimensions, mob.TileSize);
                        Vector2 secondPosition = TilePositioning.RectPositionFromIndices(secondTileIndices, centerPosition, areaDimensions, mob.TileSize);
                        float firstDistance = Vector2.Distance(mob.Position, firstPosition);
                        float secondDistance = Vector2.Distance(mob.Position, secondPosition);
                        bool firstInSweetSpot = firstDistance >= this.sweetSpotMin.ImmutableValue && firstDistance <= this.sweetSpotMax.ImmutableValue;
                        bool secondInSweetSpot = secondDistance >= this.sweetSpotMin.ImmutableValue && secondDistance <= this.sweetSpotMax.ImmutableValue;
                        float firstAvoidanceDistScore = 0;
                        float secondAvoidanceDistScore = 0;
                        foreach (Vector2[] avoidancePositions in avoidancePositionsPerTargeter)
                        {
                            firstAvoidanceDistScore += Vector2.Distance(avoidancePositions[i], firstPosition);
                            secondAvoidanceDistScore += Vector2.Distance(avoidancePositions[i], secondPosition);
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

                foreach (Vector2Int nearbyTileIndices in reachableTileIndices)
                {
                    Vector2 nearbyTilePosition = TilePositioning.RectPositionFromIndices(nearbyTileIndices, centerPosition, areaDimensions, mob.TileSize);
                    if (mob.CanSeeThrough(nearbyTilePosition, sightPositions[i], this.blockingRadius.ImmutableValue))
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
