using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceContainerSlot : FrigidMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private Text displayNameText;

        private ItemContainer itemContainer;

        public void Populate(ItemContainer itemContainer, Action onClicked)
        {
            this.itemContainer = itemContainer;

            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClicked.Invoke());
            this.button.image.sprite = itemContainer.SmallIcon;
            this.button.enabled = true;

            this.displayNameText.text = itemContainer.DisplayName;
            this.displayNameText.enabled = false;
        }

        public void Focus()
        {
            this.button.enabled = false;
            this.button.image.sprite = this.itemContainer.LargeIcon;
        }

        public void Unfocus()
        {
            this.button.enabled = true;
            this.button.image.sprite = this.itemContainer.SmallIcon;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.displayNameText.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.displayNameText.enabled = false;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
