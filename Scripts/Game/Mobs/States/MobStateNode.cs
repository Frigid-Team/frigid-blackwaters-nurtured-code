using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobStateNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobBehaviour> behaviours;

        private Mob owner;
        private AnimatorBody ownerAnimatorBody;

        private MobState currentState;
        private Action<MobState, MobState> onCurrentStateChanged;

        public abstract MobState InitialState
        {
            get;
        }

        public abstract HashSet<MobStateNode> ReferencedStateNodes
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

        public void Assign(Mob owner, AnimatorBody ownerAnimatorBody)
        {
            this.owner = owner;
            this.ownerAnimatorBody = ownerAnimatorBody;
        }

        public virtual void Setup()
        {
            this.currentState = this.InitialState;
        }

        public virtual void Enter() 
        {
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.AddBehaviour(behaviour);
        }

        public virtual void Exit() 
        {
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.RemoveBehaviour(behaviour);
        }

        public virtual void Refresh() { }

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

        protected void SetCurrentState(MobState newState)
        {
            if (newState == this.currentState) return;
            MobState previousState = this.currentState;
            this.currentState = newState;
            this.onCurrentStateChanged?.Invoke(previousState, this.currentState);
        }
    }
}
