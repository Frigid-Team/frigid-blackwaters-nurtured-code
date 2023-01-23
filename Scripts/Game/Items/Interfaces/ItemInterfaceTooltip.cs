using System;
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

        private List<Tuple<ItemStash, Vector2>> queuedStashes;
        private ItemInterfaceHand hand;

        public void ShowTooltip(ItemInterfaceHand hand)
        {
            this.gameObject.SetActive(true);
            this.hand = hand;
            if (this.hand.TryGetHeldItemStash(out HoldingItemStash heldItemStash)) 
            {
                heldItemStash.OnQuantityUpdated += UpdatePopupsAndPosition;
            }
            UpdatePopupsAndPosition();
        }

        public void HideTooltip()
        {
            if (this.hand.TryGetHeldItemStash(out HoldingItemStash heldItemStash))
            {
                heldItemStash.OnQuantityUpdated -= UpdatePopupsAndPosition;
            }
            this.hand = null;
            if (this.queuedStashes.Count > 0)
            {
                this.queuedStashes[0].Item1.OnQuantityUpdated -= UpdatePopupsAndPosition;
            }
            this.queuedStashes.Clear();
            this.gameObject.SetActive(false);
        }

        public void AddStashToShow(ItemStash itemStash, Vector2 absoluteShowPosition)
        {
            if (this.hand == null) return;

            int foundIndex = this.queuedStashes.FindIndex((Tuple<ItemStash, Vector2> queuedStash) => { return queuedStash.Item1 == itemStash; });
            if (foundIndex < 0)
            {
                if (this.queuedStashes.Count == 0)
                {
                    itemStash.OnQuantityUpdated += UpdatePopupsAndPosition;
                }
                this.queuedStashes.Add(new Tuple<ItemStash, Vector2>(itemStash, absoluteShowPosition));
                UpdatePopupsAndPosition();
            }
        }

        public void RemoveStashToShow(ItemStash itemStash)
        {
            int foundIndex = this.queuedStashes.FindIndex((Tuple<ItemStash, Vector2> queuedStash) => { return queuedStash.Item1 == itemStash; });
            if (foundIndex >= 0)
            {
                if (foundIndex == 0)
                {
                    itemStash.OnQuantityUpdated -= UpdatePopupsAndPosition;
                    if (this.queuedStashes.Count > 1)
                    {
                        this.queuedStashes[1].Item1.OnQuantityUpdated += UpdatePopupsAndPosition;
                    }
                }
                this.queuedStashes.RemoveAt(foundIndex);
                UpdatePopupsAndPosition();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.queuedStashes = new List<Tuple<ItemStash, Vector2>>();
        }

        protected override void Start()
        {
            base.Start();
            this.gameObject.SetActive(false);
        }

        private void UpdatePopupsAndPosition()
        {
            if (this.queuedStashes.Count > 0 && this.hand.TryGetHeldItemStash(out HoldingItemStash heldItemStash))
            {
                this.infoPopup.gameObject.SetActive(true);
                this.infoPopup.FillInfo(this.queuedStashes[0].Item1);
                this.transactionPopup.gameObject.SetActive(true);
                this.transactionPopup.FillTransaction(this.queuedStashes[0].Item1, heldItemStash);

                LayoutRebuilder.ForceRebuildLayoutImmediate(this.contentsTransform);

                Vector2 absoluteShowPosition = this.queuedStashes[0].Item2;
                float halfWidth = this.contentsTransform.rect.width / 2;
                float halfHeight = this.contentsTransform.rect.height / 2;
                Vector2 localCenterPosition = this.transform.InverseTransformPoint(absoluteShowPosition);

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
