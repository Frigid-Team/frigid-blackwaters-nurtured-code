using UnityEngine;

namespace FrigidBlackwaters.Game 
{
    public class MobMissingHealthPercentConditional : FloatComparisonConditional
    {
        [SerializeField]
        private MobSerializedHandle mob;

        protected override float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            if (this.mob.TryGetValue(out Mob mob))
            {
                return 1f - ((float)mob.RemainingHealth) / mob.MaxHealth;
            }
            return 0f;
        }
    }
}
