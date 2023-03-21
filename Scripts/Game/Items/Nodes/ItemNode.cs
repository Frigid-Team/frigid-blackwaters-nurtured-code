using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ItemNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<ChildBranch> baseChildBranches;

        private Item parent;

        private float enterDuration;
        private float enterDurationDelta;
        private HashSet<ChildBranch> currentChildBranches;

        public HashSet<ItemNode> ReferencedNodes
        {
            get
            {
                HashSet<ItemNode> referencedItemNodes = new HashSet<ItemNode>();
                foreach (ChildBranch baseChildBranch in this.baseChildBranches) referencedItemNodes.Add(baseChildBranch.BranchNode);
                foreach (ChildBranch additionalChildBranch in this.AdditionalChildBranches) referencedItemNodes.Add(additionalChildBranch.BranchNode);
                return referencedItemNodes;
            }
        }

        public void Link(Item parent)
        {
            this.parent = parent;
        }

        public virtual void Init()
        {
            this.currentChildBranches = new HashSet<ChildBranch>();
        }

        public virtual void Enter()
        {
            this.enterDuration = 0;
            this.enterDurationDelta = 0;

            this.currentChildBranches.Clear();
            foreach (ChildBranch childBranch in this.AllChildBranches)
            {
                if (childBranch.CanBranch(this))
                {
                    this.currentChildBranches.Add(childBranch);
                    childBranch.BranchNode.Enter();
                    childBranch.BranchEntered(this);
                    if (childBranch.StopEvaluatingFollowingBranches)
                    {
                        break;
                    }
                }
            }

            foreach (MobBehaviour mobBehaviour in this.behaviours)
            {
                this.parent.AddBehaviourToMob(mobBehaviour);
            }
        }

        public virtual void Exit()
        {
            foreach (ChildBranch currentChildBranch in this.currentChildBranches)
            {
                currentChildBranch.BranchExited(this);
                currentChildBranch.BranchNode.Exit();
            }

            foreach (MobBehaviour mobBehaviour in this.behaviours)
            {
                this.parent.RemoveBehaviourFromMob(mobBehaviour);
            }
        }

        public virtual void Refresh()
        {
            this.enterDurationDelta = Time.deltaTime;
            this.enterDuration += this.enterDurationDelta;

            UpdateCurrentChildBranches();

            foreach (ChildBranch currentChildBranch in this.currentChildBranches)
            {
                currentChildBranch.BranchNode.Refresh();
            }
        }

        protected virtual List<ChildBranch> AdditionalChildBranches
        {
            get 
            {
                return new List<ChildBranch>();
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

        protected void UpdateCurrentChildBranches()
        {
            foreach (ChildBranch childBranch in this.AllChildBranches)
            {
                if (this.currentChildBranches.Contains(childBranch))
                {
                    if (!childBranch.CanBranch(this))
                    {
                        this.currentChildBranches.Remove(childBranch);
                        childBranch.BranchExited(this);
                        childBranch.BranchNode.Exit();
                    }
                    else if (childBranch.StopEvaluatingFollowingBranches)
                    {
                        break;
                    }
                }
                else
                {
                    if (childBranch.CanBranch(this))
                    {
                        this.currentChildBranches.Add(childBranch);
                        childBranch.BranchNode.Enter();
                        childBranch.BranchEntered(this);
                        if (childBranch.StopEvaluatingFollowingBranches)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private List<ChildBranch> AllChildBranches
        {
            get
            {
                List<ChildBranch> allChildBranches = new List<ChildBranch>();
                allChildBranches.AddRange(this.AdditionalChildBranches);
                allChildBranches.AddRange(this.baseChildBranches);
                return allChildBranches;
            }
        }

        [Serializable]
        protected class ChildBranch
        {
            [SerializeField]
            private bool stopEvaluatingFollowingBranches;
            [SerializeField]
            private ConditionalClause branchConditions;
            [SerializeField]
            private ItemNode branchNode;

            public bool StopEvaluatingFollowingBranches
            {
                get
                {
                    return this.stopEvaluatingFollowingBranches;
                }
            }

            public ItemNode BranchNode
            {
                get
                {
                    return this.branchNode;
                }
            }

            public virtual void BranchEntered(ItemNode node) { }

            public virtual void BranchExited(ItemNode node) { }

            public virtual bool CanBranch(ItemNode node)
            {
                return this.branchConditions.Evaluate(node.EnterDuration, node.EnterDurationDelta);
            }
        }
    }
}