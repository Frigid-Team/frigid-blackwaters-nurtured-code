using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MoveBySyncingToMob : Move
    {
        [SerializeField]
        private MobSerializedHandle syncedMob;

        public override Vector2 Velocity
        {
            get
            {
                return this.syncedMob.TryGetValue(out Mob mob) ? mob.CurrentVelocity : Vector2.zero;
            }
        }
    }
}
