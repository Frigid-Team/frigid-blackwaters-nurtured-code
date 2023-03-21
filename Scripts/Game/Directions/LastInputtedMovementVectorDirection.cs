using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class LastInputtedMovementVectorDirection : Direction
    {
        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = CharacterInput.LastInputtedMovementVector.normalized;
            return directions;
        }
    }
}
