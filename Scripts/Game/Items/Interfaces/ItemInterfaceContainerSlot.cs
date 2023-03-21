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

        private ItemContainer container;

        public void Populate(ItemContainer container, Action onClicked)
        {
            this.container = container;

            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClicked.Invoke());
            this.button.image.sprite = container.SmallIcon;
            this.button.enabled = true;

            this.displayNameText.text = container.DisplayName;
            this.displayNameText.enabled = false;
        }

        public void Focus()
        {
            this.button.enabled = false;
            this.button.image.sprite = this.container.LargeIcon;
        }

        public void Unfocus()
        {
            this.button.enabled = true;
            this.button.image.sprite = this.container.SmallIcon;
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
