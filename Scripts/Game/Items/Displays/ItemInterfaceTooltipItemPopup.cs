using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceTooltipItemPopup : FrigidMonoBehaviour
    {
        [Header("Title Plate")]
        [SerializeField]
        private Image itemIconImage;
        [SerializeField]
        private Text itemNameText;
        [SerializeField]
        private Text rarityNameText;

        [Header("Below Title Plate")]
        [SerializeField]
        private Image classificationIconImage;
        [SerializeField]
        private Text classificationNameText;
        [SerializeField]
        private Text stackQuantityText;

        [Header("Power Plate")]
        [SerializeField]
        private GameObject powerPlate;
        [SerializeField]
        private Text powerDescriptionText;

        [Header("Descriptions")]
        [SerializeField]
        private int maxNumDescriptionCharsPerLine;
        [SerializeField]
        private Text effectDescriptionText;
        [SerializeField]
        private Text loreDescriptionText;
        
        [Header("Storage Lock Plate")]
        [SerializeField]
        private GameObject storageLockPlate;

        public void Fill(ItemStash hoveredStash)
        {
            if (hoveredStash.TryGetStorable(out ItemStorable storable))
            {
                this.gameObject.SetActive(true);

                this.itemIconImage.sprite = storable.Icon;
                this.itemNameText.text = storable.DisplayName;
                this.rarityNameText.text = storable.Rarity.DisplayName;
                this.rarityNameText.color = storable.Rarity.DisplayColor;

                this.classificationIconImage.sprite = storable.Classification.Icon;
                this.classificationNameText.text = storable.Classification.DisplayName;
                this.stackQuantityText.text = hoveredStash.CurrentQuantity.ToString() + "/" + hoveredStash.MaxQuantity.ToString();

                this.powerPlate.SetActive(storable.PowerDescription.Length > 0);
                this.powerDescriptionText.text = storable.PowerDescription;

                this.effectDescriptionText.text = TextFormatting.ManualWrap(storable.EffectDescription, this.maxNumDescriptionCharsPerLine);
                this.loreDescriptionText.text = TextFormatting.ManualWrap(storable.LoreDescription, this.maxNumDescriptionCharsPerLine);

                this.storageLockPlate.SetActive(!hoveredStash.AllStorageChangeable);
                return;
            }
            this.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
