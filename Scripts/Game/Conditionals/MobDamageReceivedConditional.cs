using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDamageReceivedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private FloatSerializedReference lastingDuration;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.Tally(elapsedDuration, elapsedDurationDelta) > 0;
        }

        protected override int CustomTally(float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return 0;
            }

            int damageTaken = 0;
            foreach (HitInfo hitInfo in mob.HitsReceived)
            {
                if (Time.time - hitInfo.TimeHit >= this.lastingDuration.ImmutableValue + Time.deltaTime)
                {
                    break;
                }
                damageTaken += hitInfo.Damage;

            }
            return damageTaken;
        }
    }
}
