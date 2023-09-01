using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class CalculatedVelocityDirection : Direction
    {
        [SerializeField]
        private Mover mover;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = this.mover.CalculatedVelocity.normalized;
            return directions;
        }
    }
}
