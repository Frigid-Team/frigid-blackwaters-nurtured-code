using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobConditionStateNode : MobStateNode
    {
        [SerializeField]
        private List<Option> options;

        private MobStateNode chosenStateNode;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                HashSet<MobState> initialStates = new HashSet<MobState>();
                foreach (Option option in this.options)
                {
                    if (option.StateNode != this) initialStates.UnionWith(option.StateNode.InitialStates);
                }
                return initialStates;
            }
        }

        public override HashSet<MobState> MoveStates
        {
            get
            {
                return this.chosenStateNode.MoveStates;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                HashSet<MobStateNode> referencedStateNodes = new HashSet<MobStateNode>();
                foreach (Option option in this.options)
                {
                    if (option.StateNode != this) referencedStateNodes.Add(option.StateNode);
                }
                return referencedStateNodes;
            }
        }

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
                return this.chosenStateNode.AutoExit;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return true;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.chosenStateNode.ShouldExit;
            }
        }

        public override void Init()
        {
            base.Init();
            foreach (Option option in this.options)
            {
                this.chosenStateNode = option.StateNode;
                if (option.StateNode != this && option.StateNode.InitialStates.Contains(this.CurrentState))
                {
                    break;
                }
            }
        }

        public override void Enter()
        {
            foreach (Option option in this.options)
            {
                if (option.Condition.Evaluate(this.EnterDuration, this.EnterDurationDelta))
                {
                    this.SetChosenStateNode(option.StateNode);
                    break;
                }
            }
            base.Enter();
            this.chosenStateNode.OnCurrentStateChanged += this.SetCurrentStateFromChosenStateNode;
            this.chosenStateNode.Enter();
        }

        public override void Exit()
        {
            base.Exit();
            this.chosenStateNode.Exit();
            this.chosenStateNode.OnCurrentStateChanged -= this.SetCurrentStateFromChosenStateNode;
        }

        public override void Refresh()
        {
            base.Refresh();
            this.chosenStateNode.Refresh();
        }

        private bool CanSetChosenStateNode(MobStateNode chosenStateNode)
        {
            return this.CanSetCurrentState(chosenStateNode.CurrentState);
        }

        private void SetChosenStateNode(MobStateNode chosenStateNode)
        {
            if (this.CanSetChosenStateNode(chosenStateNode))
            {
                if (this.Entered)
                {
                    this.chosenStateNode.Exit();
                    this.chosenStateNode.OnCurrentStateChanged -= this.SetCurrentStateFromChosenStateNode;
                }

                this.chosenStateNode = chosenStateNode;
                this.SetCurrentStateFromChosenStateNode();

                if (this.Entered)
                {
                    this.chosenStateNode.OnCurrentStateChanged += this.SetCurrentStateFromChosenStateNode;
                    this.chosenStateNode.Enter();
                }
            }
        }

        private void SetCurrentStateFromChosenStateNode(MobState previousState, MobState currentState)
        {
            this.SetCurrentStateFromChosenStateNode();
        }

        private void SetCurrentStateFromChosenStateNode()
        {
            this.SetCurrentState(this.chosenStateNode.CurrentState);
        }

        [Serializable]
        private struct Option
        {
            [SerializeField]
            private Conditional condition;
            [SerializeField]
            private MobStateNode stateNode;

            public Conditional Condition
            {
                get
                {
                    return this.condition;
                }
            }

            public MobStateNode StateNode
            {
                get
                {
                    return this.stateNode;
                }
            }
        }
    }
}
