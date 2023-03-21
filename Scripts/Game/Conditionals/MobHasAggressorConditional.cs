using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobHasAggressorConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.ImmutableValue.TryGetAggressor(out _);
        }
    }
}
