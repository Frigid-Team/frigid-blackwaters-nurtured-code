using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceBar : FrigidMonoBehaviour
    {
        [Header("Scroll")]
        [SerializeField]
        private ItemInterfaceContainerSlot slotPrefab;
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private float maxScrollLength;
        [SerializeField]
        private RectTransform contentTransform;
        [SerializeField]
        private float yPadding;
        [SerializeField]
        private float scrollDuration;

        [Header("Arrows")]
        [SerializeField]
        private Button leftArrow;
        [SerializeField]
        private Button rightArrow;

        [Header("Brace Counts")]
        [SerializeField]
        private Image powerCountImage;
        [SerializeField]
        private Text powerCountText;
        [SerializeField]
        private Image currencyCountImage;
        [SerializeField]
        private Text currencyCountText;
        [SerializeField]
        private ColorSerializedReference failureTextColor;
        [SerializeField]
        private float failureDuration;

        [Header("Transitions")]
        [SerializeField]
        private float transitionDuration;

        [Header("Optimizations")]
        [SerializeField]
        private int numberSlotsPreparedInAdvance;

        private List<ItemInterfaceContainerSlot> currentSlots;
        private RecyclePool<ItemInterfaceContainerSlot> slotPool;
        private ItemPowerBudget currentItemPowerBudget;
        private ItemCurrencyWallet currentItemCurrencyWallet;
        private FrigidCoroutine scrollRoutine;

        private Vector2 onScreenLocalPosition;
        private Vector2 offScreenLocalPosition;
        private int focusedIndex;

        private FrigidCoroutine transitionRoutine;
        private Color originalPowerCountTextColor;
        private Color originalCurrencyCountTextColor;
        private FrigidCoroutine showPowerCountFailureRoutine;
        private FrigidCoroutine showCurrencyCountFailureRoutine;

        public void Populate(
            float scrollWidth,
            Action<int> onClicked, 
            List<ItemStorageGrid> itemStorageGrids,
            ItemPowerBudget itemPowerBudget,
            ItemCurrencyWallet itemCurrencyWallet,
            int startIndex, 
            Vector2 localCenterPosition
            )
        {
            this.focusedIndex = startIndex;

            this.slotPool.Cycle(this.currentSlots, itemStorageGrids.Count);

            RectTransform rectTransform = (RectTransform)this.transform;
            float arrowsWidth = ((RectTransform)this.leftArrow.transform).rect.width + ((RectTransform)this.rightArrow.transform).rect.width;
            rectTransform.sizeDelta = new Vector2(Mathf.Min(this.maxScrollLength, scrollWidth) + arrowsWidth, rectTransform.rect.height);

            float slotWidth = ((RectTransform)this.slotPrefab.transform).rect.width;
            RectTransform scrollRectTransform = (RectTransform)this.scrollRect.transform;
            this.contentTransform.sizeDelta = new Vector2((itemStorageGrids.Count - 1) * slotWidth + scrollRectTransform.rect.width, rectTransform.rect.height);
            for (int i = 0; i < itemStorageGrids.Count; i++)
            {
                ItemInterfaceContainerSlot containerSlot = this.currentSlots[i];
                containerSlot.transform.localPosition = new Vector2(-slotWidth * (itemStorageGrids.Count - 1) / 2 + slotWidth * i, 0);

                int storageGridIndex = i;
                containerSlot.Populate(itemStorageGrids[i].ItemContainer, () => onClicked.Invoke(storageGridIndex));
                if (storageGridIndex == this.focusedIndex) containerSlot.Focus();
            }

            this.scrollRect.horizontalNormalizedPosition = this.currentSlots.Count <= 1 ? 0.5f : ((float)this.focusedIndex / (this.currentSlots.Count - 1));

            this.leftArrow.onClick.RemoveAllListeners();
            this.leftArrow.onClick.AddListener(() => onClicked.Invoke(this.focusedIndex - 1));
            this.leftArrow.interactable = this.focusedIndex > 0;
            this.rightArrow.onClick.RemoveAllListeners();
            this.rightArrow.onClick.AddListener(() => onClicked.Invoke(this.focusedIndex + 1));
            this.rightArrow.interactable = this.focusedIndex < this.currentSlots.Count - 1;

            this.powerCountImage.enabled = itemPowerBudget.MaxPower > 0;
            this.powerCountText.enabled = itemPowerBudget.MaxPower > 0;
            this.currentItemPowerBudget = itemPowerBudget;

            this.currencyCountImage.enabled = !itemCurrencyWallet.IsIgnoringTransactionCosts;
            this.currencyCountText.enabled = !itemCurrencyWallet.IsIgnoringTransactionCosts;
            this.currentItemCurrencyWallet = itemCurrencyWallet;

            RectTransform parentRectTransform = (RectTransform)this.transform.parent;
            this.onScreenLocalPosition = new Vector2(localCenterPosition.x, -parentRectTransform.rect.height / 2 + this.yPadding);
            this.offScreenLocalPosition = new Vector2(localCenterPosition.x, -parentRectTransform.rect.height / 2 - this.yPadding);
            this.transform.localPosition = this.offScreenLocalPosition;
        }

        public void Scroll(int currentIndex)
        {
            this.focusedIndex = currentIndex;

            for (int i = 0; i < this.currentSlots.Count; i++)
            {
                if (i == this.focusedIndex)
                {
                    this.currentSlots[i].Focus();
                }
                else
                {
                    this.currentSlots[i].Unfocus();
                }
            }

            FrigidCoroutine.Kill(this.scrollRoutine);
            this.scrollRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    this.scrollDuration,
                    this.scrollRect.horizontalNormalizedPosition,
                    this.currentSlots.Count <= 1 ? 0.5f : ((float)this.focusedIndex / (this.currentSlots.Count - 1)),
                    useRealTime: true,
                    onValueUpdated: (float normalizedPosition) => { this.scrollRect.horizontalNormalizedPosition = normalizedPosition; }
                    ),
                this.gameObject
                );

            this.leftArrow.interactable = this.focusedIndex > 0;
            this.rightArrow.interactable = this.focusedIndex < this.currentSlots.Count - 1;
        }

        public void TransitionOnScreen()
        {
            FrigidCoroutine.Kill(this.transitionRoutine);
            this.transitionRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    this.transitionDuration,
                    this.offScreenLocalPosition, 
                    this.onScreenLocalPosition, 
                    useRealTime: true,
                    onValueUpdated: (Vector2 localPosition) => this.transform.localPosition = localPosition
                    ),
                this.gameObject
                );
            this.currentItemPowerBudget.OnCurrentPowerChanged += UpdatePowerCount;
            this.currentItemPowerBudget.OnUsePowerFailed += ShowPowerCountFailure;
            UpdatePowerCount();
            this.currencyCountText.color = this.originalCurrencyCountTextColor;
            this.currentItemCurrencyWallet.OnCurrencyCountUpdated += UpdateCurrencyCount;
            this.currentItemCurrencyWallet.OnTransferFailed += ShowCurrencyCountFailure;
            UpdateCurrencyCount();
            this.powerCountText.color = this.originalPowerCountTextColor;
        }

        public void TransitionOffScreen(Action onComplete = null)
        {
            FrigidCoroutine.Kill(this.transitionRoutine);
            this.transitionRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    this.transitionDuration,
                    this.onScreenLocalPosition,
                    this.offScreenLocalPosition,
                    useRealTime: true,
                    onValueUpdated: (Vector2 localPosition) => this.transform.localPosition = localPosition,
                    onComplete: onComplete
                    ),
                this.gameObject
                );
            this.currentItemPowerBudget.OnCurrentPowerChanged -= UpdatePowerCount;
            this.currentItemPowerBudget.OnUsePowerFailed -= ShowPowerCountFailure;
            this.currentItemCurrencyWallet.OnCurrencyCountUpdated -= UpdateCurrencyCount;
            this.currentItemCurrencyWallet.OnTransferFailed -= ShowCurrencyCountFailure;
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentSlots = new List<ItemInterfaceContainerSlot>();
            this.slotPool = new RecyclePool<ItemInterfaceContainerSlot>(
                this.numberSlotsPreparedInAdvance,
                () => FrigidInstancing.CreateInstance<ItemInterfaceContainerSlot>(this.slotPrefab, this.contentTransform, false),
                (ItemInterfaceContainerSlot slot) => FrigidInstancing.DestroyInstance(slot)
                );
            this.originalCurrencyCountTextColor = this.currencyCountText.color;
            this.originalPowerCountTextColor = this.powerCountText.color;
        }

        private void UpdatePowerCount()
        {
            this.powerCountText.text = this.currentItemPowerBudget.CurrentPower.ToString() + "/" + this.currentItemPowerBudget.MaxPower.ToString();
        }

        private void ShowPowerCountFailure()
        {
            FrigidCoroutine.Kill(this.showPowerCountFailureRoutine);
            this.showPowerCountFailureRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    this.failureDuration,
                    this.originalPowerCountTextColor,
                    this.failureTextColor.ImmutableValue,
                    pingPong: true,
                    useRealTime: true,
                    onValueUpdated: (Color color) => { this.powerCountText.color = color; }
                    ),
                this.gameObject
                );
        }

        private void UpdateCurrencyCount()
        {
            this.currencyCountText.text = this.currentItemCurrencyWallet.CurrencyCount.ToString();
        }

        private void ShowCurrencyCountFailure()
        {
            FrigidCoroutine.Kill(this.showCurrencyCountFailureRoutine);
            this.showCurrencyCountFailureRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    this.failureDuration,
                    this.originalCurrencyCountTextColor,
                    this.failureTextColor.ImmutableValue,
                    pingPong: true,
                    useRealTime: true,
                    onValueUpdated: (Color color) => { this.currencyCountText.color = color; }
                    ),
                this.gameObject
                );
        }
    }
}
