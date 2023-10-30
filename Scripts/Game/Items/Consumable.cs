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
        private ItemEffectNode consumedRootEffectNode;
        [SerializeField]
        private bool hasUnconsumedEffect;
        [SerializeField]
        [ShowIfBool("hasUnconsumedEffect", true)]
        private ItemEffectNode unconsumedRootEffectNode;

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
                if (this.hasUnconsumedEffect) this.RemoveRootEffectNode(this.unconsumedRootEffectNode);
                if (this.hasConsumedEffect) this.AddRootEffectNode(this.consumedRootEffectNode);
                this.OnInEffectChanged += 
                    () =>
                    {
                        this.InUse = this.InEffect;
                        this.StorageChangeable = !this.InEffect;
                    };
                this.InUse = this.InEffect;
                this.StorageChangeable = !this.InEffect;
                return true;
            }
            return false;
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
                if (this.hasUnconsumedEffect) return new HashSet<ItemEffectNode>() { this.unconsumedRootEffectNode };
                return new HashSet<ItemEffectNode>();
            }
        }

        protected override HashSet<ItemEffectNode> ReferencedRootEffectNodes
        {
            get
            {
                HashSet<ItemEffectNode> referencedRootEffectNodes = new HashSet<ItemEffectNode>();
                if (this.hasConsumedEffect) referencedRootEffectNodes.Add(this.consumedRootEffectNode);
                if (this.hasUnconsumedEffect) referencedRootEffectNodes.Add(this.unconsumedRootEffectNode);
                return referencedRootEffectNodes;
            }
        }
    }
}
