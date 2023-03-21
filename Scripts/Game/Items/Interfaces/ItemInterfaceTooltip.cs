using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceTooltip : FrigidMonoBehaviour
    {
        [SerializeField]
        private Vector2 padding;
        [SerializeField]
        private RectTransform contentsTransform;
        [SerializeField]
        private ItemInterfaceInfoPopup infoPopup;
        [SerializeField]
        private ItemInterfaceTransactionPopup transactionPopup;

        private List<(ItemStash stash, Vector2 showPosition)> queuedStashShows;
        private ItemInterfaceHand hand;

        public void ShowTooltip(ItemInterfaceHand hand)
        {
            this.gameObject.SetActive(true);
            this.hand = hand;
            if (this.hand.TryGetHoldingStash(out HoldingItemStash heldItemStash)) 
            {
                heldItemStash.OnQuantityUpdated += UpdatePopupsAndPosition;
            }
            UpdatePopupsAndPosition();
        }

        public void HideTooltip()
        {
            if (this.hand.TryGetHoldingStash(out HoldingItemStash heldItemStash))
            {
                heldItemStash.OnQuantityUpdated -= UpdatePopupsAndPosition;
            }
            this.hand = null;
            if (this.queuedStashShows.Count > 0)
            {
                this.queuedStashShows[0].stash.OnQuantityUpdated -= UpdatePopupsAndPosition;
            }
            this.queuedStashShows.Clear();
            this.gameObject.SetActive(false);
        }

        public void AddStashToShow(ItemStash stash, Vector2 showPosition)
        {
            int foundIndex = this.queuedStashShows.FindIndex(((ItemStash stash, Vector2 showPosition) stashShow) => { return stashShow.stash == stash; });
            if (foundIndex < 0)
            {
                if (this.queuedStashShows.Count == 0)
                {
                    stash.OnQuantityUpdated += UpdatePopupsAndPosition;
                }
                this.queuedStashShows.Add((stash, showPosition));
                UpdatePopupsAndPosition();
            }
        }

        public void RemoveStashToShow(ItemStash stash)
        {
            int foundIndex = this.queuedStashShows.FindIndex(((ItemStash stash, Vector2 showPosition) stashShow) => { return stashShow.stash == stash; });
            if (foundIndex >= 0)
            {
                if (foundIndex == 0)
                {
                    stash.OnQuantityUpdated -= UpdatePopupsAndPosition;
                    if (this.queuedStashShows.Count > 1)
                    {
                        this.queuedStashShows[1].stash.OnQuantityUpdated += UpdatePopupsAndPosition;
                    }
                }
                this.queuedStashShows.RemoveAt(foundIndex);
                UpdatePopupsAndPosition();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.queuedStashShows = new List<(ItemStash stash, Vector2 showPosition)>();
        }

        protected override void Start()
        {
            base.Start();
            this.gameObject.SetActive(false);
        }

        private void UpdatePopupsAndPosition()
        {
            if (this.queuedStashShows.Count > 0 && this.hand.TryGetHoldingStash(out HoldingItemStash holdingStash))
            {
                this.infoPopup.gameObject.SetActive(true);
                this.infoPopup.FillInfo(this.queuedStashShows[0].stash);
                this.transactionPopup.gameObject.SetActive(true);
                this.transactionPopup.FillTransaction(this.queuedStashShows[0].stash, holdingStash);

                LayoutRebuilder.ForceRebuildLayoutImmediate(this.contentsTransform);

                float halfWidth = this.contentsTransform.rect.width / 2;
                float halfHeight = this.contentsTransform.rect.height / 2;
                Vector2 localCenterPosition = this.transform.InverseTransformPoint(this.queuedStashShows[0].showPosition);

                if (localCenterPosition.x > 0)
                {
                    this.contentsTransform.localPosition = localCenterPosition + new Vector2(-halfWidth - this.padding.x, -halfHeight + this.padding.y);
                }
                else
                {
                    this.contentsTransform.localPosition = localCenterPosition + new Vector2(halfWidth + this.padding.x, -halfHeight + this.padding.y);
                }
            }
            else
            {
                this.infoPopup.gameObject.SetActive(false);
                this.transactionPopup.gameObject.SetActive(false);
            }
        }
    }
}
