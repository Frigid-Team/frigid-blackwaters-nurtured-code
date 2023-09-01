using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobStateNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobStatusTag> statusTags;
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<AbilityResource> inUseAbilityResources;

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

        public abstract HashSet<MobState> MoveStates
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

        public abstract bool AutoEnter
        {
            get;
        }

        public abstract bool AutoExit
        {
            get;
        }

        public abstract bool ShouldEnter
        {
            get;
        }

        public abstract bool ShouldExit
        {
            get;
        }

        public bool HasValidInitialState(TiledArea tiledArea, Vector2 position)
        {
            return this.TryGetInitialState(tiledArea, position, out _);
        }

        public bool HasValidMoveState(TiledArea tiledArea, Vector2 position)
        {
            return this.TryGetMoveState(tiledArea, position, out _);
        }

        public void Link(Mob owner, AnimatorBody ownerAnimatorBody)
        {
            this.owner = owner;
            this.ownerAnimatorBody = ownerAnimatorBody;
        }

        public virtual void Init() 
        {
            this.entered = false;

            this.Owner.OnRequestedTimeScaleChanged += this.HandleOwnerTimeScaleChange;
            this.HandleOwnerTimeScaleChange();

            if (this.TryGetInitialState(this.Owner.TiledArea, this.Owner.Position, out MobState initialState)) this.currentState = initialState;
            else this.currentState = this.InitialStates.First();
        }

        public virtual void Move()
        {
            MobState bestMoveState;
            if (!this.TryGetMoveState(this.Owner.TiledArea, this.Owner.Position, out bestMoveState))
            {
                bestMoveState = this.MoveStates.First();
            }
            if (this.currentState != bestMoveState)
            {
                MobState previousState = this.currentState;
                this.currentState = bestMoveState;
                this.onCurrentStateChanged?.Invoke(previousState, this.currentState);
            }
        }

        public virtual void Enter() 
        {
            this.entered = true;
            foreach (MobStatusTag statusTag in this.statusTags) this.Owner.AddStatusTag(statusTag);
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.AddBehaviour(behaviour, false);
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Request();
            this.enterDuration = 0;
            this.enterDurationDelta = 0;
        }

        public virtual void Exit() 
        {
            this.entered = false;
            foreach (MobStatusTag statusTag in this.statusTags) this.Owner.RemoveStatusTag(statusTag);
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.RemoveBehaviour(behaviour);
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Release();
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
                Mob.CanTraverseAt(this.Owner.TiledArea, this.Owner.Position, newState.Size, newState.TraversableTerrain);
        }

        protected void SetCurrentState(MobState newState)
        {
            if (!this.CanSetCurrentState(newState))
            {
                Debug.LogError("MobStateNode " + this.name + " is trying to set a MobState that is unviable.");
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
                if (Mob.CanTraverseAt(tiledArea, position, initialState.Size, initialState.TraversableTerrain))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryGetMoveState(TiledArea tiledArea, Vector2 position, out MobState moveState)
        {
            moveState = null;
            foreach (MobState potentialMoveState in this.MoveStates)
            {
                moveState = potentialMoveState;
                if (Mob.CanTraverseAt(tiledArea, position, moveState.Size, moveState.TraversableTerrain))
                {
                    return true;
                }
            }
            return false;
        }

        private void HandleOwnerTimeScaleChange()
        {
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources)
            {
                inUseAbilityResource.LocalTimeScale = this.Owner.RequestedTimeScale;
            }
        }
    }
}
