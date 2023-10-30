using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class Valuable : Item
    {
        [SerializeField]
        private bool hasPassiveEffect;
        [SerializeField]
        [ShowIfBool("hasPassiveEffect", true)]
        private ItemEffectNode passiveRootItemEffectNode;

        public override bool IsUsable
        {
            get
            {
                return false;
            }
        }

        public override void Created()
        {
            base.Created();
            this.InUse = false;
            this.StorageChangeable = true;
        }

        protected override HashSet<ItemEffectNode> InitialRootEffectNodes
        {
            get
            {
                if (this.hasPassiveEffect) return new HashSet<ItemEffectNode>() { this.passiveRootItemEffectNode };
                return new HashSet<ItemEffectNode>();
            }
        }

        protected override HashSet<ItemEffectNode> ReferencedRootEffectNodes
        {
            get
            {
                if (this.hasPassiveEffect) return new HashSet<ItemEffectNode>() { this.passiveRootItemEffectNode };
                return new HashSet<ItemEffectNode>();
            }
        }
    }
}
