using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobHasLineOfSightPositionTargeter : Targeter
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private Targeter sightTargeter;
        [SerializeField]
        private float blockingRadius;
        [SerializeField]
        private FloatSerializedReference minDistanceFromSightPosition;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] sightPositions = this.sightTargeter.Calculate(currentPositions, elapsedDuration, elapsedDurationDelta);
            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                if (!this.mob.Physicality.IsInSightFrom(this.mob.AbsolutePosition, sightPositions[i], this.blockingRadius)) 
                {
                    List<Vector2Int> reachableTileIndices = this.mob.TiledArea.NavigationGrid.FindReachableIndices(this.mob.PositionIndices, this.mob.TileSize, this.mob.TraversableTerrain);
                    reachableTileIndices.Sort(
                        (Vector2Int firstTileIndices, Vector2Int secondtileIndices) =>
                        {
                            float firstDist = Vector2.Distance(this.mob.AbsolutePosition, TilePositioning.RectAbsolutePositionFromIndices(firstTileIndices, this.mob.TiledArea.AbsoluteCenterPosition, this.mob.TiledArea.MainAreaDimensions, this.mob.TileSize));
                            float secondDist = Vector2.Distance(this.mob.AbsolutePosition, TilePositioning.RectAbsolutePositionFromIndices(secondtileIndices, this.mob.TiledArea.AbsoluteCenterPosition, this.mob.TiledArea.MainAreaDimensions, this.mob.TileSize));
                            if (firstDist > secondDist) return 1;
                            return -1;
                        }
                        );
                    foreach (Vector2Int nearbyTileIndices in reachableTileIndices)
                    {
                        Vector2 nearbyTileAbsolutePosition = TilePositioning.RectAbsolutePositionFromIndices(nearbyTileIndices, this.mob.TiledArea.AbsoluteCenterPosition, this.mob.TiledArea.MainAreaDimensions, this.mob.TileSize);
                        if (this.mob.Physicality.IsInSightFrom(nearbyTileAbsolutePosition, sightPositions[i], this.blockingRadius) && Vector2.Distance(nearbyTileAbsolutePosition, sightPositions[i]) >= this.minDistanceFromSightPosition.ImmutableValue)
                        {
                            positions[i] = nearbyTileAbsolutePosition;
                            break;
                        }
                    }
                }
            }
            return positions;
        }
    }
}
