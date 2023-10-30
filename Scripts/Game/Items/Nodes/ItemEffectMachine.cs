using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemEffectMachine : ItemEffectNode
    {
        [SerializeField]
        private List<ItemEffectNode> startingEffectNodes;
        [SerializeField]
        private List<Transition> transitions;
        [SerializeField]
        private ReturnToStartingNodeBehaviour returnToStartingNodeBehaviour;

        public override bool AutoEnter
        {
            get
            {
                return false;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return false;
            }
        }

        public override void Enter()
        {
            if (this.returnToStartingNodeBehaviour == ReturnToStartingNodeBehaviour.OnEnter)
            {
                this.SetChosenEffectNodes(this.startingEffectNodes);
            }
            base.Enter();
            this.CheckTransitions();
        }

        public override void Refresh()
        {
            base.Refresh();
            this.CheckTransitions();
        }

        protected override HashSet<ItemEffectNode> CreationEffectNodes
        {
            get
            {
                return new HashSet<ItemEffectNode>(this.startingEffectNodes);
            }
        }

        protected override HashSet<ItemEffectNode> ChildEffectNodes
        {
            get
            {
                HashSet<ItemEffectNode> referencedItemEffectNodes = new HashSet<ItemEffectNode>(this.startingEffectNodes);
                foreach (Transition transition in this.transitions)
                {
                    referencedItemEffectNodes.Add(transition.FromEffectNode);
                    referencedItemEffectNodes.UnionWith(transition.NextEffectNodes);
                }
                return referencedItemEffectNodes;
            }
        }

        private void CheckTransitions()
        {
            List<Transition> validTransitions = new List<Transition>();
            foreach (Transition transition in this.transitions)
            {
                bool allAutoEnter = true;
                foreach (ItemEffectNode nextEffectNode in transition.NextEffectNodes)
                {
                    allAutoEnter &= nextEffectNode.AutoEnter;
                }
                foreach (ItemEffectNode chosenEffectNode in this.ChosenEffectNodes)
                {
                    if (transition.FromEffectNode == chosenEffectNode &&
                        (transition.FromEffectNode.AutoExit || allAutoEnter || transition.TriggerCondition.Evaluate(this.EnterDuration, this.EnterDurationDelta)))
                    {
                        validTransitions.Add(transition);
                    }
                }
            }

            if (validTransitions.Count > 0)
            {
                Transition chosenTransition = validTransitions[0];
                this.SetChosenEffectNodes(chosenTransition.NextEffectNodes);
            }
        }

        [Serializable]
        private struct Transition
        {
            [SerializeField]
            private Conditional triggerCondition;
            [SerializeField]
            private ItemEffectNode fromEffectNode;
            [SerializeField]
            private List<ItemEffectNode> nextEffectNodes;

            public Conditional TriggerCondition
            {
                get
                {
                    return this.triggerCondition;
                }
            }

            public ItemEffectNode FromEffectNode
            {
                get
                {
                    return this.fromEffectNode;
                }
            }

            public List<ItemEffectNode> NextEffectNodes
            {
                get
                {
                    return this.nextEffectNodes;
                }
            }
        }

        private enum ReturnToStartingNodeBehaviour
        {
            None,
            OnEnter
        }
    }
}
