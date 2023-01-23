using System;
using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceTransactionPopup : FrigidMonoBehaviour
    {
        [SerializeField]
        private TransactionRow buyRow;
        [SerializeField]
        private TransactionRow sellRow;

        public void FillTransaction(ItemStash vendorItemStash, ItemStash customerItemStash)
        {
            bool contentsActive = false;
            this.buyRow.Row.SetActive(false);
            if (vendorItemStash.TryGetItemStorable(out ItemStorable buyingItemStorable))
            {
                int buyCost = vendorItemStash.CalculateBuyCost(buyingItemStorable);
                if (buyCost > 0)
                {
                    contentsActive = true;
                    this.buyRow.Row.SetActive(true);
                    this.buyRow.ItemIconImage.sprite = buyingItemStorable.Icon;
                    this.buyRow.ItemQuantityText.enabled = vendorItemStash.CurrentQuantity > 1;
                    this.buyRow.ItemQuantityText.text = vendorItemStash.CurrentQuantity.ToString();
                    this.buyRow.SingleCostText.text = buyCost.ToString();
                    this.buyRow.StackCostPlate.SetActive(vendorItemStash.CurrentQuantity > 1);
                    this.buyRow.StackCostText.text = (buyCost * vendorItemStash.CurrentQuantity).ToString();
                }
            }

            this.sellRow.Row.SetActive(false);
            if (customerItemStash.TryGetItemStorable(out ItemStorable sellingItemStorable))
            {
                int sellCost = vendorItemStash.CalculateSellCost(sellingItemStorable);
                if (sellCost > 0)
                {
                    contentsActive = true;
                    this.sellRow.Row.SetActive(true);
                    this.sellRow.ItemIconImage.sprite = sellingItemStorable.Icon;
                    this.sellRow.ItemQuantityText.enabled = customerItemStash.CurrentQuantity > 1;
                    this.sellRow.ItemQuantityText.text = customerItemStash.CurrentQuantity.ToString();
                    this.sellRow.SingleCostText.text = sellCost.ToString();
                    this.sellRow.StackCostPlate.SetActive(customerItemStash.CurrentQuantity > 1);
                    this.sellRow.StackCostText.text = (sellCost * customerItemStash.CurrentQuantity).ToString();
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
