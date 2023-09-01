using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobEquipmentStateNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<AbilityResource> inUseAbilityResources;

        private MobEquipment owner;
        private AnimatorBody ownerAnimatorBody;

        private MobEquipmentState currentState;
        private Action<MobEquipmentState, MobEquipmentState> onCurrentStateChanged;

        private bool entered;
        private float enterDuration;
        private float enterDurationDelta;

        public abstract MobEquipmentState InitialState
        {
            get;
        }

        public MobEquipmentState CurrentState
        {
            get
            {
                return this.currentState;
            }
            protected set
            {
                if (value == this.currentState) return;
                MobEquipmentState previousState = this.currentState;
                this.currentState = value;
                this.onCurrentStateChanged?.Invoke(previousState, this.currentState);
            }
        }

        public Action<MobEquipmentState, MobEquipmentState> OnCurrentStateChanged
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

        public abstract HashSet<MobEquipmentStateNode> ReferencedStateNodes
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

        public void Link(MobEquipment owner, AnimatorBody ownerAnimatorBody)
        {
            this.owner = owner;
            this.ownerAnimatorBody = ownerAnimatorBody;
        }

        public virtual void Init()
        {
            this.currentState = this.InitialState;
        }

        public virtual void Equipped() 
        {
            this.Owner.EquipPoint.Equipper.OnRequestedTimeScaleChanged += this.HandleEquipperTimeScaleChange;
            this.HandleEquipperTimeScaleChange();
        }

        public virtual void Unequipped() 
        {
            this.Owner.EquipPoint.Equipper.OnRequestedTimeScaleChanged -= this.HandleEquipperTimeScaleChange;
        }

        public virtual void Enter() 
        { 
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.EquipPoint.Equipper.AddBehaviour(behaviour, false);
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Request();
            this.entered = true;
            this.enterDuration = 0;
        }

        public virtual void Exit() 
        {
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.EquipPoint.Equipper.RemoveBehaviour(behaviour);
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Release();
            this.entered = false;
        }

        public virtual void Refresh() 
        {
            this.enterDurationDelta = Time.deltaTime * this.Owner.EquipPoint.Equipper.RequestedTimeScale;
            this.enterDuration += this.enterDurationDelta;
        }

        protected MobEquipment Owner
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

        private void HandleEquipperTimeScaleChange()
        {
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources)
            {
                inUseAbilityResource.LocalTimeScale = this.Owner.EquipPoint.Equipper.RequestedTimeScale;
            }
        }
    }
}
