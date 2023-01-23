using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TowardCharacterInputAimDirection : Direction
    {
        [SerializeField]
        private Transform originPosition;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = CharacterInput.AimWorldPosition - (Vector2)this.originPosition.position;
            return directions;
        }
    }
}
