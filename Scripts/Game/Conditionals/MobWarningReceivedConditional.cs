using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobWarningReceivedConditional : Conditional
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private FloatSerializedReference lastingDuration;

        protected override bool CustomValidate()
        {
            return this.mob.DamageReceiver.ThreatsReceived.Count > 0 && Time.time - this.mob.DamageReceiver.ThreatsReceived.First.Value.TimeWarned <= this.lastingDuration.ImmutableValue;
        }
    }
}
