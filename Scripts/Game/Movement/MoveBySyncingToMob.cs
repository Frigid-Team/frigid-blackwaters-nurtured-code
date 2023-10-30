using FrigidBlackwaters.Core;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MoveBySyncingToMob : Move
    {
        [SerializeField]
        private MobSerializedHandle syncedMob;
        [SerializeField]
        private FloatSerializedReference speedMultiplier;

        public override Vector2 Velocity
        {
            get
            {
                return (this.syncedMob.TryGetValue(out Mob mob) ? mob.CurrentVelocity : Vector2.zero) * this.speedMultiplier.ImmutableValue;
            }
        }
    }
}
