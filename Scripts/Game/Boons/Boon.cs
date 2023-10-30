using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class Boon : FrigidScriptableObject
    {
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private string displayName;
        [SerializeField]
        [TextArea]
        private string description;
        [SerializeField]
        private int stampCost;
        [SerializeField]
        [Tooltip("Any number less than 0 counts as infinite.")]
        private IntSerializedReference maxLoadoutQuantity;

        public int MaxLoadoutQuantity
        {
            get
            {
                return this.maxLoadoutQuantity.ImmutableValue;
            }
        }

        public Sprite Icon
        {
            get
            {
                return this.icon;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public int StampCost
        {
            get
            {
                return this.stampCost;
            }
        }

        public virtual void ActivateQuantity(int quantity) { }

        public virtual void DeactivateQuantity(int quantity) { }
    }
}