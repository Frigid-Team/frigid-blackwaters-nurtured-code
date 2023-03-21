using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobHasFriendConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.ImmutableValue.TryGetFriend(out _);
        }
    }
}
