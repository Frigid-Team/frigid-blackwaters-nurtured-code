using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobFacingDirection : Direction
    {
        [SerializeField]
        private MobSerializedReference mob;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = this.mob.ImmutableValue.FacingDirection;
            }
            return directions;
        }
    }
}
