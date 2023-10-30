using FrigidBlackwaters.Core;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class BoonAccessible : SceneAccessible<BoonAccessible>
    {
        [SerializeField]
        private BoonExchangeLayout boonLayout;
        [SerializeField]
        private BoonInventory boonInventory;

        public BoonExchangeLayout BoonLayout
        {
            get
            {
                return this.boonLayout;
            }
        }

        public BoonInventory BoonInventory
        {
            get
            {
                return this.boonInventory;
            }
        }
    }
}
