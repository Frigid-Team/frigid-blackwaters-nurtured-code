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
        private ItemNode equippedRootNode;
        [SerializeField]
        private bool hasUnequippedEffect;
        [SerializeField]
        [ShowIfBool("hasUnequippedEffect", true)]
        private ItemNode unequippedRootNode;

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
                    if (this.hasEquippedEffect) this.ActivateRootNode(this.equippedRootNode);
                    if (this.hasUnequippedEffect) this.DeactivateRootNode(this.unequippedRootNode);
                }
            }
            else
            {
                if (this.Storage.PowerBudget.TryReleasePower(this) && this.Storage.PowerBudget.TryDecreaseMaxPower(this))
                {
                    this.InUse = false;
                    if (this.hasUnequippedEffect) this.ActivateRootNode(this.unequippedRootNode);
                    if (this.hasEquippedEffect) this.DeactivateRootNode(this.equippedRootNode);
                }
            }
            this.StorageChangeable = !this.InUse && !this.cannotChangeStorageWhenEquippable;
            return false;
        }

        public override void Stored()
        {
            base.Stored();
            this.InUse = false;
            this.StorageChangeable = !this.IsUsable || !this.cannotChangeStorageWhenEquippable;
            if (this.hasUnequippedEffect) this.ActivateRootNode(this.unequippedRootNode);
        }

        public override void Unstored()
        {
            base.Unstored();
            if (this.hasUnequippedEffect) this.DeactivateRootNode(this.unequippedRootNode);
            this.InUse = false;
            this.StorageChangeable = true;
        }

        protected override HashSet<ItemNode> RootNodes
        {
            get
            {
                HashSet<ItemNode> rootNodes = new HashSet<ItemNode>();
                if (this.hasEquippedEffect) rootNodes.Add(this.equippedRootNode);
                if (this.hasUnequippedEffect) rootNodes.Add(this.unequippedRootNode);
                return rootNodes;
            }
        }
    }
}
