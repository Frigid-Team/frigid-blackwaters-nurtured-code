using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobStateDecision : MobStateNode
    {
        [SerializeField]
        private List<Option> options;
        [SerializeField]
        private CheckConditionFrequency checkConditionFrequency;

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
                return this.ChosenStateNode.AutoExit;
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
                return this.ChosenStateNode.ShouldExit;
            }
        }

        public override void Enter()
        {
            base.Enter();
            this.CheckConditions();
        }

        public override void Refresh()
        {
            base.Refresh();
            if (this.checkConditionFrequency == CheckConditionFrequency.OnEnterAndRefresh)
            {
                this.CheckConditions();
            }
        }

        protected override HashSet<MobStateNode> SpawnStateNodes
        {
            get
            {
                return this.ChildStateNodes;
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
                HashSet<MobStateNode> childStateNodes = new HashSet<MobStateNode>();
                foreach (Option option in this.options)
                {
                    childStateNodes.Add(option.StateNode);
                }
                return childStateNodes;
            }
        }

        private void CheckConditions()
        {
            bool foundOption = false;
            foreach (Option option in this.options)
            {
                if (option.Condition.Evaluate(this.EnterDuration, this.EnterDurationDelta) && this.CanSetChosenStateNode(option.StateNode))
                {
                    foundOption = true;
                    this.SetChosenStateNode(option.StateNode);
                    break;
                }
            }
            if (!foundOption)
            {
                Debug.LogWarning("MobConditionStateNode " + this.name + " could not find a node to enter.");
            }
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

        private enum CheckConditionFrequency
        {
            OnEnter,
            OnEnterAndRefresh
        }
    }
}
