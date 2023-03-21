using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TowardTargeterDirection : Direction
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private Targeter destinationTargeter;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            Vector2[] origins = this.originTargeter.Calculate(new Vector2[currDirections.Length], elapsedDuration, elapsedDurationDelta);
            Vector2[] destinations = this.destinationTargeter.Calculate(origins, elapsedDuration, elapsedDurationDelta);
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = (destinations[i] - origins[i]).normalized;
            }
            return directions;
        }
    }
}
