using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobSetPushModeBehaviour : MobBehaviour
    {
        [SerializeField]
        private MobPushMode pushMode;

        public override void Enter()
        {
            base.Enter();
            this.Owner.RequestPushMode(this.pushMode);
        }

        public override void Exit()
        {
            base.Exit();
            this.Owner.ReleasePushMode(this.pushMode);
        }
    }
}
