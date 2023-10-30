using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class Equippable : Item
    {
        [SerializeField]
        private IntSerializedReference equipPowerUsage;
        [SerializeField]
        private IntSerializedReference equipMaxPowerIncrease;
        [SerializeField]
        private bool cannotChangeStorageWhenEquippable;
        [Space]
        [SerializeField]
        private bool hasEquippedEffect;
        [SerializeField]
        [ShowIfBool("hasEquippedEffect", true)]
        private ItemEffectNode equippedRootEffectNode;
        [SerializeField]
        private bool hasUnequippedEffect;
        [SerializeField]
        [ShowIfBool("hasUnequippedEffect", true)]
        private ItemEffectNode unequippedRootEffectNode;

        public override bool IsUsable
        {
            get
            {
                return this.Storage.TryGetUsingMob(out _);
            }
        }

        public override bool Used()
        {
            if (!this.InUse)
            {
                if (this.Storage.PowerBudget.TryUsePower(this, this.equipPowerUsage.ImmutableValue) && this.Storage.PowerBudget.TryIncreaseMaxPower(this, this.equipMaxPowerIncrease.ImmutableValue))
                {
                    this.InUse = true;
                    this.StorageChangeable = false;
                    if (this.hasEquippedEffect) this.AddRootEffectNode(this.equippedRootEffectNode);
                    if (this.hasUnequippedEffect) this.RemoveRootEffectNode(this.unequippedRootEffectNode);
                }
            }
            else
            {
                if (this.Storage.PowerBudget.TryReleasePower(this) && this.Storage.PowerBudget.TryDecreaseMaxPower(this))
                {
                    this.InUse = false;
                    if (this.hasUnequippedEffect) this.AddRootEffectNode(this.unequippedRootEffectNode);
                    if (this.hasEquippedEffect) this.RemoveRootEffectNode(this.equippedRootEffectNode);
                }
            }
            this.StorageChangeable = !this.InUse && !this.cannotChangeStorageWhenEquippable;
            return false;
        }

        public override void Created()
        {
            base.Created();
            this.InUse = false;
            this.StorageChangeable = true;
        }

        public override void Stored()
        {
            base.Stored();
            this.StorageChangeable = !this.IsUsable || !this.cannotChangeStorageWhenEquippable;
        }

        public override void Unstored()
        {
            base.Unstored();
            if (this.InUse)
            {
                if (this.hasUnequippedEffect) this.AddRootEffectNode(this.unequippedRootEffectNode);
                if (this.hasEquippedEffect) this.RemoveRootEffectNode(this.equippedRootEffectNode);
                this.InUse = false;
            }
            this.StorageChangeable = true;
        }

        protected override HashSet<ItemEffectNode> InitialRootEffectNodes
        {
            get
            {
                if (this.hasUnequippedEffect) return new HashSet<ItemEffectNode>() { this.unequippedRootEffectNode };
                return new HashSet<ItemEffectNode>();
            }
        }

        protected override HashSet<ItemEffectNode> ReferencedRootEffectNodes
        {
            get
            {
                HashSet<ItemEffectNode> referencedRootEffectNodes = new HashSet<ItemEffectNode>();
                if (this.hasEquippedEffect) referencedRootEffectNodes.Add(this.equippedRootEffectNode);
                if (this.hasUnequippedEffect) referencedRootEffectNodes.Add(this.unequippedRootEffectNode);
                return referencedRootEffectNodes;
            }
        }
    }
}
