using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace FrigidBlackwaters.Game
{
    public abstract class ItemEffectNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobStatusTag> statusTags;
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<AbilityResource> inUseAbilityResources;

        private Item owner;

        private int numberBehavioursFinished;

        private bool entered;
        private float enterDuration;
        private float enterDurationDelta;

        private bool enteredSelf;
        private float selfEnterDuration;
        private float selfEnterDurationDelta;

        private HashSet<ItemEffectNode> chosenEffectNodes;

        public bool IsGrantingEffect
        {
            get
            {
                return this.Entered && this.numberBehavioursFinished < this.behaviours.Count;
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

        public void OwnedBy(Item owner)
        {
            this.owner = owner;

            foreach (ItemEffectNode childEffectNode in this.ChildEffectNodes)
            {
                childEffectNode.OwnedBy(owner);
            }
        }

        public void CollectAbilityResources(ref List<AbilityResource> abilityResources)
        {
            abilityResources.AddRange(this.inUseAbilityResources);
            foreach (ItemEffectNode childEffectNode in this.ChildEffectNodes)
            {
                childEffectNode.CollectAbilityResources(ref abilityResources);
            }
            abilityResources = abilityResources.Distinct().ToList();
        }

        public virtual void Created()
        {
            this.chosenEffectNodes = new HashSet<ItemEffectNode>();
            // Optimization - the maximum number of chosen effect nodes is the number of children.
            this.chosenEffectNodes.EnsureCapacity(this.ChildEffectNodes.Count);

            foreach (ItemEffectNode childEffectNode in this.ChildEffectNodes)
            {
                childEffectNode.Created();
            }

            this.AddChosenEffectNodes(this.CreationEffectNodes);
        }

        public virtual void Discarded() 
        { 
            foreach (ItemEffectNode childEffectNode in this.ChildEffectNodes)
            {
                childEffectNode.Discarded();
            }
        }

        public virtual void Enter()
        {
            this.entered = true;

            foreach (MobStatusTag statusTag in this.statusTags) this.Owner.AddStatusTagToMob(statusTag);

            this.numberBehavioursFinished = 0;
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.AddEffectBehaviour(behaviour, () => this.numberBehavioursFinished++);

            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Request();

            this.enterDuration = 0;
            this.enterDurationDelta = 0;

            
            foreach (ItemEffectNode chosenEffectNode in this.chosenEffectNodes)
            {
                if (chosenEffectNode != this)
                {
                    chosenEffectNode.Enter();
                }
                else
                {
                    this.EnterSelf();
                }
            }
        }

        public virtual void EnterSelf()
        {
            this.enteredSelf = true;
            this.selfEnterDuration = 0f;
            this.selfEnterDurationDelta = 0f;
        }

        public virtual void Exit()
        {
            this.entered = false;

            foreach (MobStatusTag statusTag in this.statusTags) this.Owner.RemoveStatusTagFromMob(statusTag);

            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.RemoveEffectBehaviour(behaviour);

            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Release();

            foreach (ItemEffectNode chosenEffectNode in this.chosenEffectNodes)
            {
                if (chosenEffectNode != this)
                {
                    chosenEffectNode.Exit();
                }
                else
                {
                    this.ExitSelf();
                }
            }
        }

        public virtual void ExitSelf()
        {
            this.enteredSelf = false;
        }

        public virtual void Refresh()
        {
            this.enterDurationDelta = Time.deltaTime;
            this.enterDuration += this.enterDurationDelta;

            foreach (ItemEffectNode chosenEffectNode in this.chosenEffectNodes)
            {
                if (chosenEffectNode != this)
                {
                    chosenEffectNode.Refresh();
                }
                else
                {
                    this.RefreshSelf();
                }
            }
        }

        public virtual void RefreshSelf()
        {
            this.selfEnterDurationDelta = Time.deltaTime;
            this.selfEnterDuration += this.selfEnterDurationDelta;
        }

        protected Item Owner
        {
            get
            {
                return this.owner;
            }
        }

        protected abstract HashSet<ItemEffectNode> CreationEffectNodes
        {
            get;
        }

        protected abstract HashSet<ItemEffectNode> ChildEffectNodes
        {
            get;
        }

        protected HashSet<ItemEffectNode> ChosenEffectNodes
        {
            get
            {
                return this.chosenEffectNodes;
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

        protected void SetChosenEffectNodes(IEnumerable<ItemEffectNode> effectNodes)
        {
            ItemEffectNode[] previousEffectNodes = this.chosenEffectNodes.ToArray();
            ItemEffectNode[] newEffectNodes = effectNodes.ToArray();
            this.RemoveChosenEffectNodes(previousEffectNodes);
            this.AddChosenEffectNodes(newEffectNodes);
        }

        protected void AddChosenEffectNodes(IEnumerable<ItemEffectNode> effectNodes)
        {
            foreach (ItemEffectNode effectNode in effectNodes)
            {
                Debug.Assert(effectNode == this || this.ChildEffectNodes.Contains(effectNode), "Chosen effect node must be itself or a child effect node!");

                if (!this.chosenEffectNodes.Add(effectNode))
                {
                    continue;
                }

                if (this.Entered)
                {
                    if (effectNode != this)
                    {
                        effectNode.Enter();
                        continue;
                    }
                    this.EnterSelf();
                }
            }
        }

        protected void RemoveChosenEffectNodes(IEnumerable<ItemEffectNode> effectNodes)
        {
            foreach (ItemEffectNode effectNode in effectNodes)
            {
                Debug.Assert(effectNode == this || this.ChildEffectNodes.Contains(effectNode), "Chosen effect node must be itself or a child effect node!");

                if (!this.chosenEffectNodes.Remove(effectNode))
                {
                    continue;
                }

                if (this.Entered)
                {
                    if (effectNode != this)
                    {
                        effectNode.Exit();
                        continue;
                    }
                    this.ExitSelf();
                }
            }
        }
    }
}
