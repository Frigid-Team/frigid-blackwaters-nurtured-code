using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Targeter : FrigidMonoBehaviour
    {
        public Vector2 Calculate(Vector2 currentPosition, float elapsedDuration, float elapsedDurationDelta)
        {
            return Calculate(new Vector2[] { currentPosition }, elapsedDuration, elapsedDurationDelta)[0];
        }

        public abstract Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta);
    }
}
