using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class StampsInterface : FrigidMonoBehaviour
    {
        [SerializeField]
        private Text currentQuantityText;
        [SerializeField]
        private Text totalQuantityText;
        [SerializeField]
        private BoonInventory boonInventory;

        public void OnMenuOpen()
        {
            this.totalQuantityText.text = Stamps.TotalAmount + " total stamps";

            this.UpdateStampCount(Stamps.CurrentAmount);
            this.boonInventory.TryGetCurrentLoadout(out BoonLoadout currentLoadout);
            Stamps.OnCurrentQuantityChanged += this.UpdateStampCount;
        }

        public void OnMenuClose()
        {
            Stamps.OnCurrentQuantityChanged -= this.UpdateStampCount;
        }

        private void UpdateStampCount(int currentStamps)
        {
            this.currentQuantityText.text = currentStamps.ToString();
        }
    }
}