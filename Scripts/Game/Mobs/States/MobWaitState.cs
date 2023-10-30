using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobWaitState : MobState
    {
        [SerializeField]
        private string waitAnimationName;

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

        public sealed override MobStatus Status
        {
            get
            {
                return MobStatus.Waiting;
            }
        }

        public override void EnterSelf()
        {
            base.EnterSelf();
            this.Owner.StopVelocities.Request();
            this.Owner.StopReceivingDamage.Request();
            this.Owner.RequestPushMode(MobPushMode.IgnoreEverything);

            this.OwnerAnimatorBody.Play(this.waitAnimationName);
        }

        public override void ExitSelf()
        {
            base.ExitSelf();
            this.Owner.StopVelocities.Release();
            this.Owner.StopReceivingDamage.Release();
            this.Owner.ReleasePushMode(MobPushMode.IgnoreEverything);
        }

        protected override HashSet<MobStateNode> SpawnStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>() { this };
            }
        }

        protected override HashSet<MobStateNode> MoveStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>() { this };
            }
        }

        protected override HashSet<MobStateNode> ChildStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>();
            }
        }
    }
}
