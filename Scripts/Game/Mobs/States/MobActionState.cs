using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobActionState : MobState
    {
        [Space]
        [SerializeField]
        private string actionAnimationName;
        [SerializeField]
        private Direction facingDirection;
        [SerializeField]
        private MobWaitState waitState;
        [SerializeField]
        private MobDeathState deathState;

        private bool actionAnimationCompleted;

        public override bool AutoEnter
        {
            get
            {
                if (this.ChosenStateNode == this)
                {
                    return false;
                }
                else
                {
                    return this.ChosenStateNode.AutoEnter;
                }
            }
        }

        public override bool AutoExit
        {
            get
            {
                if (this.ChosenStateNode == this)
                {
                    return this.actionAnimationCompleted;
                }
                else
                {
                    return this.ChosenStateNode.AutoExit;
                }
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return this.ChosenStateNode == this || this.ChosenStateNode.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.ChosenStateNode == this || this.ChosenStateNode.ShouldExit;
            }
        }

        public sealed override MobStatus Status
        {
            get
            {
                return MobStatus.Acting;
            }
        }

        public sealed override void Enter()
        {
            base.Enter();
            this.Owner.OnRemainingHealthChanged += this.CheckTransitions;
            this.Owner.TiledArea.OnTransitionStarted += this.CheckTransitions;
            this.Owner.TiledArea.OnTransitionFinished += this.CheckTransitions;
            this.Owner.TiledArea.OnClosed += this.CheckTransitions;
            this.Owner.TiledArea.OnOpened += this.CheckTransitions;
            this.Owner.OnTiledAreaChanged += this.SetTiledAreaCallbacks;
            this.CheckTransitions();
        }

        public sealed override void Exit()
        {
            base.Exit();
            this.Owner.OnRemainingHealthChanged -= this.CheckTransitions;
            this.Owner.TiledArea.OnTransitionStarted -= this.CheckTransitions;
            this.Owner.TiledArea.OnTransitionFinished -= this.CheckTransitions;
            this.Owner.TiledArea.OnClosed -= this.CheckTransitions;
            this.Owner.TiledArea.OnOpened -= this.CheckTransitions;
            this.Owner.OnTiledAreaChanged -= this.SetTiledAreaCallbacks;
        }

        public sealed override void Refresh()
        {
            base.Refresh();
            this.CheckTransitions();
        }

        public override void EnterSelf()
        {
            base.EnterSelf();
            if (this.Owner.IsActingAndNotStunned)
            {
                Vector2 directionToFace = this.facingDirection.Retrieve(this.OwnerAnimatorBody.Direction, this.SelfEnterDuration, this.SelfEnterDurationDelta);
                if (directionToFace.magnitude > 0) this.OwnerAnimatorBody.Direction = directionToFace;
            }
            this.actionAnimationCompleted = false;
            this.OwnerAnimatorBody.Play(this.actionAnimationName, () => { this.actionAnimationCompleted = true; });
        }

        public override void RefreshSelf()
        {
            base.RefreshSelf();
            if (this.Owner.IsActingAndNotStunned)
            {
                Vector2 directionToFace = this.facingDirection.Retrieve(this.OwnerAnimatorBody.Direction, this.SelfEnterDuration, this.SelfEnterDurationDelta);
                if (directionToFace.magnitude > 0) this.OwnerAnimatorBody.Direction = directionToFace;
            }
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
                return new HashSet<MobStateNode> { this.waitState, this.deathState };
            }
        }

        private void CheckTransitions()
        {
            MobStateNode nextStateNode;
            if (this.ChosenStateNode == this && this.waitState.ShouldEnter && this.waitState.AutoEnter)
            {
                nextStateNode = this.waitState;
            }
            else if (this.ChosenStateNode == this.waitState && this.waitState.ShouldExit && this.waitState.AutoExit)
            {
                nextStateNode = this;
            }
            else if (this.ChosenStateNode == this && this.deathState.ShouldEnter && this.deathState.AutoEnter)
            {
                nextStateNode = this.deathState;
            }
            else if (this.ChosenStateNode == this.deathState && this.deathState.ShouldExit && this.deathState.AutoExit)
            {
                nextStateNode = this;
            }
            else
            {
                return;
            }

            if (this.CanSetChosenStateNode(nextStateNode))
            {
                this.SetChosenStateNode(nextStateNode);
            }
        }

        private void CheckTransitions(int previousCurrentHealth, int currentHealth)
        {
            this.CheckTransitions();
        }

        private void SetTiledAreaCallbacks(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            previousTiledArea.OnTransitionStarted -= this.CheckTransitions;
            previousTiledArea.OnTransitionFinished -= this.CheckTransitions;
            previousTiledArea.OnClosed -= this.CheckTransitions;
            previousTiledArea.OnOpened -= this.CheckTransitions;

            currentTiledArea.OnTransitionStarted += this.CheckTransitions;
            currentTiledArea.OnTransitionFinished += this.CheckTransitions;
            currentTiledArea.OnClosed += this.CheckTransitions;
            currentTiledArea.OnOpened += this.CheckTransitions;

            this.CheckTransitions();
        }
    }
}
