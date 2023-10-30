using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceStashSlot : FrigidMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Image rayCastImage;
        [SerializeField]
        private float useDelayDuration;
        [SerializeField]
        private float usingDuration;

        [Header("Border & Background")]
        [SerializeField]
        private Image borderImage;
        [SerializeField]
        private Image highlightImage;
        [SerializeField]
        private GameObject backgroundIconsParent;
        [SerializeField]
        private GameObject transactionIcon;
        [SerializeField]
        private GameObject cannotTransferInIcon;
        [SerializeField]
        private GameObject cannotTransferOutIcon;
        [SerializeField]
        private GameObject replenishIcon;
        [SerializeField]
        private GameObject discardIcon;

        [Header("Item Info")]
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Image useProgressImage;
        [SerializeField]
        private Text quantityText;
        [SerializeField]
        private Image accentImage;
        [SerializeField]
        private Animator accentAnimator;
        [SerializeField]
        private string addAccentAnimationName;
        [SerializeField]
        private string removeAccentAnimationName;
        [SerializeField]
        private Image cornerLockImage;
        [SerializeField]
        private Animator cornerLockAnimator;
        [SerializeField]
        private string addCornerLockAnimationName;
        [SerializeField]
        private string removeCornerLockAnimationName;

        [Header("Colors")]
        [SerializeField]
        private ColorSerializedReference transferableColor;
        [SerializeField]
        private ColorSerializedReference cannotStackColor;
        [SerializeField]
        private ColorSerializedReference isFullColor;
        [SerializeField]
        private ColorSerializedReference usableColor;
        [SerializeField]
        private ColorSerializedReference unusableColor;

        [Header("Audio")]
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip quickTransferSuccessClip;
        [SerializeField]
        private AudioClip holdSuccessClip;
        [SerializeField]
        private AudioClip depositSuccessClip;
        [SerializeField]
        private AudioClip buySuccessClip;
        [SerializeField]
        private AudioClip sellSuccessClip;
        [SerializeField]
        private AudioClip transferFailureClip;

        private ItemStash stash;
        private ItemInterfaceHand hand;
        private ItemInterfaceTooltip tooltip;

        private TransferAction transferAction;
        private PointerEventData.InputButton transferButton;
        private FrigidCoroutine useHoldRoutine;
        private int lastClickCount;
        private bool pointerEntered;

        public void PopulateForInteraction(ItemStash stash, ItemInterfaceHand hand, ItemInterfaceTooltip tooltip)
        {
            if (this.stash != null)
            {
                this.stash.OnQuantityUpdated -= this.FillSlot;
                this.stash.OnAnyInUseChanged -= this.FeedbackOnItemsInUse;
                this.stash.OnAllStorageChangeableChanged -= this.FeedbackOnItemsStorageChangeable;
            }

            this.rayCastImage.raycastTarget = true;
            this.borderImage.enabled = true;
            this.backgroundIconsParent.SetActive(true);
            this.accentImage.enabled = true;
            this.cornerLockImage.enabled = true;
            this.highlightImage.enabled = false;

            this.stash = stash;
            this.hand = hand;
            this.tooltip = tooltip;

            this.stash.OnQuantityUpdated += this.FillSlot;
            this.stash.OnAnyInUseChanged += this.FeedbackOnItemsInUse;
            this.stash.OnAllStorageChangeableChanged += this.FeedbackOnItemsStorageChangeable;
            this.FillSlot();
        }

        public void PopulateForDisplay(ItemStash stash, ItemInterfaceHand hand, ItemInterfaceTooltip tooltip)
        {
            if (this.stash != null)
            {
                this.stash.OnQuantityUpdated -= this.FillSlot;
                this.stash.OnAnyInUseChanged -= this.FeedbackOnItemsInUse;
                this.stash.OnAllStorageChangeableChanged -= this.FeedbackOnItemsStorageChangeable;
            }

            this.rayCastImage.raycastTarget = false;
            this.borderImage.enabled = false;
            this.backgroundIconsParent.SetActive(false);
            this.accentImage.enabled = false;
            this.cornerLockImage.enabled = false;
            this.highlightImage.enabled = false;

            this.stash = stash;
            this.hand = hand;
            this.tooltip = tooltip;

            this.stash.OnQuantityUpdated += this.FillSlot;
            this.stash.OnAnyInUseChanged += this.FeedbackOnItemsInUse;
            this.stash.OnAllStorageChangeableChanged += this.FeedbackOnItemsStorageChangeable;
            this.FillSlot();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.pointerEntered = true;
            this.highlightImage.enabled = true;
            this.SetContextualHoverColors();
            this.ResetTransfer();
            this.tooltip.AddStashToShow(this.stash, this.transform.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.pointerEntered = false;
            this.highlightImage.enabled = false;
            this.quantityText.color = this.transferableColor.ImmutableValue;
            this.SetDefaultHoverColors();
            this.ResetTransfer();
            this.tooltip.RemoveStashToShow(this.stash);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (this.pointerEntered) this.BeginItemAction(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.pointerEntered) this.FinishItemAction(eventData);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.pointerEntered = false;
            this.highlightImage.enabled = false;
            if (this.stash != null)
            {
                this.stash.OnQuantityUpdated += this.FillSlot;
                this.stash.OnAnyInUseChanged += this.FeedbackOnItemsInUse;
                this.stash.OnAllStorageChangeableChanged += this.FeedbackOnItemsStorageChangeable;
                this.FillSlot();
            }
            this.ResetTransfer();

            this.SetDefaultHoverColors();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.stash != null)
            {
                this.stash.OnQuantityUpdated -= this.FillSlot;
                this.stash.OnAnyInUseChanged -= this.FeedbackOnItemsInUse;
                this.stash.OnAllStorageChangeableChanged -= this.FeedbackOnItemsStorageChangeable;
            }
            this.ResetTransfer();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void FillSlot()
        {
            if (!this.hand.TryGetHoldingStash(out HoldingItemStash holdingStash))
            {
                return;
            }

            this.transactionIcon.SetActive(this.stash.DoesTransferInvolveTransaction(holdingStash) || holdingStash.DoesTransferInvolveTransaction(this.stash));
            this.cannotTransferInIcon.SetActive(this.stash.Storage.CannotTransferInItems);
            this.cannotTransferOutIcon.SetActive(this.stash.Storage.CannotTransferOutItems);
            this.replenishIcon.SetActive(this.stash.Storage.ReplenishTakenItems);
            this.discardIcon.SetActive(this.stash.Storage.DiscardReplacedItems);

            this.iconImage.enabled = this.stash.CurrentQuantity > 0;
            this.quantityText.enabled = this.stash.CurrentQuantity > 1;

            if (this.stash.TryGetStorable(out ItemStorable storable))
            {
                this.iconImage.sprite = storable.Icon;
                this.quantityText.text = this.stash.CurrentQuantity.ToString();
                this.accentImage.color = storable.AccentColor;
            }

            if (this.stash.AnyInUse)
            {
                this.accentAnimator.Play(this.addAccentAnimationName, 0, 1);
            }
            else
            {
                this.accentAnimator.Play(this.removeAccentAnimationName, 0, 1);
            }

            if (this.stash.AllStorageChangeable)
            {
                this.cornerLockAnimator.Play(this.removeCornerLockAnimationName, 0, 1);
            }
            else
            {
                this.cornerLockAnimator.Play(this.addCornerLockAnimationName, 0, 1);
            }

            this.SetDefaultHoverColors();
        }

        private void FeedbackOnItemsInUse()
        {
            if (this.stash.AnyInUse)
            {
                this.accentAnimator.Play(this.addAccentAnimationName);
                if (this.stash.TryGetStorable(out ItemStorable storable)) this.audioSource.PlayOneShot(storable.InUseClip);
            }
            else
            {
                this.accentAnimator.Play(this.removeAccentAnimationName);
                if (this.stash.TryGetStorable(out ItemStorable storable)) this.audioSource.PlayOneShot(storable.NotInUseClip);
            }
        }

        private void FeedbackOnItemsStorageChangeable()
        {
            if (this.stash.AllStorageChangeable)
            {
                this.cornerLockAnimator.Play(this.removeCornerLockAnimationName);
            }
            else
            {
                this.cornerLockAnimator.Play(this.addCornerLockAnimationName);
            }
        }

        private void BeginItemAction(PointerEventData eventData)
        {
            if (!this.hand.TryGetHoldingStash(out HoldingItemStash holdingStash) ||
                eventData.button != PointerEventData.InputButton.Left &&
                eventData.button != PointerEventData.InputButton.Right) return;

            if (InterfaceInput.QuickHeld)
            {
                this.transferButton = eventData.button;
                this.transferAction = TransferAction.QuickTransfer;
                return;
            }

            this.StopWaitForUse();
            this.StartWaitForUse();

            if (eventData.clickCount - this.lastClickCount == 1 || this.transferAction == TransferAction.None || this.transferButton != eventData.button)
            {
                this.transferButton = eventData.button;
                if (holdingStash.CurrentQuantity == 0)
                {
                    this.transferAction = TransferAction.Hold;
                }
                else
                {
                    this.transferAction = TransferAction.Deposit;
                }
            }
        }

        private void FinishItemAction(PointerEventData eventData)
        {
            this.lastClickCount = eventData.clickCount;

            if (!this.hand.TryGetHoldingStash(out HoldingItemStash holdingStash) ||
                eventData.button != PointerEventData.InputButton.Left &&
                eventData.button != PointerEventData.InputButton.Right) return;

            if (this.transferAction == TransferAction.QuickTransfer)
            {
                this.stash.TryGetStorable(out ItemStorable transferredStorable);

                int quantity = this.transferButton == PointerEventData.InputButton.Left ? this.stash.CurrentQuantity : 1;
                this.hand.QuickTransferItems(this.stash, quantity, out int numItemsTransferred, out List<ItemStash> transferredStashes);

                if (numItemsTransferred > 0)
                {
                    if (this.stash.CalculateBuyCost(transferredStorable) > 0)
                    {
                        this.audioSource.PlayOneShot(this.buySuccessClip);
                    }
                    foreach (ItemStash transferredStash in transferredStashes)
                    {
                        if (transferredStash.CalculateSellCost(transferredStorable) > 0)
                        {
                            this.audioSource.PlayOneShot(this.sellSuccessClip);
                            break;
                        }
                    }
                    this.audioSource.PlayOneShot(this.quickTransferSuccessClip);
                }
                else
                {
                    this.audioSource.PlayOneShot(this.transferFailureClip);
                }

                this.transferAction = TransferAction.None;
            }

            this.StopWaitForUse();
            if (this.transferButton == eventData.button)
            {
                if (this.transferAction == TransferAction.Hold)
                {
                    this.stash.TryGetStorable(out ItemStorable heldStorable);

                    int quantity = this.transferButton == PointerEventData.InputButton.Left ? this.stash.CurrentQuantity : 1;
                    this.hand.HoldItems(this.stash, quantity, out int numItemsHeld);

                    if (numItemsHeld > 0)
                    {
                        if (this.stash.CalculateBuyCost(heldStorable) > 0)
                        {
                            this.audioSource.PlayOneShot(this.buySuccessClip);
                        }
                        this.audioSource.PlayOneShot(this.holdSuccessClip);
                    }
                    else
                    {
                        this.audioSource.PlayOneShot(this.transferFailureClip);
                    }

                    if (this.stash.CurrentQuantity == 0 || numItemsHeld == 0) this.transferAction = TransferAction.None;
                }
                else if (this.transferAction == TransferAction.Deposit)
                {
                    holdingStash.TryGetStorable(out ItemStorable depositedStorable);

                    int quantity = this.transferButton == PointerEventData.InputButton.Left ? holdingStash.CurrentQuantity : 1;
                    this.hand.DepositItems(this.stash, quantity, out int numItemsDeposited);

                    if (numItemsDeposited > 0)
                    {
                        if (this.stash.CalculateSellCost(depositedStorable) > 0)
                        {
                            this.audioSource.PlayOneShot(this.sellSuccessClip);
                        }
                        this.audioSource.PlayOneShot(this.depositSuccessClip);
                    }
                    else
                    {
                        this.audioSource.PlayOneShot(this.transferFailureClip);
                    }

                    if (holdingStash.CurrentQuantity == 0 || numItemsDeposited == 0) this.transferAction = TransferAction.None;
                }
            }

            this.SetContextualHoverColors();
        }

        private void StartWaitForUse()
        {
            if (this.useHoldRoutine == null)
            {
                this.useHoldRoutine = FrigidCoroutine.Run(this.WaitForUse(), this.gameObject);
            }
        }

        private void StopWaitForUse()
        {
            this.useProgressImage.enabled = false;
            if (this.useHoldRoutine != null)
            {
                FrigidCoroutine.Kill(this.useHoldRoutine);
                this.useHoldRoutine = null;
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> WaitForUse()
        {
            if (this.stash.TryGetStorable(out ItemStorable storable))
            {
                yield return new FrigidCoroutine.DelayForSecondsRealTime(this.useDelayDuration);

                bool canUse = this.stash.CanUseTopmostItem();

                Color progressColor = canUse ? this.usableColor.MutableValue : this.unusableColor.MutableValue;
                progressColor.a = this.useProgressImage.color.a;
                this.useProgressImage.color = progressColor;

                this.transferAction = TransferAction.None;
                this.useProgressImage.enabled = true;
                float elapsedDuration = 0;
                while (elapsedDuration < this.usingDuration)
                {
                    elapsedDuration += Time.unscaledDeltaTime;
                    this.useProgressImage.fillAmount = EasingFunctions.EaseOutSine(0f, 1f, elapsedDuration / this.usingDuration);
                    yield return null;
                }

                if (canUse && this.stash.UseTopmostItem())
                {
                    this.audioSource.PlayOneShot(storable.ConsumedAudioClip);
                }
            }
            this.ResetTransfer();
        }

        private void SetContextualHoverColors()
        {
            this.SetDefaultHoverColors();

            if (this.hand.TryGetHoldingStash(out HoldingItemStash heldItemStash))
            {
                if (heldItemStash.TryGetStorable(out ItemStorable storable))
                {
                    if (!this.stash.CanStackStorable(storable))
                    {
                        this.highlightImage.color = this.cannotStackColor.ImmutableValue;
                        this.quantityText.color = this.cannotStackColor.ImmutableValue;
                        return;
                    }

                    if (this.stash.IsFull)
                    {
                        this.highlightImage.color = this.isFullColor.ImmutableValue;
                        this.quantityText.color = this.isFullColor.ImmutableValue;
                        return;
                    }
                }
            }
        }

        private void SetDefaultHoverColors()
        {
            this.highlightImage.color = this.transferableColor.ImmutableValue;
            this.quantityText.color = this.transferableColor.ImmutableValue;
        }

        private void ResetTransfer()
        {
            this.StopWaitForUse();
            this.transferAction = TransferAction.None;
            this.transferButton = PointerEventData.InputButton.Left;
            this.lastClickCount = 0;
        }

        private enum TransferAction
        {
            None,
            Hold,
            Deposit,
            QuickTransfer
        }
    }
}
