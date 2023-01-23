using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Direction : FrigidMonoBehaviour
    {
        public Vector2 Calculate(Vector2 currDirection, float elapsedDuration, float elapsedDurationDelta)
        {
            return Calculate(new Vector2[] { currDirection }, elapsedDuration, elapsedDurationDelta)[0];
        }

        public abstract Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta);
    }
}
