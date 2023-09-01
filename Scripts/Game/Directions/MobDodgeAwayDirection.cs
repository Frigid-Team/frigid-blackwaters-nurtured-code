using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDodgeAwayDirection : Direction
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private List<Targeter> targetersToAvoid;
        [SerializeField]
        private List<MobQuery> mobsToAvoidQueries;
        [SerializeField]
        private FloatSerializedReference dodgeDistance;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currDirections;
            }

            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = Vector2.zero;

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

            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int mobTileIndexPosition = AreaTiling.TileIndexPositionFromPosition(mob.Position, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions);
                Vector2 dodgeDirectionSum = Vector2.zero;
                foreach (Vector2Int tileIndexPosition in Geometry.GetAllSquaresOverlappingCircle(mobTileIndexPosition, this.dodgeDistance.ImmutableValue))
                {
                    if (AreaTiling.TileIndexPositionWithinBounds(tileIndexPosition, mob.TiledArea.MainAreaDimensions))
                    {
                        Vector2 tilePosition = AreaTiling.TilePositionFromIndexPosition(tileIndexPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions);
                        Vector2 tileDisplacement = tilePosition - mob.Position;
                        Vector2 dodgeDisplacement = (mob.CanPassThrough(mob.Position, tilePosition) ? 1 : -1) * tileDisplacement;
                        float dotScore = 0;
                        foreach (Vector2 positionToAvoid in positionsToAvoid)
                        {
                            dotScore += 1 - Vector2.Dot(dodgeDisplacement, positionToAvoid - mob.Position);
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
