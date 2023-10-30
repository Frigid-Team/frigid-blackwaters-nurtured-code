using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AngleOffsetDirection : Direction
    {
        [SerializeField]
        private Direction originalDirection;
        [SerializeField]
        private FloatSerializedReference angleOffset;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            float angle = this.angleOffset.MutableValue;
            Vector2[] directions = this.originalDirection.Retrieve(currDirections, elapsedDuration, elapsedDurationDelta);
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = directions[i].RotateAround(angle);
            }
            return directions;
        }
    }
}
