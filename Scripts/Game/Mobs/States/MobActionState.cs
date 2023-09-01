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

        private MobState chosenState;
        private bool actionAnimationCompleted;

        private bool enteredSelf;
        private float selfEnterDuration;
        private float selfEnterDurationDelta;

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                return new HashSet<MobStateNode> { this.waitState, this.deathState };
            }
        }

        public override bool AutoEnter
        {
            get
            {
                if (this.chosenState == this)
                {
                    return false;
                }
                else
                {
                    return this.chosenState.AutoEnter;
                }
            }
        }

        public override bool AutoExit
        {
            get
            {
                if (this.chosenState == this)
                {
                    return !this.OwnerAnimatorBody.IsLooping && this.actionAnimationCompleted;
                }
                else
                {
                    return this.chosenState.AutoExit;
                }
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                if (this.chosenState == this)
                {
                    return true;
                }
                else
                {
                    return this.chosenState.ShouldEnter;
                }
            }
        }

        public override bool ShouldExit
        {
            get
            {
                if (this.chosenState == this)
                {
                    return this.OwnerAnimatorBody.IsLooping || this.actionAnimationCompleted;
                }
                else
                {
                    return this.chosenState.ShouldExit;
                }
            }
        }

        public sealed override MobStatus Status
        {
            get
            {
                return MobStatus.Acting;
            }
        }

        public override void Init()
        {
            base.Init();
            this.chosenState = this;
            this.enteredSelf = false;
        }

        public override void Move()
        {
            base.Move();
            this.chosenState = this;
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
            if (this.chosenState == this)
            {
                this.EnterSelf();
            }
            else
            {
                this.chosenState.OnCurrentStateChanged += this.SetCurrentStateFromChosenState;
                this.chosenState.Enter();
            }
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
            if (this.chosenState == this)
            {
                this.ExitSelf();
            }
            else
            {
                this.chosenState.OnCurrentStateChanged -= this.SetCurrentStateFromChosenState;
                this.chosenState.Exit();
            }
        }

        public sealed override void Refresh()
        {
            base.Refresh();
            this.CheckTransitions();
            if (this.chosenState == this)
            {
                this.RefreshSelf();
            }
            else
            {
                this.chosenState.Refresh();
            }
        }
        
        protected bool EnteredSelf
        {
            get
            {
                return this.enteredSelf;
            }
        }

        protected float SelfEnterDuration
        {
            get
            {
                return this.selfEnterDuration;
            }
        }

        protected float SelfEnterDurationDelta
        {
            get
            {
                return this.selfEnterDurationDelta;
            }
        }

        protected virtual void EnterSelf()
        {
            this.enteredSelf = true;
            this.selfEnterDuration = 0;
            if (this.Owner.IsActingAndNotStunned)
            {
                Vector2 directionToFace = this.facingDirection.Retrieve(this.OwnerAnimatorBody.Direction, this.SelfEnterDuration, this.SelfEnterDurationDelta);
                if (directionToFace.magnitude > 0) this.OwnerAnimatorBody.Direction = directionToFace;
            }
            this.actionAnimationCompleted = false;
            this.OwnerAnimatorBody.Play(this.actionAnimationName, () => { this.actionAnimationCompleted = true; });
        }

        protected virtual void ExitSelf()
        {
            this.enteredSelf = false;
        }

        protected virtual void RefreshSelf()
        {
            this.selfEnterDurationDelta = Time.deltaTime * this.Owner.RequestedTimeScale;
            this.selfEnterDuration += this.selfEnterDurationDelta;
            if (this.Owner.IsActingAndNotStunned)
            {
                Vector2 directionToFace = this.facingDirection.Retrieve(this.OwnerAnimatorBody.Direction, this.SelfEnterDuration, this.SelfEnterDurationDelta);
                if (directionToFace.magnitude > 0) this.OwnerAnimatorBody.Direction = directionToFace;
            }
        }

        private void CheckTransitions()
        {
            if (this.chosenState == this && this.waitState.ShouldEnter && this.waitState.AutoEnter)
            {
                this.SetChosenState(this.waitState);
            }

            if (this.chosenState == this.waitState && this.waitState.ShouldExit && this.waitState.AutoExit)
            {
                this.SetChosenState(this);
            }

            if (this.chosenState == this && this.deathState.ShouldEnter && this.deathState.AutoEnter)
            {
                this.SetChosenState(this.deathState);
            }

            if (this.chosenState == this.deathState && this.deathState.ShouldExit && this.deathState.AutoExit)
            {
                this.SetChosenState(this);
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

        private bool CanSetChosenState(MobState chosenState)
        {
            if (chosenState == this) return this.CanSetCurrentState(this);
            else return this.CanSetCurrentState(chosenState.CurrentState);
        }

        private void SetChosenState(MobState chosenState)
        {
            if (this.CanSetChosenState(chosenState) && chosenState != this.chosenState)
            {
                if (this.Entered)
                {
                    if (this.chosenState == this)
                    {
                        this.ExitSelf();
                    }
                    else
                    {
                        this.chosenState.Exit();
                        this.chosenState.OnCurrentStateChanged -= this.SetCurrentStateFromChosenState;
                    }
                }

                this.chosenState = chosenState;
                this.SetCurrentStateFromChosenState();

                if (this.Entered)
                {
                    if (this.chosenState == this)
                    {
                        this.EnterSelf();
                    }
                    else
                    {
                        this.chosenState.OnCurrentStateChanged += this.SetCurrentStateFromChosenState;
                        this.chosenState.Enter();
                    }
                }
            }
        }

        private void SetCurrentStateFromChosenState(MobState previousState, MobState newState)
        {
            this.SetCurrentStateFromChosenState();
        }

        private void SetCurrentStateFromChosenState()
        {
            if (this.chosenState == this) this.SetCurrentState(this);
            else this.SetCurrentState(this.chosenState.CurrentState);
        }
    }
}
