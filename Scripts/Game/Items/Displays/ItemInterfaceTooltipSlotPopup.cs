using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemInterfaceTooltipSlotPopup : FrigidMonoBehaviour
    {
        [SerializeField]
        private GameObject transactionPlate;
        [SerializeField]
        private GameObject cannotTransferInPlate;
        [SerializeField]
        private GameObject cannotTransferOutPlate;
        [SerializeField]
        private GameObject replenishPlate;
        [SerializeField]
        private GameObject discardPlate;

        public void Fill(ItemStash hoveredStash, ItemStash holdingStash)
        {
            this.transactionPlate.SetActive(hoveredStash.DoesTransferInvolveTransaction(holdingStash) || holdingStash.DoesTransferInvolveTransaction(hoveredStash));
            this.cannotTransferInPlate.SetActive(hoveredStash.Storage.CannotTransferInItems);
            this.cannotTransferOutPlate.SetActive(hoveredStash.Storage.CannotTransferOutItems);
            this.replenishPlate.SetActive(hoveredStash.Storage.ReplenishTakenItems);
            this.discardPlate.SetActive(hoveredStash.Storage.DiscardReplacedItems);
            this.gameObject.SetActive(this.cannotTransferInPlate.activeSelf || this.cannotTransferOutPlate.activeSelf || this.discardPlate.activeSelf || this.replenishPlate.activeSelf);
        }
    }
}
