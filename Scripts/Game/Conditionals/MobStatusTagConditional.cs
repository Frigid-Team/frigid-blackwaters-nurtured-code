using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobStatusTagConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private MobStatusTag mobStatusTag;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.TryGetValue(out Mob mob) && mob.HasStatusTag(this.mobStatusTag);
        }
    }
}
