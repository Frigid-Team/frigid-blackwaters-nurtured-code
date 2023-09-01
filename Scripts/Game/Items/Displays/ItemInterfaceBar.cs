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
        private ItemPowerBudget currentPowerBudget;
        private ItemCurrencyWallet currentCurrencyWallet;
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
            List<ItemStorageGrid> storageGrids,
            ItemPowerBudget powerBudget,
            ItemCurrencyWallet currencyWallet,
            int startIndex, 
            Vector2 localCenterPosition
            )
        {
            this.focusedIndex = startIndex;

            this.slotPool.Cycle(this.currentSlots, storageGrids.Count);

            RectTransform rectTransform = (RectTransform)this.transform;
            float arrowsWidth = ((RectTransform)this.leftArrow.transform).rect.width + ((RectTransform)this.rightArrow.transform).rect.width;
            rectTransform.sizeDelta = new Vector2(Mathf.Min(this.maxScrollLength, scrollWidth) + arrowsWidth, rectTransform.rect.height);

            float slotWidth = ((RectTransform)this.slotPrefab.transform).rect.width;
            RectTransform scrollRectTransform = (RectTransform)this.scrollRect.transform;
            this.contentTransform.sizeDelta = new Vector2((storageGrids.Count - 1) * slotWidth + scrollRectTransform.rect.width, rectTransform.rect.height);
            for (int i = 0; i < storageGrids.Count; i++)
            {
                ItemInterfaceContainerSlot containerSlot = this.currentSlots[i];
                containerSlot.transform.localPosition = new Vector2(-slotWidth * (storageGrids.Count - 1) / 2 + slotWidth * i, 0);

                int storageGridIndex = i;
                containerSlot.Populate(storageGrids[i].Container, () => onClicked.Invoke(storageGridIndex));
                if (storageGridIndex == this.focusedIndex) containerSlot.Focus();
            }

            this.scrollRect.horizontalNormalizedPosition = this.currentSlots.Count <= 1 ? 0.5f : ((float)this.focusedIndex / (this.currentSlots.Count - 1));

            this.leftArrow.onClick.RemoveAllListeners();
            this.leftArrow.onClick.AddListener(() => onClicked.Invoke(this.focusedIndex - 1));
            this.leftArrow.interactable = this.focusedIndex > 0;
            this.rightArrow.onClick.RemoveAllListeners();
            this.rightArrow.onClick.AddListener(() => onClicked.Invoke(this.focusedIndex + 1));
            this.rightArrow.interactable = this.focusedIndex < this.currentSlots.Count - 1;

            this.powerCountImage.enabled = powerBudget.MaxPower > 0;
            this.powerCountText.enabled = powerBudget.MaxPower > 0;
            this.currentPowerBudget = powerBudget;

            this.currencyCountImage.enabled = !currencyWallet.IsIgnoringTransactionCosts;
            this.currencyCountText.enabled = !currencyWallet.IsIgnoringTransactionCosts;
            this.currentCurrencyWallet = currencyWallet;

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
                Tween.Value(
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
                Tween.Value(
                    this.transitionDuration,
                    this.offScreenLocalPosition, 
                    this.onScreenLocalPosition, 
                    useRealTime: true,
                    onValueUpdated: (Vector2 localPosition) => this.transform.localPosition = localPosition
                    ),
                this.gameObject
                );
            this.currentPowerBudget.OnCurrentPowerChanged += this.UpdatePowerCount;
            this.currentPowerBudget.OnCurrentPowerChangeFailed += this.ShowPowerCountFailure;
            this.currentPowerBudget.OnMaxPowerChanged += this.UpdatePowerCount;
            this.currentPowerBudget.OnMaxPowerChangeFailed += this.ShowPowerCountFailure;
            this.UpdatePowerCount();
            this.currencyCountText.color = this.originalCurrencyCountTextColor;
            this.currentCurrencyWallet.OnCurrencyCountUpdated += this.UpdateCurrencyCount;
            this.currentCurrencyWallet.OnTransferFailed += this.ShowCurrencyCountFailure;
            this.UpdateCurrencyCount();
            this.powerCountText.color = this.originalPowerCountTextColor;
        }

        public void TransitionOffScreen(Action onComplete = null)
        {
            FrigidCoroutine.Kill(this.transitionRoutine);
            this.transitionRoutine = FrigidCoroutine.Run(
                Tween.Value(
                    this.transitionDuration,
                    this.onScreenLocalPosition,
                    this.offScreenLocalPosition,
                    useRealTime: true,
                    onValueUpdated: (Vector2 localPosition) => this.transform.localPosition = localPosition,
                    onComplete: onComplete
                    ),
                this.gameObject
                );
            this.currentPowerBudget.OnCurrentPowerChanged -= this.UpdatePowerCount;
            this.currentPowerBudget.OnCurrentPowerChangeFailed -= this.ShowPowerCountFailure;
            this.currentPowerBudget.OnMaxPowerChanged -= this.UpdatePowerCount;
            this.currentPowerBudget.OnMaxPowerChangeFailed -= this.ShowPowerCountFailure;
            this.currentCurrencyWallet.OnCurrencyCountUpdated -= this.UpdateCurrencyCount;
            this.currentCurrencyWallet.OnTransferFailed -= this.ShowCurrencyCountFailure;
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentSlots = new List<ItemInterfaceContainerSlot>();
            this.slotPool = new RecyclePool<ItemInterfaceContainerSlot>(
                this.numberSlotsPreparedInAdvance,
                () => CreateInstance<ItemInterfaceContainerSlot>(this.slotPrefab, this.contentTransform, false),
                (ItemInterfaceContainerSlot slot) => DestroyInstance(slot)
                );
            this.originalCurrencyCountTextColor = this.currencyCountText.color;
            this.originalPowerCountTextColor = this.powerCountText.color;
        }

        private void UpdatePowerCount()
        {
            this.powerCountText.text = this.currentPowerBudget.CurrentPower.ToString() + "/" + this.currentPowerBudget.MaxPower.ToString();
        }

        private void ShowPowerCountFailure()
        {
            FrigidCoroutine.Kill(this.showPowerCountFailureRoutine);
            this.showPowerCountFailureRoutine = FrigidCoroutine.Run(
                Tween.Value(
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
            this.currencyCountText.text = this.currentCurrencyWallet.CurrencyCount.ToString();
        }

        private void ShowCurrencyCountFailure()
        {
            FrigidCoroutine.Kill(this.showCurrencyCountFailureRoutine);
            this.showCurrencyCountFailureRoutine = FrigidCoroutine.Run(
                Tween.Value(
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
