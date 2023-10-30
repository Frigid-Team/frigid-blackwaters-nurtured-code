using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class BoonExchangeInterface : FrigidMonoBehaviour
    {
        [SerializeField]
        private CurrentBoonsInterface currentBoonsInterface;
        [SerializeField]
        private BoonPannableSpace boonPannableSpace;
        [SerializeField]
        private StampsInterface boonStampsInterface;
        [SerializeField]
        private BoonLoadoutSelectorInterface boonLoadoutInterface;

        public void Open(BoonExchangeLayout boonLayout, BoonInventory boonInventory)
        {
            this.gameObject.SetActive(true);
            this.boonLoadoutInterface.OnMenuOpen(boonInventory);
            this.currentBoonsInterface.OnMenuOpen(boonInventory);
            this.boonPannableSpace.OnMenuOpen(boonLayout);
            this.boonStampsInterface.OnMenuOpen();
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
            this.currentBoonsInterface.OnMenuClose();
            this.boonStampsInterface.OnMenuClose();
            this.boonPannableSpace.OnMenuClose();
            this.boonLoadoutInterface.OnMenuClose();
        }

        protected override void Awake()
        {
            base.Awake();
            this.gameObject.SetActive(false);
        }
    }
}