using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class CombatHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private FloatSerializedReference transitionDuration;

        [Header("Health")]
        [SerializeField]
        private HealthHUD primaryHealthHUD;
        [SerializeField]
        private HealthHUD secondaryHealthHUD;

        [Header("Weapon Cooldowns")]
        [SerializeField]
        private WeaponCooldownHUD weaponCooldownHUD;

        protected override void OnEnable()
        {
            base.OnEnable();
            // Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnOrderChanged += TransitionHUD;
            TransitionHUD();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnOrderChanged -= TransitionHUD;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void TransitionHUD()
        {
            /* TODO: Mobs_V2
            LinkedList<Mob_Legacy> playerMobPresenceOrder = Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).PresenceOrder;
            if (playerMobPresenceOrder.Count > 1)
            {
                this.primaryHealthHUD.TransitionToNewHealth(this.transitionDuration.ImmutableValue, playerMobPresenceOrder.First.Value.MobHealth);
                this.secondaryHealthHUD.TransitionToNewHealth(this.transitionDuration.ImmutableValue, playerMobPresenceOrder.First.Next.Value.MobHealth);
                this.weaponCooldownHUD.TransitionToNewSocket(this.transitionDuration.ImmutableValue, playerMobPresenceOrder.First.Value.MobSegments.WeaponSockets[0]);
            }
            */
        }
    }
}
