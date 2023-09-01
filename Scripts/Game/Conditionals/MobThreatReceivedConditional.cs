using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobThreatReceivedConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private FloatSerializedReference lastingDuration;
        [SerializeField]
        private IntSerializedReference numberThreatsRequired;
        [SerializeField]
        private FloatSerializedReference chanceOfNotOccuring;

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

            int numberThreats = 0;
            foreach (ThreatInfo threatInfo in mob.ThreatsReceived)
            {
                if (Time.time - threatInfo.TimeWarned >= this.lastingDuration.ImmutableValue + Time.deltaTime)
                {
                    break;
                }
                if ((this.numberThreatsRequired.ImmutableValue == 0 || mob.HitsReceived.Count % this.numberThreatsRequired.ImmutableValue == 0) &&
                    Random.Range(0f, 1f) >= this.chanceOfNotOccuring.ImmutableValue)
                {
                    numberThreats++;
                }
            }
            return numberThreats;
        }
    }
}
