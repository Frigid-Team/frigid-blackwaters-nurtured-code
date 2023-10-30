using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobEquipmentStateNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobStatusTag> statusTags;
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<AbilityResource> inUseAbilityResources;

        private MobEquipment owner;
        private AnimatorBody ownerAnimatorBody;

        private MobEquipmentStateNode chosenStateNode;
        private Action<MobEquipmentState, MobEquipmentState> onCurrentStateChanged;

        private bool entered;
        private float enterDuration;
        private float enterDurationDelta;

        public MobEquipmentState CurrentState
        {
            get
            {
                if (this.chosenStateNode != this)
                {
                    return this.chosenStateNode.CurrentState;
                }
                return (MobEquipmentState)this;
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

        public abstract bool AutoEnter
        {
            get;
        }

        public abstract bool AutoExit
        {
            get;
        }

        public void OwnedBy(MobEquipment owner, AnimatorBody ownerAnimatorBody)
        {
            this.owner = owner;
            this.ownerAnimatorBody = ownerAnimatorBody;

            foreach (MobEquipmentStateNode childStateNode in this.ChildStateNodes)
            {
                childStateNode.OwnedBy(owner, ownerAnimatorBody);
            }
        }

        public void CollectAbilityResources(ref List<AbilityResource> abilityResources)
        {
            abilityResources.AddRange(this.inUseAbilityResources);
            foreach (MobEquipmentStateNode childStateNode in this.ChildStateNodes)
            {
                childStateNode.CollectAbilityResources(ref abilityResources);
            }
            abilityResources = abilityResources.Distinct().ToList();
        }

        public virtual void Spawn()
        {
            this.chosenStateNode = this.SpawnStateNode;

            foreach (MobEquipmentStateNode childStateNode in this.ChildStateNodes)
            {
                childStateNode.Spawn();
            }

            this.SetChosenStateNode(this.SpawnStateNode);
        }

        public virtual void Equipped() 
        {
            this.Owner.EquipPoint.Equipper.OnRequestedTimeScaleChanged += this.HandleEquipperTimeScaleChange;
            this.HandleEquipperTimeScaleChange();

            foreach (MobEquipmentStateNode childStateNode in this.ChildStateNodes)
            {
                childStateNode.Equipped();
            }
        }

        public virtual void Unequipped() 
        {
            this.Owner.EquipPoint.Equipper.OnRequestedTimeScaleChanged -= this.HandleEquipperTimeScaleChange;

            foreach (MobEquipmentStateNode childStateNode in this.ChildStateNodes)
            {
                childStateNode.Unequipped();
            }
        }

        public virtual void Enter() 
        {
            foreach (MobStatusTag statusTag in this.statusTags) this.Owner.EquipPoint.Equipper.AddStatusTag(statusTag);
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.EquipPoint.Equipper.AddBehaviour(behaviour, false);
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Request();
            this.entered = true;
            this.enterDuration = 0;

            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Enter();
            }
            else
            {
                ((MobEquipmentState)this).EnterSelf();
            }
        }

        public virtual void Exit() 
        {
            foreach (MobStatusTag statusTag in this.statusTags) this.Owner.EquipPoint.Equipper.RemoveStatusTag(statusTag);
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.EquipPoint.Equipper.RemoveBehaviour(behaviour);
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Release();
            this.entered = false;

            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Exit();
            }
            else
            {
                ((MobEquipmentState)this).ExitSelf();
            }
        }

        public virtual void Refresh() 
        {
            this.enterDurationDelta = Time.deltaTime * this.Owner.EquipPoint.Equipper.RequestedTimeScale;
            this.enterDuration += this.enterDurationDelta;

            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Refresh();
            }
            else
            {
                ((MobEquipmentState)this).RefreshSelf();
            }
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

        protected abstract MobEquipmentStateNode SpawnStateNode
        {
            get;
        }

        protected abstract HashSet<MobEquipmentStateNode> ChildStateNodes
        {
            get;
        }

        protected MobEquipmentStateNode ChosenStateNode
        {
            get
            {
                return this.chosenStateNode;
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

        protected void SetChosenStateNode(MobEquipmentStateNode stateNode)
        {
            Debug.Assert(stateNode == this || this.ChildStateNodes.Contains(stateNode), "Chosen state node must be itself or a child state node!");

            MobEquipmentState previousState;
            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.OnCurrentStateChanged -= this.CurrentStateChanged;

                if (this.Entered) this.chosenStateNode.Exit();
                previousState = this.chosenStateNode.CurrentState;
            }
            else
            {
                Debug.Assert(this is MobEquipmentState, "MobEquipmentStateNode chooses itself yet is not a state!");
                previousState = (MobEquipmentState)this;
                if (this.Entered) previousState.ExitSelf();
            }

            this.chosenStateNode = stateNode;

            MobEquipmentState currentState;
            if (this.chosenStateNode != this)
            {
                if (this.Entered) this.chosenStateNode.Enter();
                currentState = this.chosenStateNode.CurrentState;

                this.chosenStateNode.OnCurrentStateChanged += this.CurrentStateChanged;
            }
            else
            {
                Debug.Assert(this is MobEquipmentState, "MobEquipmentStateNode chooses itself yet is not a state!");
                currentState = (MobEquipmentState)this;
                if (this.Entered) currentState.EnterSelf();
            }

            if (previousState != currentState)
            {
                this.CurrentStateChanged(previousState, currentState);
            }
        }

        private void CurrentStateChanged(MobEquipmentState previousState, MobEquipmentState currentState)
        {
            this.onCurrentStateChanged?.Invoke(previousState, currentState);
        }

        private void HandleEquipperTimeScaleChange()
        {
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources)
            {
                inUseAbilityResource.LocalTimeScale = this.Owner.EquipPoint.Equipper.RequestedTimeScale;
            }
        }
    }
}
