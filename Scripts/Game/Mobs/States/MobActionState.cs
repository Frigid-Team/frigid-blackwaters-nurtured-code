using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobActionState : MobState
    {
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
                    return this.actionAnimationCompleted;
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
                return this.chosenState == this || this.chosenState.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.chosenState == this || this.chosenState.ShouldExit;
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
                return false;
            }
        }

        public override void Init()
        {
            base.Init();
            this.chosenState = this;
            this.enteredSelf = false;
        }

        public override void Switch()
        {
            base.Switch();
            this.chosenState = this;
        }

        public sealed override void Enter()
        {
            base.Enter();
            this.Owner.OnRemainingHealthChanged += CheckTransitions;
            this.Owner.TiledArea.OnTransitionStarted += CheckTransitions;
            this.Owner.TiledArea.OnTransitionFinished += CheckTransitions;
            this.Owner.TiledArea.OnClosed += CheckTransitions;
            this.Owner.TiledArea.OnOpened += CheckTransitions;
            this.Owner.OnTiledAreaChanged += SetTiledAreaCallbacks;
            if (this.chosenState == this)
            {
                EnterSelf();
            }
            else
            {
                this.chosenState.OnCurrentStateChanged += SetCurrentStateFromChosenState;
                this.chosenState.Enter();
            }
            CheckTransitions();
        }

        public sealed override void Exit()
        {
            base.Exit();
            this.Owner.OnRemainingHealthChanged -= CheckTransitions;
            this.Owner.TiledArea.OnTransitionStarted -= CheckTransitions;
            this.Owner.TiledArea.OnTransitionFinished -= CheckTransitions;
            this.Owner.TiledArea.OnClosed -= CheckTransitions;
            this.Owner.TiledArea.OnOpened -= CheckTransitions;
            this.Owner.OnTiledAreaChanged -= SetTiledAreaCallbacks;
            if (this.chosenState == this)
            {
                ExitSelf();
            }
            else
            {
                this.chosenState.OnCurrentStateChanged -= SetCurrentStateFromChosenState;
                this.chosenState.Exit();
            }
        }

        public sealed override void Refresh()
        {
            base.Refresh();
            CheckTransitions();
            if (this.chosenState == this)
            {
                RefreshSelf();
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
            if (this.Owner.CanAct)
            {
                Vector2 directionToFace = this.facingDirection.Calculate(this.OwnerAnimatorBody.Direction, this.SelfEnterDuration, this.SelfEnterDurationDelta);
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
            if (this.Owner.CanAct)
            {
                Vector2 directionToFace = this.facingDirection.Calculate(this.OwnerAnimatorBody.Direction, this.SelfEnterDuration, this.SelfEnterDurationDelta);
                if (directionToFace.magnitude > 0) this.OwnerAnimatorBody.Direction = directionToFace;
            }
        }

        private void CheckTransitions()
        {
            if (this.chosenState == this && this.waitState.ShouldEnter && this.waitState.AutoEnter)
            {
                SetChosenState(this.waitState);
            }

            if (this.chosenState == this.waitState && this.waitState.ShouldExit && this.waitState.AutoExit)
            {
                SetChosenState(this);
            }

            if (this.chosenState == this && this.deathState.ShouldEnter && this.deathState.AutoEnter)
            {
                SetChosenState(this.deathState);
            }

            if (this.chosenState == this.deathState && this.deathState.ShouldExit && this.deathState.AutoExit)
            {
                SetChosenState(this);
            }
        }

        private void CheckTransitions(int previousCurrentHealth, int currentHealth)
        {
            CheckTransitions();
        }

        private void SetTiledAreaCallbacks(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            previousTiledArea.OnTransitionStarted -= CheckTransitions;
            previousTiledArea.OnTransitionFinished -= CheckTransitions;
            previousTiledArea.OnClosed -= CheckTransitions;
            previousTiledArea.OnOpened -= CheckTransitions;

            currentTiledArea.OnTransitionStarted += CheckTransitions;
            currentTiledArea.OnTransitionFinished += CheckTransitions;
            currentTiledArea.OnClosed += CheckTransitions;
            currentTiledArea.OnOpened += CheckTransitions;

            CheckTransitions();
        }

        private bool CanSetChosenState(MobState chosenState)
        {
            if (chosenState == this) return CanSetCurrentState(this);
            else return CanSetCurrentState(chosenState.CurrentState);
        }

        private void SetChosenState(MobState chosenState)
        {
            if (CanSetChosenState(chosenState) && chosenState != this.chosenState)
            {
                if (this.Entered)
                {
                    if (this.chosenState == this)
                    {
                        ExitSelf();
                    }
                    else
                    {
                        this.chosenState.Exit();
                        this.chosenState.OnCurrentStateChanged -= SetCurrentStateFromChosenState;
                    }
                }

                this.chosenState = chosenState;
                SetCurrentStateFromChosenState();

                if (this.Entered)
                {
                    if (this.chosenState == this)
                    {
                        EnterSelf();
                    }
                    else
                    {
                        this.chosenState.OnCurrentStateChanged += SetCurrentStateFromChosenState;
                        this.chosenState.Enter();
                    }
                }
            }
        }

        private void SetCurrentStateFromChosenState(MobState previousState, MobState newState)
        {
            SetCurrentStateFromChosenState();
        }

        private void SetCurrentStateFromChosenState()
        {
            if (this.chosenState == this) SetCurrentState(this);
            else SetCurrentState(this.chosenState.CurrentState);
        }
    }
}
