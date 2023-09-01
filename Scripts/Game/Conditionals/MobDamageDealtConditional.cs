using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDamageDealtConditional : Conditional
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

            int damageDealt = 0;
            foreach (HitInfo hitInfo in mob.HitsDealt)
            {
                if (Time.time - hitInfo.TimeHit >= this.lastingDuration.ImmutableValue + Time.deltaTime)
                {
                    break;
                }
                damageDealt += hitInfo.Damage;

            }
            return damageDealt;
        }

    }
}
