using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDodgeAwayDirection : Direction
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private List<Targeter> avoidanceTargeters;
        [SerializeField]
        private FloatSerializedReference dodgeDistance;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Mob mob = this.mob.ImmutableValue;
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = Vector2.zero;

            List<Vector2[]> avoidancePositionsPerTargeter = new List<Vector2[]>();
            foreach (Targeter avoidanceTargeter in this.avoidanceTargeters)
            {
                avoidancePositionsPerTargeter.Add(avoidanceTargeter.Calculate(new Vector2[currDirections.Length], elapsedDuration, elapsedDurationDelta));
            }

            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int mobTileIndices = TilePositioning.TileIndicesFromPosition(mob.Position, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions);
                Vector2 dodgeDirectionSum = Vector2.zero;
                foreach (Vector2Int tileIndices in Geometry.GetAllSquaresOverlappingCircle(mobTileIndices, this.dodgeDistance.ImmutableValue))
                {
                    if (TilePositioning.TileIndicesWithinBounds(tileIndices, mob.TiledArea.MainAreaDimensions))
                    {
                        Vector2 tilePosition = TilePositioning.TilePositionFromIndices(tileIndices, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions);
                        Vector2 tileDisplacement = tilePosition - mob.Position;
                        Vector2 dodgeDisplacement = (mob.CanPassThrough(mob.Position, tilePosition) ? 1 : -1) * tileDisplacement;
                        float dotScore = 0;
                        foreach (Vector2[] avoidancePositions in avoidancePositionsPerTargeter)
                        {
                            dotScore += 1 - Vector2.Dot(dodgeDisplacement, avoidancePositions[i] - mob.Position);
                        }
                        dodgeDirectionSum += dodgeDisplacement * dotScore;
                    }
                }
                directions[i] = dodgeDirectionSum.normalized;
            }

            return directions;
        }
    }
}
