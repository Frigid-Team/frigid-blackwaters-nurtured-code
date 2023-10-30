using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobStunnedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.TryGetValue(out Mob mob) && mob.Stunned;
        }
    }
}
