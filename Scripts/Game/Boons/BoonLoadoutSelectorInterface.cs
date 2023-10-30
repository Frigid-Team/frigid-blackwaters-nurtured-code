using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class BoonLoadoutSelectorInterface : FrigidMonoBehaviour
    {
        [SerializeField]
        private BoonLoadoutInterface loadoutPrefab;
        [SerializeField]
        private Transform loadoutsParentTransform;
        [SerializeField]
        private Button newLoadoutButton;
        [SerializeField]
        private GameObject submenu;
        [SerializeField]
        private BoonLoadoutSubmenuInterface submenuInterface;

        //TEMP
        [SerializeField]
        private List<Sprite> icons;

        private BoonInventory boonInventory;
        private List<BoonLoadoutInterface> availableLoadouts;

        private void Awake()
        {
            this.availableLoadouts = new List<BoonLoadoutInterface>();
        }

        public void OnMenuOpen(BoonInventory boonInventory)
        {
            this.boonInventory = boonInventory;
            this.UpdateLoadouts();

            this.boonInventory.OnLoadoutsUpdated += this.UpdateLoadouts;

            this.newLoadoutButton.onClick.AddListener(() => this.boonInventory.AddLoadout(this.boonInventory.NumberLoadouts));

            this.submenu.SetActive(false);
            this.submenuInterface.UpdateInventory(this.boonInventory);
        }

        public void OnMenuClose()
        {
            this.boonInventory.OnLoadoutsUpdated -= this.UpdateLoadouts;

            this.newLoadoutButton.onClick.RemoveAllListeners();
        }

        private void ChangeToLoadout(BoonLoadout boonLoadout, int index)
        {
            if(!this.boonInventory.TryGetCurrentLoadout(out BoonLoadout currentLoadout) || currentLoadout == boonLoadout)
            {
                return;
            }
            this.boonInventory.SwitchCurrentLoadout(index);
        }

        private void OpenOptionsSubmenu(BoonLoadoutInterface loadoutInterface, int loadoutIndex)
        {
            this.submenu.SetActive(true);
            this.submenuInterface.UpdateButtons(loadoutIndex);
            this.submenu.transform.position = loadoutInterface.transform.position;
        }

        private void UpdateLoadouts()
        {
            foreach(BoonLoadoutInterface loadoutInterface in this.availableLoadouts)
            {
                FrigidMonoBehaviour.DestroyInstance(loadoutInterface);
            }

            this.availableLoadouts.Clear();

            for (int loadoutIndex = 0; loadoutIndex < this.boonInventory.NumberLoadouts; loadoutIndex++)
            {
                int currentLoadoutIndex = loadoutIndex;
                if(!this.boonInventory.TryGetLoadout(loadoutIndex, out BoonLoadout loadout))
                {
                    continue;
                }
                BoonLoadoutInterface loadoutInterface = FrigidMonoBehaviour.CreateInstance<BoonLoadoutInterface>(this.loadoutPrefab, this.loadoutsParentTransform, false);
                loadoutInterface.Populate_(loadout, 
                    this.icons[currentLoadoutIndex],
                    (BoonLoadout boonLoadout) => {
                    this.ChangeToLoadout(boonLoadout, currentLoadoutIndex);
                    this.OpenOptionsSubmenu(loadoutInterface, currentLoadoutIndex);
                });

                this.availableLoadouts.Add(loadoutInterface);
            }
        }
    }
}