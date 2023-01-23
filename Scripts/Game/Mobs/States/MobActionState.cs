using FrigidBlackwaters.Core;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobActionState : MobState
    {
        [SerializeField]
        private StringSerializedReference actionAnimationName;
        [SerializeField]
        private Direction facingDirection;
        [SerializeField]
        private MobStaggerState staggerState;
        [SerializeField]
        private MobWaitState waitState;
        [SerializeField]
        private MobDeathState deathState;

        private MobState chosenState;
        private float elapsedDuration;

        public override bool Dead
        {
            get
            {
                return false;
            }
        }

        public override MobState InitialState
        {
            get
            {
                return this;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                return new HashSet<MobStateNode> { this.staggerState, this.waitState, this.deathState };
            }
        }

        public override void Setup()
        {
            base.Setup();
            this.chosenState = this;
            this.elapsedDuration = 0;
        }

        public override void Enter()
        {
            base.Enter();
            this.Owner.Health.OnCurrentHealthChanged += CheckTransitions;
            this.Owner.TiledArea.OnTransitionStarted += CheckTransitions;
            this.Owner.TiledArea.OnTransitionFinished += CheckTransitions;
            this.Owner.TiledArea.OnClosed += CheckTransitions;
            this.Owner.TiledArea.OnOpened += CheckTransitions;
            this.Owner.OnTiledAreaChanged += ChangeTiledAreaCallbacks;
            if (this.chosenState == this)
            {
                this.elapsedDuration = 0;
                this.OwnerAnimatorBody.Direction = this.facingDirection.Calculate(this.OwnerAnimatorBody.Direction, this.elapsedDuration, Time.deltaTime);
                this.OwnerAnimatorBody.PlayByName(this.actionAnimationName.MutableValue);
            }
            else
            {
                this.chosenState.OnCurrentStateChanged += SetCurrentStateFromChosenState;
                this.chosenState.Enter();
            }
            SetCurrentStateFromChosenState();
            CheckTransitions();
        }

        public override void Exit()
        {
            base.Exit();
            this.Owner.Health.OnCurrentHealthChanged -= CheckTransitions;
            this.Owner.TiledArea.OnTransitionStarted -= CheckTransitions;
            this.Owner.TiledArea.OnTransitionFinished -= CheckTransitions;
            this.Owner.TiledArea.OnClosed -= CheckTransitions;
            this.Owner.TiledArea.OnOpened -= CheckTransitions;
            this.Owner.OnTiledAreaChanged -= ChangeTiledAreaCallbacks;
            if (this.chosenState == this)
            {
                this.OwnerAnimatorBody.PlayEmpty();
            }
            else
            {
                this.chosenState.OnCurrentStateChanged -= SetCurrentStateFromChosenState;
                this.chosenState.Exit();
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            if (this.chosenState == this)
            {
                this.elapsedDuration += Time.deltaTime;
                this.OwnerAnimatorBody.Direction = this.facingDirection.Calculate(this.OwnerAnimatorBody.Direction, this.elapsedDuration, Time.deltaTime);
            }
            else
            {
                this.chosenState.Refresh();
            }
        }

        private void CheckTransitions()
        {
            if (false /* Staggered check here */)
            {
                if (this.chosenState == this)
                {
                    SetChosenState(this.staggerState);
                }
            }
            else
            {
                if (this.chosenState == this.staggerState)
                {
                    SetChosenState(this);
                }
            }

            if (this.Owner.TiledArea.IsTransitioning || !this.Owner.TiledArea.IsOpened)
            {
                if (this.chosenState == this || this.chosenState == this.staggerState)
                {
                    SetChosenState(this.waitState);
                }
            }
            else
            {
                if (this.chosenState == this.waitState)
                {
                    SetChosenState(this);
                }
            }

            if (this.Owner.Health.CurrentHealth <= 0)
            {
                if (this.chosenState == this || this.chosenState == this.staggerState)
                {
                    SetChosenState(this.deathState);
                }
            }
        }

        private void CheckTransitions(int previousCurrentHealth, int currentHealth)
        {
            CheckTransitions();
        }

        private void ChangeTiledAreaCallbacks(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            previousTiledArea.OnTransitionStarted -= CheckTransitions;
            previousTiledArea.OnTransitionFinished -= CheckTransitions;
            previousTiledArea.OnClosed -= CheckTransitions;
            previousTiledArea.OnOpened -= CheckTransitions;

            currentTiledArea.OnTransitionStarted += CheckTransitions;
            currentTiledArea.OnTransitionFinished += CheckTransitions;
            currentTiledArea.OnClosed += CheckTransitions;
            currentTiledArea.OnOpened += CheckTransitions;
        }

        private void SetChosenState(MobState chosenState)
        {
            if (this.chosenState == this)
            {
                this.elapsedDuration = 0;
                this.OwnerAnimatorBody.Direction = this.facingDirection.Calculate(this.OwnerAnimatorBody.Direction, this.elapsedDuration, Time.deltaTime);
                this.OwnerAnimatorBody.PlayEmpty();
            }
            else
            {
                this.chosenState.OnCurrentStateChanged -= SetCurrentStateFromChosenState;
                this.chosenState.Exit();
            }

            this.chosenState = chosenState;

            if (this.chosenState == this)
            {
                this.OwnerAnimatorBody.PlayByName(this.actionAnimationName.MutableValue);
            }
            else
            {
                this.chosenState.OnCurrentStateChanged += SetCurrentStateFromChosenState;
                this.chosenState.Enter();
            }

            SetCurrentStateFromChosenState();
        }

        private void SetCurrentStateFromChosenState(MobState previousState, MobState newState)
        {
            SetCurrentStateFromChosenState();
        }

        private void SetCurrentStateFromChosenState()
        {
            SetCurrentState(this.chosenState.CurrentState);
        }
    }
}
