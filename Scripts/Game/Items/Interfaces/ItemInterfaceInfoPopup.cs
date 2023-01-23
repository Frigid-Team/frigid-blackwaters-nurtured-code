using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceInfoPopup : FrigidMonoBehaviour
    {
        [Header("Title Plate")]
        [SerializeField]
        private Image itemIconImage;
        [SerializeField]
        private Text itemNameText;

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
        private Text effectDescriptionText;
        [SerializeField]
        private Text loreDescriptionText;

        public void FillInfo(ItemStash itemStash)
        {
            if (itemStash.TryGetItemStorable(out ItemStorable itemStorable))
            {
                this.gameObject.SetActive(true);

                this.itemIconImage.sprite = itemStorable.Icon;
                this.itemNameText.text = itemStorable.DisplayName;

                this.classificationIconImage.sprite = itemStorable.Classification.Icon;
                this.classificationNameText.text = itemStorable.Classification.DisplayName;
                this.stackQuantityText.text = itemStash.CurrentQuantity.ToString() + "/" + itemStash.MaxQuantity.ToString();

                this.powerPlate.SetActive(itemStorable.PowerDescription.Length > 0);
                this.powerDescriptionText.text = itemStorable.PowerDescription;

                this.effectDescriptionText.text = itemStorable.EffectDescription;
                this.loreDescriptionText.text = itemStorable.LoreDescription;
                return;
            }
            this.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
