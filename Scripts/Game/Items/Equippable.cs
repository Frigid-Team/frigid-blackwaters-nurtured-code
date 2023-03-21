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
        private bool hasEquippedEffect;
        [SerializeField]
        [ShowIfBool("hasEquippedEffect", true)]
        private ItemNode equippedRootNode;
        [SerializeField]
        private bool hasUnequippedEffect;
        [SerializeField]
        [ShowIfBool("hasUnequippedEffect", true)]
        private ItemNode unequippedRootNode;

        private bool isEquipped;

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
                return this.isEquipped;
            }
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

        public override bool Used()
        {
            if (!this.isEquipped)
            {
                if (this.Storage.PowerBudget.TryUsePower(this, this.equipPowerUsage.ImmutableValue))
                {
                    this.isEquipped = true;
                    if (this.hasEquippedEffect) ActivateRootNode(this.equippedRootNode);
                    if (this.hasUnequippedEffect) DeactivateRootNode(this.unequippedRootNode);
                }
            }
            else
            {
                if (this.Storage.PowerBudget.TryReleasePower(this))
                {
                    this.isEquipped = false;
                    if (this.hasUnequippedEffect) ActivateRootNode(this.unequippedRootNode);
                    if (this.hasEquippedEffect) DeactivateRootNode(this.equippedRootNode);
                }
            }
            return false;
        }

        public override void Stored()
        {
            base.Stored();
            this.isEquipped = false;
            if (this.hasUnequippedEffect) ActivateRootNode(this.unequippedRootNode);
        }

        public override void Unstored()
        {
            base.Unstored();
            if (this.isEquipped)
            {
                if (this.Storage.PowerBudget.TryReleasePower(this))
                {
                    if (this.hasEquippedEffect) DeactivateRootNode(this.equippedRootNode);
                }
            }
            else
            {
                if (this.hasUnequippedEffect) DeactivateRootNode(this.unequippedRootNode);
            }
            this.isEquipped = false;
        }

        protected override void Awake()
        {
            base.Awake();
            this.isEquipped = false;
        }
    }
}
