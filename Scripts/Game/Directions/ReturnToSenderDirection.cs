using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ReturnToSenderDirection : Direction
    {
        [SerializeField]
        private Direction baseDirection;
        [SerializeField]
        private FloatSerializedReference returnDuration;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            Vector2[] baseDirections = this.baseDirection.Retrieve(currDirections, elapsedDuration, elapsedDurationDelta);
            for (int i = 0; i < directions.Length; i++) directions[i] = baseDirections[i] * (elapsedDuration < this.returnDuration.ImmutableValue ? 1 : -1);
            return directions;
        }
    }
}
