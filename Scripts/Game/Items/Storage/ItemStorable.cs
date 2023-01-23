using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ItemStorable", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.ITEMS + "ItemStorable")]
    public class ItemStorable : FrigidScriptableObject
    {
        [SerializeField]
        private Item itemPrefab;
        [SerializeField]
        private ItemClassification classification;
        [SerializeField]
        private bool ignoreBuyCostModifiers;
        [SerializeField]
        private IntSerializedReference currencyValue;
        [SerializeField]
        private IntSerializedReference stackSize;

        [Header("Display")]
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private string displayName;
        [SerializeField]
        [TextArea]
        private string powerDescription;
        [SerializeField]
        [TextArea]
        private string effectDescription;
        [SerializeField]
        [TextArea]
        private string loreDescription;
        [SerializeField]
        private AudioClip consumedAudioClip;
        [SerializeField]
        private ColorSerializedReference accentColor;
        [SerializeField]
        private AudioClip inEffectAudioClip;
        [SerializeField]
        private AudioClip notInEffectAudioClip;

        public ItemClassification Classification
        {
            get
            {
                return this.classification;
            }
        }

        public int StackSize
        {
            get
            {
                return this.stackSize.ImmutableValue;
            }
        }

        public bool IgnoreBuyCostModifiers
        {
            get
            {
                return this.ignoreBuyCostModifiers;
            }
        }

        public int CurrencyValue
        {
            get
            {
                return this.currencyValue.ImmutableValue;
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

        public string PowerDescription
        {
            get
            {
                return this.powerDescription;
            }
        }

        public string EffectDescription
        {
            get
            {
                return this.effectDescription;
            }
        }

        public string LoreDescription
        {
            get
            {
                return this.loreDescription;
            }
        }

        public AudioClip ConsumedAudioClip
        {
            get
            {
                return this.consumedAudioClip;
            }
        }

        public Color AccentColor
        {
            get
            {
                return this.accentColor.ImmutableValue;
            }
        }

        public AudioClip InEffectAudioClip
        {
            get
            {
                return this.inEffectAudioClip;
            }
        }

        public AudioClip NotInEffectAudioClip
        {
            get
            {
                return this.notInEffectAudioClip;
            }
        }

        public Item CreateItem()
        {
            return FrigidInstancing.CreateInstance<Item>(this.itemPrefab);
        }

        public List<Item> CreateItems(int quantity)
        {
            List<Item> spawnedItems = new List<Item>();
            for (int i = 0; i < quantity; i++)
            {
                spawnedItems.Add(CreateItem());
            }
            return spawnedItems;
        }
    }
}
