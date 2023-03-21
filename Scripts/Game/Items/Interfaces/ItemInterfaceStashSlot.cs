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

        [Header("Display")]
        [SerializeField]
        private Image borderImage;
        [SerializeField]
        private Image highlightImage;
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

        [Header("Colors")]
        [SerializeField]
        private ColorSerializedReference defaultColor;
        [SerializeField]
        private ColorSerializedReference cannotStackColor;
        [SerializeField]
        private ColorSerializedReference isFullColor;

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
        private AudioClip transferFailureClip;

        private ItemStorageGrid storageGrid;
        private ItemStash stash;
        private ItemInterfaceHand hand;
        private ItemInterfaceTooltip tooltip;

        private TransferAction transferAction;
        private PointerEventData.InputButton transferButton;
        private FrigidCoroutine useHoldRoutine;
        private int lastClickCount;
        private bool pointerEntered;

        public void PopulateForInteraction(
            ItemStorageGrid storageGrid, 
            ItemStash stash, 
            ItemInterfaceHand hand, 
            ItemInterfaceTooltip tooltip
            )
        {
            if (this.stash != null) this.stash.OnQuantityUpdated -= FillSlot;

            this.rayCastImage.raycastTarget = true;
            this.accentImage.enabled = true;
            this.highlightImage.enabled = false;
            this.storageGrid = storageGrid;
            this.stash = stash;
            this.hand = hand;
            this.tooltip = tooltip;
            this.stash.OnQuantityUpdated += FillSlot;
            FillSlot();
        }

        public void PopulateForDisplay(ItemStash stash)
        {
            if (this.stash != null) this.stash.OnQuantityUpdated -= FillSlot;

            this.rayCastImage.raycastTarget = false;
            this.borderImage.enabled = false;
            this.accentImage.enabled = false;
            this.highlightImage.enabled = false;
            this.stash = stash;
            this.hand = null;
            this.stash.OnQuantityUpdated += FillSlot;
            FillSlot();
        }

        public void FillSlot()
        {
            this.iconImage.enabled = this.stash.CurrentQuantity > 0;
            this.quantityText.enabled = this.stash.CurrentQuantity > 1;

            if (this.stash.TryGetTopmostItem(out Item topmostItem) && this.stash.TryGetStorable(out ItemStorable storable))
            {
                this.iconImage.sprite = storable.Icon;
                this.quantityText.text = this.stash.CurrentQuantity.ToString();
                this.accentImage.color = storable.AccentColor;
                if (topmostItem.IsInEffect)
                {
                    this.accentAnimator.Play(Animator.StringToHash(this.addAccentAnimationName), 0, 1);
                }
                else
                {
                    this.accentAnimator.Play(Animator.StringToHash(this.removeAccentAnimationName), 0, 1);
                }
            }
            else
            {
                this.accentAnimator.Play(Animator.StringToHash(this.removeAccentAnimationName), 0, 1);
            }

            SetDefaultHoverColors();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.pointerEntered = true;
            this.highlightImage.enabled = true;
            SetContextualHoverColors();
            ResetTransfer();
            this.tooltip.AddStashToShow(this.stash, this.transform.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.pointerEntered = false;
            FinishItemAction(eventData);
            this.highlightImage.enabled = false;
            this.quantityText.color = this.defaultColor.ImmutableValue;
            SetDefaultHoverColors();
            ResetTransfer();
            this.tooltip.RemoveStashToShow(this.stash);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (this.pointerEntered) BeginItemAction(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.pointerEntered) FinishItemAction(eventData);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.pointerEntered = false;
            this.highlightImage.enabled = false;
            if (this.stash != null)
            {
                this.stash.OnQuantityUpdated += FillSlot;
                FillSlot();
            }
            ResetTransfer();

            SetDefaultHoverColors();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.stash != null)
            {
                this.stash.OnQuantityUpdated -= FillSlot;
            }
            ResetTransfer();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void BeginItemAction(PointerEventData eventData)
        {
            if (!this.hand.TryGetHoldingStash(out HoldingItemStash heldItemStash) ||
                eventData.button != PointerEventData.InputButton.Left &&
                eventData.button != PointerEventData.InputButton.Right) return;

            if (InterfaceInput.QuickHeld)
            {
                this.transferButton = eventData.button;
                this.transferAction = TransferAction.QuickTransfer;
                return;
            }

            StopWaitForUse();
            StartWaitForUse();

            if (eventData.clickCount - this.lastClickCount == 1 || this.transferAction == TransferAction.None || this.transferButton != eventData.button)
            {
                this.transferButton = eventData.button;
                if (heldItemStash.CurrentQuantity == 0)
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

            if (!this.hand.TryGetHoldingStash(out HoldingItemStash heldItemStash) ||
                eventData.button != PointerEventData.InputButton.Left &&
                eventData.button != PointerEventData.InputButton.Right) return;

            if (this.transferAction == TransferAction.QuickTransfer)
            {
                int quantity = this.transferButton == PointerEventData.InputButton.Left ? this.stash.CurrentQuantity : 1;
                this.hand.QuickTransferItems(this.storageGrid, this.stash, quantity, out int numItemsTransferred);

                if (numItemsTransferred > 0)
                {
                    this.audioSource.clip = this.quickTransferSuccessClip;
                    this.audioSource.Play();
                }
                else
                {
                    this.audioSource.clip = this.transferFailureClip;
                    this.audioSource.Play();
                }

                this.transferAction = TransferAction.None;
            }

            StopWaitForUse();
            if (this.transferButton == eventData.button)
            {
                if (this.transferAction == TransferAction.Hold) 
                {
                    if (this.stash.CurrentQuantity > 0)
                    {
                        int quantity = this.transferButton == PointerEventData.InputButton.Left ? this.stash.CurrentQuantity : 1;
                        this.hand.HoldItems(this.stash, quantity, out int numItemsHeld);

                        if (numItemsHeld > 0)
                        {
                            this.audioSource.clip = this.holdSuccessClip;
                            this.audioSource.Play();
                        }
                        else
                        {
                            this.audioSource.clip = this.transferFailureClip;
                            this.audioSource.Play();
                        }
                    }

                    if (this.stash.CurrentQuantity == 0) this.transferAction = TransferAction.None;
                }
                else if (this.transferAction == TransferAction.Deposit)
                {
                    if (heldItemStash.CurrentQuantity > 0)
                    {
                        int quantity = this.transferButton == PointerEventData.InputButton.Left ? heldItemStash.CurrentQuantity : 1;
                        this.hand.DepositItems(this.stash, quantity, out int numItemsDeposited);

                        if (numItemsDeposited > 0)
                        {
                            this.audioSource.clip = this.depositSuccessClip;
                            this.audioSource.Play();
                        }
                        else
                        {
                            this.audioSource.clip = this.transferFailureClip;
                            this.audioSource.Play();
                        }
                    }

                    if (heldItemStash.CurrentQuantity == 0) this.transferAction = TransferAction.None;
                }
            }

            SetContextualHoverColors();
        }

        private void StartWaitForUse()
        {
            if (this.useHoldRoutine == null && this.stash.HasItemAndIsUsable)
            {
                this.useHoldRoutine = FrigidCoroutine.Run(WaitForUse(), this.gameObject);
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
            yield return new FrigidCoroutine.DelayForSecondsRealTime(this.useDelayDuration);

            this.transferAction = TransferAction.None;
            this.useProgressImage.enabled = true;
            float elapsedDuration = 0;
            while (elapsedDuration < this.usingDuration)
            {
                elapsedDuration += Time.unscaledDeltaTime;
                this.useProgressImage.fillAmount = elapsedDuration / this.usingDuration;
                yield return null;
            }

            if (this.stash.TryGetTopmostItem(out Item prevTopmostItem) && this.stash.TryGetStorable(out ItemStorable itemStorable))
            {
                bool wasInEffect = prevTopmostItem.IsInEffect;
                if (this.stash.UseTopmostItem())
                {
                    this.audioSource.clip = itemStorable.ConsumedAudioClip;
                    this.audioSource.Play();
                }
                else if (this.stash.TryGetTopmostItem(out Item nextTopmostItem) && nextTopmostItem.IsInEffect != wasInEffect)
                {
                    if (nextTopmostItem.IsInEffect)
                    {
                        this.audioSource.clip = itemStorable.InEffectAudioClip;
                        this.audioSource.Play();
                        this.accentAnimator.Play(Animator.StringToHash(this.addAccentAnimationName));
                    }
                    else
                    {
                        this.audioSource.clip = itemStorable.NotInEffectAudioClip;
                        this.audioSource.Play();
                        this.accentAnimator.Play(Animator.StringToHash(this.removeAccentAnimationName));
                    }
                }
            }

            ResetTransfer();
        }

        private void SetContextualHoverColors()
        {
            SetDefaultHoverColors();

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
            this.highlightImage.color = this.defaultColor.ImmutableValue;
            this.quantityText.color = this.defaultColor.ImmutableValue;
        }

        private void ResetTransfer()
        {
            StopWaitForUse();
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
