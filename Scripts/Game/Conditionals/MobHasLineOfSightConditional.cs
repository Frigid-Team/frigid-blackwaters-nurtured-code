using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobHasLineOfSightConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private Targeter sightTargeter;
        [SerializeField]
        private float blockingRadius;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.TryGetValue(out Mob mob) && mob.CanSeeUnobstructed(mob.Position, this.sightTargeter.Retrieve(mob.Position, elapsedDuration, elapsedDurationDelta), this.blockingRadius);
        }
    }
}
