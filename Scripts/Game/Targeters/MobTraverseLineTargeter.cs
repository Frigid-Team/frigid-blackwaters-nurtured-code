using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobTraverseLineTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private FloatSerializedReference minimumDistanceAway;
        [SerializeField]
        private Direction direction;

        protected override Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currentPositions;
            }

            Vector2[] positions = new Vector2[currentPositions.Length];
            currentPositions.CopyTo(positions, 0);

            Vector2[] directions = this.direction.Retrieve(new Vector2[currentPositions.Length], elapsedDuration, elapsedDurationDelta);
            for (int i = 0; i < positions.Length; i++)
            {
                if (directions[i].magnitude == 0) continue;

                Vector2 currentTestPosition = mob.Position + directions[i] * this.minimumDistanceAway.ImmutableValue;
                while (AreaTiling.RectPositionWithinBounds(currentTestPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize) && 
                       !Mob.CanTraverseAt(mob.TiledArea, currentTestPosition, mob.Size, mob.TraversableTerrain))
                {
                    currentTestPosition += directions[i] * Mathf.Min(mob.Size.x, mob.Size.y) / 2;
                }
                positions[i] = currentTestPosition;
            }

            return positions;
        }
    }
}
