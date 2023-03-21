using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class Consumable : Item
    {
        [SerializeField]
        private IntSerializedReference consumePowerUsage;
        [SerializeField]
        private bool hasConsumedEffect;
        [SerializeField]
        [ShowIfBool("hasConsumedEffect", true)]
        private ItemNode consumedRootNode;
        [SerializeField]
        private bool hasUnconsumedEffect;
        [SerializeField]
        [ShowIfBool("hasUnconsumedEffect", true)]
        private ItemNode unconsumedRootNode;

        public override bool IsUsable
        {
            get
            {
                return true;
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
                HashSet<ItemNode> rootNodes = new HashSet<ItemNode>();
                if (this.hasConsumedEffect) rootNodes.Add(this.consumedRootNode);
                if (this.hasUnconsumedEffect) rootNodes.Add(this.unconsumedRootNode);
                return rootNodes;
            }
        }

        public override bool Used()
        {
            if (this.Storage.PowerBudget.TryUsePower(this, this.consumePowerUsage.ImmutableValue))
            {
                if (this.hasUnconsumedEffect) DeactivateRootNode(this.unconsumedRootNode);
                if (this.hasConsumedEffect) ActivateRootNode(this.consumedRootNode);
                return true;
            }
            return false;
        }

        public override void Stored()
        {
            base.Stored();
            if (this.hasUnconsumedEffect) ActivateRootNode(this.unconsumedRootNode);
        }

        public override void Unstored()
        {
            base.Unstored();
            if (this.hasUnconsumedEffect) DeactivateRootNode(this.unconsumedRootNode);
        }
    }
}
