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

        public void FillInfo(ItemStash stash)
        {
            if (stash.TryGetStorable(out ItemStorable storable))
            {
                this.gameObject.SetActive(true);

                this.itemIconImage.sprite = storable.Icon;
                this.itemNameText.text = storable.DisplayName;

                this.classificationIconImage.sprite = storable.Classification.Icon;
                this.classificationNameText.text = storable.Classification.DisplayName;
                this.stackQuantityText.text = stash.CurrentQuantity.ToString() + "/" + stash.MaxQuantity.ToString();

                this.powerPlate.SetActive(storable.PowerDescription.Length > 0);
                this.powerDescriptionText.text = storable.PowerDescription;

                this.effectDescriptionText.text = storable.EffectDescription;
                this.loreDescriptionText.text = storable.LoreDescription;
                return;
            }
            this.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
