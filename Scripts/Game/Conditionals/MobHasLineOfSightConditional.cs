using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobHasLineOfSightConditional : Conditional
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private Targeter sightTargeter;
        [SerializeField]
        private float blockingRadius;

        protected override bool CustomValidate()
        {
            return this.mob.Physicality.IsInSightFrom(this.mob.AbsolutePosition, this.sightTargeter.Calculate(this.mob.transform.position, 0, 0), this.blockingRadius);
        }
    }
}
