using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Direction : FrigidMonoBehaviour
    {
        public Vector2 Retrieve(Vector2 currDirection, float elapsedDuration, float elapsedDurationDelta)
        {
            return this.Retrieve(new Vector2[] { currDirection }, elapsedDuration, elapsedDurationDelta)[0];
        }

        public Vector2[] Retrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            return this.CustomRetrieve(currDirections, elapsedDuration, elapsedDurationDelta);
        }

        protected abstract Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta);
    }
}
