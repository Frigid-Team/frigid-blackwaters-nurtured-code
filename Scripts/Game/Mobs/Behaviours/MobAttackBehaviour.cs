using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobAttackBehaviour : MobBehaviour
    {
        [SerializeField]
        private Attack attack;

        private bool attackComplete;

        public override bool IsFinished
        {
            get
            {
                return this.attackComplete;
            }
        }

        public override void Enter()
        {
            base.Enter();
            this.attack.DamageAlignment = this.Owner.Alignment;
            this.attackComplete = false;
            this.attack.Perform(this.EnterDuration, () => this.attackComplete = true);
        }
    }
}