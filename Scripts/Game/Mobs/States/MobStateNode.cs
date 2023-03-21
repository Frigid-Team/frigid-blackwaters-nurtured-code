using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobStateNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<Timer> timers;

        private Mob owner;
        private AnimatorBody ownerAnimatorBody;

        private MobState currentState;
        private Action<MobState, MobState> onCurrentStateChanged;

        private bool entered;
        private float enterDuration;
        private float enterDurationDelta;

        public abstract HashSet<MobState> InitialStates
        {
            get;
        }

        public abstract HashSet<MobState> SwitchableStates
        {
            get;
        }

        public MobState CurrentState
        {
            get
            {
                return this.currentState;
            }
        }

        public Action<MobState, MobState> OnCurrentStateChanged
        {
            get
            {
                return this.onCurrentStateChanged;
            }
            set
            {
                this.onCurrentStateChanged = value;
            }
        }

        public abstract HashSet<MobStateNode> ReferencedStateNodes
        {
            get;
        }

        public virtual bool AutoEnter
        {
            get
            {
                return false;
            }
        }

        public virtual bool AutoExit
        {
            get
            {
                return false;
            }
        }

        public virtual bool ShouldEnter
        {
            get
            {
                return true;
            }
        }

        public virtual bool ShouldExit
        {
            get
            {
                return true;
            }
        }

        public bool HasValidInitialState(TiledArea tiledArea, Vector2 position)
        {
            return TryGetInitialState(tiledArea, position, out _);
        }

        public bool HasValidSwitchState(TiledArea tiledArea, Vector2 position)
        {
            return TryGetSwitchState(tiledArea, position, out _);
        }

        public void Link(Mob owner, AnimatorBody ownerAnimatorBody)
        {
            this.owner = owner;
            this.ownerAnimatorBody = ownerAnimatorBody;
        }

        public virtual void Init() 
        {
            this.entered = false;

            this.Owner.OnRequestedTimeScaleChanged += HandleOwnerTimeScaleChange;
            HandleOwnerTimeScaleChange();

            if (TryGetInitialState(this.Owner.TiledArea, this.Owner.Position, out MobState initialState)) this.currentState = initialState;
            else this.currentState = this.InitialStates.First();
        }

        public virtual void Switch()
        {
            if (TryGetSwitchState(this.Owner.TiledArea, this.Owner.Position, out MobState switchState) && this.currentState != switchState)
            {
                MobState previousState = this.currentState;
                this.currentState = switchState;
                onCurrentStateChanged?.Invoke(previousState, this.currentState);
            }
        }

        public virtual void Enter() 
        {
            this.entered = true;
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.AddBehaviour(behaviour, false);
            foreach (Timer timer in this.timers) timer.InUse.Request();
            this.enterDuration = 0;
            this.enterDurationDelta = 0;
        }

        public virtual void Exit() 
        {
            this.entered = false;
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.RemoveBehaviour(behaviour);
            foreach (Timer timer in this.timers) timer.InUse.Release();
        }

        public virtual void Refresh()
        {
            this.enterDurationDelta = Time.deltaTime * this.Owner.RequestedTimeScale;
            this.enterDuration += this.enterDurationDelta;
        }

        protected Mob Owner
        {
            get
            {
                return this.owner;
            }
        }

        protected AnimatorBody OwnerAnimatorBody
        {
            get
            {
                return this.ownerAnimatorBody;
            }
        }

        protected bool Entered
        {
            get
            {
                return this.entered;
            }
        }

        protected float EnterDuration
        {
            get
            {
                return this.enterDuration;
            }
        }

        protected float EnterDurationDelta
        {
            get
            {
                return this.enterDurationDelta;
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject()
        {
            return true;
        }
#endif

        protected bool CanSetCurrentState(MobState newState)
        {
            return
                newState.TileSize == this.CurrentState.TileSize && newState.TraversableTerrain >= this.CurrentState.TraversableTerrain || 
                Mob.CanFitAt(this.Owner.TiledArea, this.Owner.Position, newState.Size, newState.TraversableTerrain);
        }

        protected void SetCurrentState(MobState newState)
        {
            if (!CanSetCurrentState(newState))
            {
                Debug.LogError("Mob State Node " + this.name + " is trying to set a MobState that is unviable.");
                return;
            }

            if (newState == this.currentState) return;
            MobState previousState = this.currentState;
            this.currentState = newState;
            this.onCurrentStateChanged?.Invoke(previousState, this.currentState);
        }

        private bool TryGetInitialState(TiledArea tiledArea, Vector2 position, out MobState initialState)
        {
            initialState = null;
            foreach (MobState potentialInitialState in this.InitialStates)
            {
                initialState = potentialInitialState;
                if (Mob.CanFitAt(tiledArea, position, initialState.Size, initialState.TraversableTerrain))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryGetSwitchState(TiledArea tiledArea, Vector2 position, out MobState switchState)
        {
            switchState = null;
            foreach (MobState potentialSwitchState in this.SwitchableStates)
            {
                switchState = potentialSwitchState;
                if (Mob.CanFitAt(tiledArea, position, switchState.Size, switchState.TraversableTerrain))
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleOwnerTimeScaleChange()
        {
            foreach (Timer timer in this.timers)
            {
                timer.LocalTimeScale = this.Owner.RequestedTimeScale;
            }
        }
    }
}
