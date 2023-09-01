using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TargeterToTargeterDirection : Direction
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private Targeter destinationTargeter;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            Vector2[] origins = this.originTargeter.Retrieve(new Vector2[currDirections.Length], elapsedDuration, elapsedDurationDelta);
            Vector2[] destinations = this.destinationTargeter.Retrieve(origins, elapsedDuration, elapsedDurationDelta);
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = (destinations[i] - origins[i]).normalized;
            }
            return directions;
        }
    }
}
