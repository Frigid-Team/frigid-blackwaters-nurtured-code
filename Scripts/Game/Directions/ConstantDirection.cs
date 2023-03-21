using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ConstantDirection : Direction
    {
        [SerializeField]
        private Vector2SerializedReference constantDirection;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = this.constantDirection.MutableValue.normalized;
            return directions;
        }
    }
}
