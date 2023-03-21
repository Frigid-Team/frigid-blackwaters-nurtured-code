using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobHasLineOfSightConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private Targeter sightTargeter;
        [SerializeField]
        private float blockingRadius;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.ImmutableValue.CanSeeThrough(this.mob.ImmutableValue.Position, this.sightTargeter.Calculate(this.mob.ImmutableValue.Position, elapsedDuration, elapsedDurationDelta), this.blockingRadius);
        }
    }
}
