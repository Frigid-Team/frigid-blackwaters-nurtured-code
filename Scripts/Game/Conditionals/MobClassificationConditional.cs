using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobClassificationConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private MobClassification mobClassification;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.ImmutableValue.Classification == this.mobClassification;
        }
    }
}
