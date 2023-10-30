using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemEffectSeparator : ItemEffectNode
    {
        [SerializeField]
        private List<ItemEffectNode> effectNodes;

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

        protected override HashSet<ItemEffectNode> CreationEffectNodes
        {
            get
            {
                return this.ChildEffectNodes;
            }
        }

        protected override HashSet<ItemEffectNode> ChildEffectNodes
        {
            get
            {
                return new HashSet<ItemEffectNode>(this.effectNodes);
            }
        }
    }
}
