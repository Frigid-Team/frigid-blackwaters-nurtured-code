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
        private IntSerializedReference consumeMaxPowerIncrease;
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
                return this.Storage.TryGetUsingMob(out _);
            }
        }

        public override bool Used()
        {
            if (this.Storage.PowerBudget.TryUsePower(this, this.consumePowerUsage.ImmutableValue) && this.Storage.PowerBudget.TryIncreaseMaxPower(this, this.consumeMaxPowerIncrease.ImmutableValue))
            {
                if (this.hasUnconsumedEffect) this.DeactivateRootNode(this.unconsumedRootNode);
                if (this.hasConsumedEffect) this.ActivateRootNode(this.consumedRootNode);
                this.OnInEffectChanged += () => this.InUse = this.InEffect;
                this.InUse = this.InEffect;
                return true;
            }
            return false;
        }

        public override void Stored()
        {
            base.Stored();
            if (this.InUse)
            {
                if (this.hasConsumedEffect) this.ActivateRootNode(this.consumedRootNode);
            }
            else
            {
                if (this.hasUnconsumedEffect) this.ActivateRootNode(this.unconsumedRootNode);
            }
        }

        public override void Unstored()
        {
            base.Unstored();
            if (this.InUse)
            {
                if (this.hasConsumedEffect) this.DeactivateRootNode(this.consumedRootNode);
            }
            else
            {
                if (this.hasUnconsumedEffect) this.DeactivateRootNode(this.unconsumedRootNode);
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
    }
}
