using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace FrigidBlackwaters.Game
{
    public class BoonLoadoutInterface : FrigidMonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image backgroundImage;  // Image just changes based on if its selected  -  Btw these comments will be removed, this is just so that Jing knows what I was thinking as he styles the interfaces
        [SerializeField]
        private Image logoImage; // The unique image that identifies this loadout

        private BoonLoadout boonLoadout;
        private Action<BoonLoadout> onPrimaryClick;
        private Action<BoonLoadout> onSecondaryClick;

        public Action<BoonLoadout> OnPrimaryClick
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

        public Action<BoonLoadout> OnSecondaryClick
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

        public void Populate_(BoonLoadout boonLoadout, Sprite image, Action<BoonLoadout> onPrimaryClickAction)
        {
            this.boonLoadout = boonLoadout;
            this.logoImage.sprite = image;
            this.onPrimaryClick += onPrimaryClickAction;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.onPrimaryClick?.Invoke(this.boonLoadout);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                this.onSecondaryClick?.Invoke(this.boonLoadout);
            }
        }
    }
}