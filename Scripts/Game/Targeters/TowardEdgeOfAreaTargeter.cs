using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TowardEdgeOfAreaTargeter : Targeter
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private Direction direction;

        protected override Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = this.originTargeter.Retrieve(currentPositions, elapsedDuration, elapsedDurationDelta);
            Vector2[] directions = this.direction.Retrieve(new Vector2[positions.Length], elapsedDuration, elapsedDurationDelta);
            for (int i = 0; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 direction = directions[i];
                if (TiledArea.TryGetAreaAtPosition(position, out TiledArea tiledArea))
                {
                    positions[i] = AreaTiling.EdgePositionTowardDirection(position, direction, tiledArea.CenterPosition, tiledArea.MainAreaDimensions);
                }
            }
            return positions;
        }
    }
}
