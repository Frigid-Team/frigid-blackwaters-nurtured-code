using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class BoonSlotInterface : FrigidMonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Image fadedImage;
        [SerializeField]
        private Text quantityText;
        [SerializeField]
        private BoonInventory boonInventory;

        private Boon boon;
        private Action<Boon> onPrimaryClick;
        private Action<Boon> onSecondaryClick;

        public Boon Boon
        {
            get
            {
                return this.boon;
            }
        }

        public Action<Boon> OnPrimaryClick
        {
            get
            {
                return this.onPrimaryClick;
            }
            set
            {
                this.onPrimaryClick = value;
            }
        }

        public Action<Boon> OnSecondaryClick
        {
            get
            {
                return this.onSecondaryClick;
            }
            set
            {
                this.onSecondaryClick = value;
            }
        }

        public void SetupBoon(Boon boon, int currentQuantity, bool unlocked)
        {
            this.boon = boon;
            this.iconImage.sprite = this.boon.Icon;
            this.quantityText.text = currentQuantity + "/" + this.boon.MaxLoadoutQuantity;
            this.fadedImage.enabled = !unlocked;
        }

        public void UpdateCurrentQuantity(int newQuantity)
        {
            this.quantityText.text = newQuantity + "/" + this.boon.MaxLoadoutQuantity;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.onPrimaryClick?.Invoke(this.boon);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                this.onSecondaryClick?.Invoke(this.boon);
            }
        }
    }
}