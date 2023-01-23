using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DodgeAwayFromWarningDirection : Direction
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private FloatSerializedReference dodgeDistance;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = Vector2.zero;

            if (this.mob.DamageReceiver.ThreatsReceived.Count > 0)
            {
                Vector2 dodgeDirectionSum = Vector2.zero;
                Vector2Int mobTileIndices = TilePositioning.TileIndicesFromAbsolutePosition(this.mob.AbsolutePosition, this.mob.TiledArea.AbsoluteCenterPosition, this.mob.TiledArea.MainAreaDimensions);
                float minDistanceFactor = Mathf.Min(this.mob.Size.x, this.mob.Size.y);
                int count = 0;
                foreach (Vector2Int tileIndices in Geometry.GetAllSquaresOverlappingCircle(mobTileIndices, this.dodgeDistance.ImmutableValue))
                {
                    if (TilePositioning.TileIndicesWithinBounds(tileIndices, this.mob.TiledArea.MainAreaDimensions))
                    {
                        Vector2 absoluteTilePosition = TilePositioning.TileAbsolutePositionFromIndices(tileIndices, this.mob.TiledArea.AbsoluteCenterPosition, this.mob.TiledArea.MainAreaDimensions);
                        Vector2 tileDisplacement = absoluteTilePosition - (Vector2)this.mob.AbsolutePosition;
                        dodgeDirectionSum +=
                            (this.mob.TiledArea.NavigationGrid.UnobstructedAtTile(tileIndices) ? 1 : -1) *
                            tileDisplacement.normalized *
                            (this.dodgeDistance.ImmutableValue / Mathf.Max(minDistanceFactor, tileDisplacement.magnitude));
                        count++;
                    }
                }
                Vector2 warnDisplacement = this.mob.DamageReceiver.ThreatsReceived.First.Value.ThreatPosition - (Vector2)this.mob.AbsolutePosition;
                dodgeDirectionSum += warnDisplacement.normalized * (this.dodgeDistance.ImmutableValue / Mathf.Max(minDistanceFactor, warnDisplacement.magnitude)) * count;
                Vector2 warnDirection = -this.mob.DamageReceiver.ThreatsReceived.First.Value.ThreatDirection;
                dodgeDirectionSum += warnDirection.normalized * (this.dodgeDistance.ImmutableValue / Mathf.Max(minDistanceFactor, warnDirection.magnitude)) * count;
                for (int i = 0; i < directions.Length; i++) directions[i] = dodgeDirectionSum;
            }

            return directions;
        }
    }
}
