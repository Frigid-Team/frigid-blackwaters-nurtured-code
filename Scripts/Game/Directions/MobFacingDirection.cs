using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobFacingDirection : Direction
    {
        [SerializeField]
        private MobSerializedHandle mob;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currDirections;
            }
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = mob.FacingDirection;
            }
            return directions;
        }
    }
}
