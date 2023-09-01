using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemEffectsHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private ItemEffectsGrid itemEffectsGrid;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMob.OnExists += this.Startup;
            PlayerMob.OnUnexists += this.Teardown;
            this.Startup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerMob.OnExists -= this.Startup;
            PlayerMob.OnUnexists -= this.Teardown;
            this.Teardown();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void Startup()
        {
            if (PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out ItemStorage storage))
            {
                this.itemEffectsGrid.ShowStorage(storage);
            }
        }

        private void Teardown()
        {
            if (PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out ItemStorage storage))
            {
                this.itemEffectsGrid.ClearStorage(storage);
            }
        }
    }
}
