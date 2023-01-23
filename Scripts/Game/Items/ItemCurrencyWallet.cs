using System;

namespace FrigidBlackwaters.Game
{
    public class ItemCurrencyWallet
    {
        private bool isIgnoringTransactionCosts;
        private int currencyCount;
        private Action onCurrencyCountUpdated;
        private Action onTransferFailed;

        public ItemCurrencyWallet(bool isIgnoringTransactionCosts, int initialCurrencyCount)
        {
            this.isIgnoringTransactionCosts = isIgnoringTransactionCosts;
            this.currencyCount = initialCurrencyCount;
        }

        public bool IsIgnoringTransactionCosts
        {
            get
            {
                return this.isIgnoringTransactionCosts;
            }
        }

        public int CurrencyCount
        {
            get
            {
                return this.currencyCount;
            }
        }

        public Action OnCurrencyCountUpdated
        {
            get
            {
                return this.onCurrencyCountUpdated;
            }
            set
            {
                this.onCurrencyCountUpdated = value;
            }
        }

        public Action OnTransferFailed
        {
            get
            {
                return this.onTransferFailed;
            }
            set
            {
                this.onTransferFailed = value;
            }
        }

        public bool TryTransactionFrom(ItemCurrencyWallet otherItemCurrencyWallet, int cost)
        {
            if (!otherItemCurrencyWallet.isIgnoringTransactionCosts && otherItemCurrencyWallet.currencyCount < cost)
            {
                otherItemCurrencyWallet.onTransferFailed?.Invoke();
                return false;
            }

            if (cost > 0)
            {
                if (!otherItemCurrencyWallet.isIgnoringTransactionCosts)
                {
                    otherItemCurrencyWallet.currencyCount -= cost;
                    otherItemCurrencyWallet.onCurrencyCountUpdated?.Invoke();
                }
                if (!this.isIgnoringTransactionCosts)
                {
                    this.currencyCount += cost;
                    this.onCurrencyCountUpdated?.Invoke();
                }
            }
            return true;
        }
    }
}