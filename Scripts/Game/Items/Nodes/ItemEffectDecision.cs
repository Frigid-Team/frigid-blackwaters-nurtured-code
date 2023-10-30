using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemEffectDecision : ItemEffectNode
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
                bool allAutoExit = true;
                foreach (ItemEffectNode chosenEffectNode in this.ChosenEffectNodes)
                {
                    allAutoExit &= chosenEffectNode.AutoExit;
                }
                return allAutoExit;
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

        protected override HashSet<ItemEffectNode> CreationEffectNodes
        {
            get
            {
                return new HashSet<ItemEffectNode>();
            }
        }

        protected override HashSet<ItemEffectNode> ChildEffectNodes
        {
            get
            {
                HashSet<ItemEffectNode> childEffectNodes = new HashSet<ItemEffectNode>();
                foreach (Option option in this.options)
                {
                    childEffectNodes.UnionWith(option.EffectNodes);
                }
                return childEffectNodes;
            }
        }

        private void CheckConditions()
        {
            foreach (Option option in this.options)
            {
                if (option.Condition.Evaluate(this.EnterDuration, this.EnterDurationDelta))
                {
                    this.AddChosenEffectNodes(option.EffectNodes);
                }
                else
                {
                    this.RemoveChosenEffectNodes(option.EffectNodes);
                }
            }
        }

        [Serializable]
        private struct Option
        {
            [SerializeField]
            private Conditional condition;
            [SerializeField]
            private List<ItemEffectNode> effectNodes;

            public Conditional Condition
            {
                get
                {
                    return this.condition;
                }
            }

            public List<ItemEffectNode> EffectNodes
            {
                get
                {
                    return this.effectNodes;
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
