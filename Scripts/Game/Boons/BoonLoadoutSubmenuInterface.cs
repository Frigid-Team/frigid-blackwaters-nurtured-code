using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class BoonLoadoutSubmenuInterface : FrigidMonoBehaviour
    {
        [SerializeField]
        private Button changeIconButton;
        [SerializeField]
        private Button moveRightButton;
        [SerializeField]
        private Button moveLeftButton;
        [SerializeField]
        private Button deleteButton;

        private BoonInventory activeBoonInventory;

        public void UpdateInventory(BoonInventory activeBoonInventory)
        {
            this.activeBoonInventory = activeBoonInventory;
        }

        public void UpdateButtons(int index)
        {
            this.deleteButton.onClick.RemoveAllListeners();
            this.moveLeftButton.onClick.RemoveAllListeners();
            this.moveRightButton.onClick.RemoveAllListeners();
            this.changeIconButton.onClick.RemoveAllListeners();

            this.deleteButton.onClick.AddListener(() => {
                this.activeBoonInventory.RemoveLoadout(index);
            });
            this.changeIconButton.onClick.AddListener(() =>
            {
                // TODO add interface for changing the icon of the loadout
            });
            this.moveRightButton.onClick.AddListener(() =>
            {
                if(index >= this.activeBoonInventory.NumberLoadouts - 1)
                {
                    return;
                }
                this.activeBoonInventory.SwapLoadouts(index, index + 1);
            });
            this.moveLeftButton.onClick.AddListener(() =>
            {
                if(index <= 0)
                {
                    return;
                }
                this.activeBoonInventory.SwapLoadouts(index, index - 1);
            });
        }
    }
}