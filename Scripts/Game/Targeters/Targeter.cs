using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Targeter : FrigidMonoBehaviour
    {
        public Vector2 Retrieve(Vector2 currentPosition, float elapsedDuration, float elapsedDurationDelta)
        {
            return this.Retrieve(new Vector2[] { currentPosition }, elapsedDuration, elapsedDurationDelta)[0];
        }

        public Vector2[] Retrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            return this.CustomRetrieve(currentPositions, elapsedDuration, elapsedDurationDelta);
        }

        protected abstract Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta);
    }
}
