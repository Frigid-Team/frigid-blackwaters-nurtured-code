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
        private ItemNode passiveRootItemNode;

        public override bool IsUsable
        {
            get
            {
                return false;
            }
        }

        public override bool IsInEffect
        {
            get
            {
                return false;
            }
        }

        protected override HashSet<ItemNode> RootNodes
        {
            get
            {
                if (this.hasPassiveEffect) return new HashSet<ItemNode> { this.passiveRootItemNode };
                return new HashSet<ItemNode>();
            }
        }

        public override void Stored()
        {
            base.Stored();
            if (this.hasPassiveEffect) ActivateRootNode(this.passiveRootItemNode);
        }

        public override void Unstored()
        {
            base.Unstored();
            if (this.hasPassiveEffect) DeactivateRootNode(this.passiveRootItemNode);
        }
    }
}
