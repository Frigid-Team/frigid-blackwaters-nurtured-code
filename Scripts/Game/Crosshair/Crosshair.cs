using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class Crosshair : FrigidMonoBehaviour
    {
        [Header("Fill")]
        [SerializeField]
        private SpriteRenderer fillSpriteRenderer;
        [SerializeField]
        private List<Sprite> fillSprites;

        [Header("Point")]
        [SerializeField]
        private SpriteRenderer pointSpriteRenderer;
        [SerializeField]
        private List<Sprite> pulseSprites;
        [SerializeField]
        private float pulseDuration;

        /*
         * Have to wait for weapons to be reimplemented
        private List<WeaponSocket> currentlySubscribedWeaponSockets;
        private float lastTimePulsed;

        protected override void Awake()
        {
            base.Awake();
            this.currentlySubscribedWeaponSockets = new List<WeaponSocket>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnOrderChanged += ListenToNewWeapons;
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob))
            {
                foreach (WeaponSocket weaponSocket in recentPlayerMob.MobSegments.WeaponSockets)
                {
                    weaponSocket.SpawnedWeapon.OnFired += ResetPulseTime;
                }
                this.currentlySubscribedWeaponSockets = recentPlayerMob.MobSegments.WeaponSockets;
            }
            CursorDisplay.Hidden.Request();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnOrderChanged -= ListenToNewWeapons;
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob))
            {
                foreach (WeaponSocket weaponSocket in recentPlayerMob.MobSegments.WeaponSockets)
                {
                    weaponSocket.SpawnedWeapon.OnFired -= ResetPulseTime;
                }
                this.currentlySubscribedWeaponSockets.Clear();
            }
            CursorDisplay.Hidden.Release();
        }

        protected override void Update()
        {
            base.Update();
            this.transform.position = CharacterInput.AimWorldPosition;
            float cursorFill = 1f;
            Color cursorColour = Color.white;
            if (TryGetCursorWeaponSocket(out WeaponSocket cursorWeaponSocket))
            {
                cursorFill = cursorWeaponSocket.SpawnedWeapon.Cooldown.GetProgress();
                cursorColour = cursorWeaponSocket.ColorIdentifier;
            }
            this.fillSpriteRenderer.color = cursorColour;
            this.pointSpriteRenderer.color = cursorColour;
            this.fillSpriteRenderer.sprite = this.fillSprites[Mathf.FloorToInt((this.fillSprites.Count - 1) * cursorFill)];
            this.pointSpriteRenderer.sprite = this.pulseSprites[Mathf.FloorToInt((this.pulseSprites.Count - 1) * Mathf.Clamp01((Time.time - this.lastTimePulsed) / this.pulseDuration))];
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private bool TryGetCursorWeaponSocket(out WeaponSocket cursorWeaponSocket)
        {
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob))
            {
                foreach (WeaponSocket weaponSocket in recentPlayerMob.MobSegments.WeaponSockets)
                {
                    if (weaponSocket.LatestVisionConeEvaluation)
                    {
                        cursorWeaponSocket = weaponSocket;
                        return true;
                    }
                }
            }
            cursorWeaponSocket = null;
            return false;
        }

        private void ListenToNewWeapons()
        {
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob))
            {
                foreach (WeaponSocket weaponSocket in this.currentlySubscribedWeaponSockets)
                {
                    weaponSocket.SpawnedWeapon.OnFired -= ResetPulseTime;
                }
                foreach (WeaponSocket weaponSocket in recentPlayerMob.MobSegments.WeaponSockets)
                {
                    weaponSocket.SpawnedWeapon.OnFired += ResetPulseTime;
                }
                this.currentlySubscribedWeaponSockets = recentPlayerMob.MobSegments.WeaponSockets;
            }
        }

        private void ResetPulseTime()
        {
            this.lastTimePulsed = Time.time;
        }
        */
    }
}
