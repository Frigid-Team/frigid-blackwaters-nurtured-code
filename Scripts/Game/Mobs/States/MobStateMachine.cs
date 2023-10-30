using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobStateMachine : MobStateNode
    {
        [SerializeField]
        private List<MobStateNode> startingStateNodes;
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

        public override bool ShouldEnter
        {
            get
            {
                return this.ChosenStateNode.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.ChosenStateNode.ShouldExit;
            }
        }

        public override void Enter()
        {
            if (this.returnToStartingNodeBehaviour == ReturnToStartingNodeBehaviour.OnEnter)
            {
                bool foundStartingNode = false;
                foreach (MobStateNode startingStateNode in this.startingStateNodes)
                {
                    if (this.CanSetChosenStateNode(startingStateNode))
                    {
                        foundStartingNode = true;
                        this.SetChosenStateNode(startingStateNode);
                        break;
                    }
                }
                if (!foundStartingNode)
                {
                    Debug.LogWarning("MobStateMachine " + this.name + " could not find a starting node to return to.");
                }
            }
            base.Enter();
            this.CheckTransitions();
        }

        public override void Refresh()
        {
            base.Refresh();
            this.CheckTransitions();
        }

        protected override HashSet<MobStateNode> SpawnStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>(this.startingStateNodes);
            }
        }

        protected override HashSet<MobStateNode> MoveStateNodes
        {
            get
            {
                return new HashSet<MobStateNode> { this.ChosenStateNode };
            }
        }

        protected override HashSet<MobStateNode> ChildStateNodes
        {
            get
            {
                HashSet<MobStateNode> childStateNodes = new HashSet<MobStateNode>(this.startingStateNodes);
                foreach (Transition transition in this.transitions)
                {
                    childStateNodes.Add(transition.FromStateNode);
                    childStateNodes.Add(transition.NextStateNode);
                }
                return childStateNodes;
            }
        }

        private void CheckTransitions()
        {
            if (!this.Owner.IsActingAndNotStunned) return;
            if (this.ChosenStateNode.ShouldExit)
            {
                List<Transition> validTransitions = new List<Transition>();
                foreach (Transition transition in this.transitions)
                {
                    if (transition.FromStateNode == this.ChosenStateNode &&
                        transition.NextStateNode.ShouldEnter &&
                        (transition.FromStateNode.AutoExit || transition.NextStateNode.AutoEnter || transition.TriggerCondition.Evaluate(this.EnterDuration, this.EnterDurationDelta)))
                    {
                        validTransitions.Add(transition);
                    }
                }

                foreach (Transition validTransition in validTransitions)
                {
                    if (!this.CanSetChosenStateNode(validTransition.NextStateNode))
                    {
                        continue;
                    }
                    this.SetChosenStateNode(validTransition.NextStateNode);
                    break;
                }
            }
        }

        [Serializable]
        private struct Transition
        {
            [SerializeField]
            private Conditional triggerCondition;
            [SerializeField]
            private MobStateNode fromStateNode;
            [SerializeField]
            private MobStateNode nextStateNode;

            public Conditional TriggerCondition
            {
                get
                {
                    return this.triggerCondition;
                }
            }

            public MobStateNode FromStateNode
            {
                get
                {
                    return this.fromStateNode;
                }
            }

            public MobStateNode NextStateNode
            {
                get
                {
                    return this.nextStateNode;
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
