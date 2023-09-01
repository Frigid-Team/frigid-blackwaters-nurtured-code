using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobDesiredVelocityDirection : Direction
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private List<MobMovePriority> mobMovePriorities;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currDirections;
            }

            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = this.mobMovePriorities.Contains(mob.CurrentMovePriority) ? mob.DesiredVelocity.normalized : Vector2.zero;
            }
            return directions;
        }
    }
}

