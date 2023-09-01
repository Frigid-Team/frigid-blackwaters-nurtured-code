using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ItemNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private bool inEffect;
        [SerializeField]
        private List<AbilityResource> activeAbilityResources;
        [Space]
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<ChildBranch> childBranches;

        private Item owner;

        private float enterDuration;
        private float enterDurationDelta;
        private HashSet<ChildBranch> activeChildBranches;
        private HashSet<ItemNode> currentNodes;
        private Action<ItemNode> onCurrentNodeAdded;
        private Action<ItemNode> onCurrentNodeRemoved;

        public bool InEffect
        {
            get
            {
                return this.inEffect;
            }
        }

        public List<AbilityResource> ActiveAbilityResources
        {
            get
            {
                return this.activeAbilityResources;
            }
        }

        public HashSet<ItemNode> CurrentNodes
        {
            get
            {
                return this.currentNodes;
            }
        }

        public Action<ItemNode> OnCurrentNodeAdded
        {
            get
            {
                return this.onCurrentNodeAdded;
            }
            set
            {
                this.onCurrentNodeAdded = value;
            }
        }

        public Action<ItemNode> OnCurrentNodeRemoved
        {
            get
            {
                return this.onCurrentNodeRemoved;
            }
            set
            {
                this.onCurrentNodeRemoved = value;
            }
        }

        public HashSet<ItemNode> ReferencedNodes
        {
            get
            {
                HashSet<ItemNode> referencedItemNodes = new HashSet<ItemNode>();
                foreach (ChildBranch childBranch in this.childBranches) referencedItemNodes.Add(childBranch.BranchNode);
                return referencedItemNodes;
            }
        }

        public void Link(Item owner)
        {
            this.owner = owner;
        }

        public virtual void Init()
        {
            this.activeChildBranches = new HashSet<ChildBranch>();
            this.currentNodes = new HashSet<ItemNode>() { this };
        }

        public virtual void Enter()
        {
            this.enterDuration = 0;
            this.enterDurationDelta = 0;

            foreach (ChildBranch activeChildBranch in this.activeChildBranches)
            {
                activeChildBranch.BranchNode.Enter();
            }

            foreach (MobBehaviour mobBehaviour in this.behaviours)
            {
                this.owner.AddBehaviourToMob(mobBehaviour);
            }

            this.UpdateCurrentChildBranches();
        }

        public virtual void Exit()
        {
            foreach (ChildBranch activeChildBranch in this.activeChildBranches)
            {
                activeChildBranch.BranchNode.Exit();
            }

            foreach (MobBehaviour mobBehaviour in this.behaviours)
            {
                this.owner.RemoveBehaviourFromMob(mobBehaviour);
            }
        }

        public virtual void Refresh()
        {
            this.enterDurationDelta = Time.deltaTime;
            this.enterDuration += this.enterDurationDelta;

            this.UpdateCurrentChildBranches();

            foreach (ChildBranch activeChildBranch in this.activeChildBranches)
            {
                activeChildBranch.BranchNode.Refresh();
            }
        }

        protected Item Owner
        {
            get
            {
                return this.owner;
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

        private void UpdateCurrentChildBranches()
        {
            bool stopEvaluating = false;
            foreach (ChildBranch childBranch in this.childBranches)
            {
                if (this.activeChildBranches.Contains(childBranch))
                {
                    if (stopEvaluating || !childBranch.CanBranch(this))
                    {
                        this.activeChildBranches.Remove(childBranch);
                        childBranch.BranchNode.Exit();
                        childBranch.BranchExited();
                        childBranch.BranchNode.onCurrentNodeAdded -= this.AddCurrentNode;
                        childBranch.BranchNode.onCurrentNodeRemoved -= this.RemoveCurrentNode;
                        foreach (ItemNode currentNode in childBranch.BranchNode.CurrentNodes)
                        {
                            this.RemoveCurrentNode(currentNode);
                        }
                    }
                    else
                    {
                        stopEvaluating |= childBranch.StopEvaluatingFollowingBranches;
                    }
                }
                else
                {
                    if (!stopEvaluating && childBranch.CanBranch(this))
                    {
                        this.activeChildBranches.Add(childBranch);
                        childBranch.BranchNode.onCurrentNodeAdded += this.AddCurrentNode;
                        childBranch.BranchNode.onCurrentNodeRemoved += this.RemoveCurrentNode;
                        foreach (ItemNode currentNode in childBranch.BranchNode.CurrentNodes)
                        {
                            this.AddCurrentNode(currentNode);
                        }
                        childBranch.BranchEntered();
                        childBranch.BranchNode.Enter();
                        stopEvaluating |= childBranch.StopEvaluatingFollowingBranches;
                    }
                }
            }
        }

        private void AddCurrentNode(ItemNode currentNode)
        {
            if (this.currentNodes.Add(currentNode))
            {
                this.onCurrentNodeAdded?.Invoke(currentNode);
            }
        }

        private void RemoveCurrentNode(ItemNode currentNode)
        {
            if (this.currentNodes.Remove(currentNode))
            {
                this.onCurrentNodeRemoved?.Invoke(currentNode);
            }
        }

        [Serializable]
        private class ChildBranch
        {
            [SerializeField]
            private ItemNode branchNode;
            [SerializeField]
            private bool stopEvaluatingFollowingBranches;
            [SerializeField]
            private List<AbilityResource> inUseAbilityResources;
            [SerializeField]
            private Conditional branchCondition;

            public ItemNode BranchNode
            {
                get
                {
                    return this.branchNode;
                }
            }

            public bool StopEvaluatingFollowingBranches
            {
                get
                {
                    return this.stopEvaluatingFollowingBranches;
                }
            }

            public void BranchEntered() 
            { 
                foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources)
                {
                    inUseAbilityResource.InUse.Request();
                }
            }

            public void BranchExited() 
            {
                foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources)
                {
                    inUseAbilityResource.InUse.Release();
                }
            }

            public virtual bool CanBranch(ItemNode node)
            {
                return this.branchCondition.Evaluate(node.EnterDuration, node.EnterDurationDelta);
            }
        }
    }
}