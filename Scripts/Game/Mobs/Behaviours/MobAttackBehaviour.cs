using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobAttackBehaviour : MobBehaviour
    {
        [SerializeField]
        private Attack attack;

        public override bool IsFinished
        {
            get
            {
                return true;
            }
        }

        public override void Enter()
        {
            base.Enter();
            this.attack.DamageAlignment = this.Owner.Alignment;
            this.attack.Perform(this.EnterDuration);
        }
    }
}