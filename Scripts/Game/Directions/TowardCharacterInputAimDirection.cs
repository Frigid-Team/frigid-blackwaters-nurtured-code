using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TowardCharacterInputAimDirection : Direction
    {
        [SerializeField]
        private Targeter originTargeter;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = (CharacterInput.AimWorldPosition - this.originTargeter.Retrieve(Vector2.zero, elapsedDuration, elapsedDurationDelta)).normalized;
            return directions;
        }
    }
}
