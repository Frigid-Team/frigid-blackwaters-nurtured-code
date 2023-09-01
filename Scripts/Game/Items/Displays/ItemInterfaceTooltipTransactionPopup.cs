using System;
using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceTooltipTransactionPopup : FrigidMonoBehaviour
    {
        [SerializeField]
        private TransactionRow buyRow;
        [SerializeField]
        private TransactionRow sellRow;

        public void Fill(ItemStash hoveredStash, ItemStash holdingStash)
        {
            bool contentsActive = false;
            this.buyRow.Row.SetActive(false);
            if (holdingStash.CanTransferFrom(hoveredStash) && holdingStash.DoesTransferInvolveTransaction(hoveredStash) && hoveredStash.TryGetStorable(out ItemStorable buyingItemStorable))
            {
                int buyCost = hoveredStash.CalculateBuyCost(buyingItemStorable);
                if (buyCost > 0)
                {
                    contentsActive = true;
                    this.buyRow.Row.SetActive(true);
                    this.buyRow.ItemIconImage.sprite = buyingItemStorable.Icon;
                    this.buyRow.ItemQuantityText.enabled = hoveredStash.CurrentQuantity > 1;
                    this.buyRow.ItemQuantityText.text = hoveredStash.CurrentQuantity.ToString();
                    this.buyRow.SingleCostText.text = buyCost.ToString();
                    this.buyRow.StackCostPlate.SetActive(hoveredStash.CurrentQuantity > 1);
                    this.buyRow.StackCostText.text = (buyCost * hoveredStash.CurrentQuantity).ToString();
                }
            }

            this.sellRow.Row.SetActive(false);
            if (hoveredStash.CanTransferFrom(holdingStash) && hoveredStash.DoesTransferInvolveTransaction(holdingStash) && holdingStash.TryGetStorable(out ItemStorable sellingItemStorable))
            {
                int sellCost = hoveredStash.CalculateSellCost(sellingItemStorable);
                if (sellCost > 0)
                {
                    contentsActive = true;
                    this.sellRow.Row.SetActive(true);
                    this.sellRow.ItemIconImage.sprite = sellingItemStorable.Icon;
                    this.sellRow.ItemQuantityText.enabled = holdingStash.CurrentQuantity > 1;
                    this.sellRow.ItemQuantityText.text = holdingStash.CurrentQuantity.ToString();
                    this.sellRow.SingleCostText.text = sellCost.ToString();
                    this.sellRow.StackCostPlate.SetActive(holdingStash.CurrentQuantity > 1);
                    this.sellRow.StackCostText.text = (sellCost * holdingStash.CurrentQuantity).ToString();
                }
            }
            this.gameObject.SetActive(contentsActive);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        [Serializable]
        private struct TransactionRow
        {
            [SerializeField]
            private GameObject row;
            [SerializeField]
            private Image itemIconImage;
            [SerializeField]
            private Text itemQuantityText;
            [SerializeField]
            private Text singleCostText;
            [SerializeField]
            private GameObject stackCostPlate;
            [SerializeField]
            private Text stackCostText;

            public GameObject Row
            {
                get
                {
                    return this.row;
                }
            }

            public Image ItemIconImage
            {
                get
                {
                    return this.itemIconImage;
                }
            }

            public Text ItemQuantityText
            {
                get
                {
                    return this.itemQuantityText;
                }
            }

            public Text SingleCostText
            {
                get
                {
                    return this.singleCostText;
                }
            }

            public GameObject StackCostPlate
            {
                get
                {
                    return this.stackCostPlate;
                }
            }

            public Text StackCostText
            {
                get
                {
                    return this.stackCostText;
                }
            }
        }
    }
}
