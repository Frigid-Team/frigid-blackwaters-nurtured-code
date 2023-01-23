using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class DashingDirection : Direction
    {
        [SerializeField]
        private MoveByDashing moveByDashing;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = this.moveByDashing.DashDirection;
            return directions;
        }
    }
}
