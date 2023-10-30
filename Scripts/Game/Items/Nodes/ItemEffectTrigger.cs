using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemEffectTrigger : ItemEffectNode
    {
        [SerializeField]
        private List<Option> options;

        private List<bool> areEffectsTriggered; 

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

        public override void Created()
        {
            base.Created();
            this.areEffectsTriggered = new List<bool>();
            for (int i = 0; i < this.options.Count; i++)
            {
                this.areEffectsTriggered.Add(false);
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
            this.CheckConditions();
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
            for (int i = 0; i < this.options.Count; i++)
            {
                Option option = this.options[i];
                bool isEffectTriggered = this.areEffectsTriggered[i];

                if (!isEffectTriggered && option.Condition.Evaluate(this.EnterDuration, this.EnterDurationDelta))
                {
                    this.areEffectsTriggered[i] = true;
                    this.AddChosenEffectNodes(option.EffectNodes);
                }
                if (isEffectTriggered && option.EffectNodes.All((ItemEffectNode effectNode) => !effectNode.IsGrantingEffect))
                {
                    this.areEffectsTriggered[i] = false;
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
    }
}
