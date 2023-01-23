using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class BossBarHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private FloatSerializedReference transitionDuration;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private HealthHUD healthHUD;

        /*
        TODO MOBS_V2
        private Mob_Legacy currentBoss;

        protected override void Awake()
        {
            base.Awake();
            this.currentBoss = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Bosses).OnRecentlyPresentMobChanged += RenewBarHUD;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Bosses).OnAddedToPresentMobs += RenewBarHUD;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Bosses).OnRemovedFromPresentMobs += RenewBarHUD;
            RenewBarHUD();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Bosses).OnRecentlyPresentMobChanged -= RenewBarHUD;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Bosses).OnAddedToPresentMobs -= RenewBarHUD;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Bosses).OnRemovedFromPresentMobs -= RenewBarHUD;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void RenewBarHUD(Mob_Legacy changedMob)
        {
            RenewBarHUD();
        }

        private void RenewBarHUD(bool hadOld, Mob_Legacy old, bool hasCurrent, Mob_Legacy current)
        {
            RenewBarHUD();
        }

        private void RenewBarHUD(Mob_Legacy.Status oldStatus, Mob_Legacy.Status newStatus)
        {
            RenewBarHUD();
        }

        private void RenewBarHUD()
        {
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Bosses).TryGetRecentlyPresentMob(out Mob_Legacy recentBoss) && 
                recentBoss.Present && 
                recentBoss.CurrentStatus != Mob_Legacy.Status.Dead)
            {
                if (this.currentBoss == recentBoss) return;

                if (this.currentBoss != null) this.currentBoss.OnStatusChanged -= RenewBarHUD;
                this.currentBoss = recentBoss;
                this.currentBoss.OnStatusChanged += RenewBarHUD;
                this.backgroundImage.enabled = true;
                this.healthHUD.gameObject.SetActive(true);
                this.healthHUD.TransitionToNewHealth(this.transitionDuration.ImmutableValue, recentBoss.MobHealth, 0);
                return;
            }
            this.currentBoss = null;
            this.backgroundImage.enabled = false;
            this.healthHUD.gameObject.SetActive(false);
        }
        */
    }
}
