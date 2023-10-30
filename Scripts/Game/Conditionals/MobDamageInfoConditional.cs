using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class MobDamageInfoConditional<DI> : Conditional where DI : DamageInfo
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private FloatSerializedReference lastingDuration;
        [SerializeField]
        private FloatSerializedReference chanceOfNotOccuring;
        [SerializeField]
        private bool onlyOnNonTrivial;

        protected sealed override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.Tally(elapsedDuration, elapsedDurationDelta) > 0;
        }

        protected sealed override int CustomTally(float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return 0;
            }

            int tally = 0;
            foreach (DI damageInfo in this.GetDamageInfos(mob))
            {
                // Since damage infos are constructed only during physics updates, we can check for their fixed time directly. 
                if (Time.fixedTime - damageInfo.TimeOfIncident > this.lastingDuration.ImmutableValue)
                {
                    break;
                }
                if (Random.Range(0f, 1f) >= this.chanceOfNotOccuring.ImmutableValue && (!this.onlyOnNonTrivial || damageInfo.IsNonTrivial))
                {
                    tally += this.TallyDamageInfo(damageInfo);
                }
            }
            return tally;
        }

        protected abstract LinkedList<DI> GetDamageInfos(Mob mob);

        protected abstract int TallyDamageInfo(DI damageInfo);
    }
}
