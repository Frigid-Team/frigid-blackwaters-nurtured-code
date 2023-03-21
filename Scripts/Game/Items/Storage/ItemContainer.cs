using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ItemContainer", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.ITEMS + "ItemContainer")]
    public class ItemContainer : FrigidScriptableObject
    {
        [SerializeField]
        private Vector2Int dimensions;

        [Header("Capacity")]
        [SerializeField]
        private float defaultMaxCapacityModifier;
        [SerializeField]
        private int defaultAdditiveCapacity;
        [SerializeField]
        private List<CapacityModifier> capacityModifiers;

        [Header("Interface")]
        [SerializeField]
        private string displayName;
        [SerializeField]
        private Sprite background;
        [SerializeField]
        private Sprite smallIcon;
        [SerializeField]
        private Sprite largeIcon;
        [SerializeField]
        private AudioClip rustleAudioClip;

        public Vector2Int Dimensions
        {
            get
            {
                return this.dimensions;
            }
        }

        public Sprite Background
        {
            get
            {
                return this.background;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }

        public Sprite SmallIcon
        {
            get
            {
                return this.smallIcon;
            }
        }

        public Sprite LargeIcon
        {
            get
            {
                return this.largeIcon;
            }
        }

        public AudioClip RustleAudioClip
        {
            get
            {
                return this.rustleAudioClip;
            }
        }

        public int CalculateMaxCapacityFromStorable(ItemStorable storable)
        {
            float maxCapacityMultiplier = this.defaultMaxCapacityModifier;
            int additiveCapacity = this.defaultAdditiveCapacity;
            foreach (CapacityModifier capacityModifier in this.capacityModifiers)
            {
                if (capacityModifier.Classification == storable.Classification)
                {
                    maxCapacityMultiplier = capacityModifier.MaxCapacityMultiplier;
                    additiveCapacity = capacityModifier.AdditiveCapacity;
                }
            }
            return Mathf.Max(Mathf.FloorToInt(storable.StackSize * maxCapacityMultiplier) + additiveCapacity, 0);
        }

        [Serializable]
        private struct CapacityModifier
        {
            [SerializeField]
            private ItemClassification classification;
            [SerializeField]
            private float maxCapacityMultiplier;
            [SerializeField]
            private int additiveCapacity;

            public ItemClassification Classification
            {
                get
                {
                    return this.classification;
                }
            }

            public float MaxCapacityMultiplier
            {
                get
                {
                    return this.maxCapacityMultiplier;
                }
            }

            public int AdditiveCapacity
            {
                get
                {
                    return this.additiveCapacity;
                }
            }
        }
    }
}
