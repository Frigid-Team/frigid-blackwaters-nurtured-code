using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobLineFitTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private FloatSerializedReference minimumDistanceAway;
        [SerializeField]
        private Direction direction;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = new Vector2[currentPositions.Length];
            Vector2[] directions = this.direction.Calculate(new Vector2[currentPositions.Length], elapsedDuration, elapsedDurationDelta);
            Mob mob = this.mob.ImmutableValue;
            TiledArea tiledArea = mob.TiledArea;
            for (int i = 0; i < positions.Length; i++)
            {
                Vector2 currentTestPosition = mob.Position + directions[i] * this.minimumDistanceAway.ImmutableValue;
                while (TilePositioning.RectPositionWithinBounds(currentTestPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions, mob.TileSize) && 
                       !Mob.CanFitAt(tiledArea, currentTestPosition, mob.Size, mob.TraversableTerrain))
                {
                    currentTestPosition += directions[i] * Mathf.Min(mob.Size.x, mob.Size.y) / 2;
                }
                positions[i] = currentTestPosition;
            }
            return positions;
        }
    }
}
