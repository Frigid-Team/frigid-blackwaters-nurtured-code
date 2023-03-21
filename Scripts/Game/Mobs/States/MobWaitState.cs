using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobWaitState : MobState
    {
        [SerializeField]
        private string waitAnimationName;

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>();
            }
        }

        public override bool AutoEnter
        {
            get
            {
                return this.Owner.TiledArea.IsTransitioning || !this.Owner.TiledArea.IsOpened;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return true;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return true;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return !this.AutoEnter;
            }
        }

        public sealed override bool Dead
        {
            get
            {
                return false;
            }
        }

        public sealed override bool Waiting
        {
            get
            {
                return true;
            }
        }

        public override void Enter()
        {
            base.Enter();
            this.Owner.StopVelocities.Request();
            this.Owner.StopDealingDamage.Request();
            this.Owner.StopReceivingDamage.Request();
            this.Owner.RequestPushMode(MobPushMode.IgnoreEverything);

            this.OwnerAnimatorBody.Play(this.waitAnimationName);
        }

        public override void Exit()
        {
            base.Exit();
            this.Owner.StopVelocities.Release();
            this.Owner.StopDealingDamage.Release();
            this.Owner.StopReceivingDamage.Release();
            this.Owner.ReleasePushMode(MobPushMode.IgnoreEverything);
        }
    }
}
