using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class ItemEffectLeaf : ItemEffectNode
    {
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
                return new HashSet<ItemEffectNode>() { this };
            }
        }

        protected override HashSet<ItemEffectNode> ChildEffectNodes
        {
            get
            {
                return new HashSet<ItemEffectNode>();
            }
        }
    }
}
