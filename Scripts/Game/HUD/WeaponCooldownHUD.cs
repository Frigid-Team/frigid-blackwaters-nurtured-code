using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class WeaponCooldownHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private Image fillImage;
        [SerializeField]
        private Text ammoCountText;

        /*
        private FrigidCoroutine transitionRoutine;
        private FrigidCoroutine updateAmmoCountRoutine;
        private FrigidCoroutine updateFillRoutine;

        public void TransitionToNewSocket(float transitionDuration, WeaponSocket weaponSocket)
        {
            StopTransitionsAndUpdate();
            this.transitionRoutine = FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    transitionDuration / 2,
                    1,
                    0,
                    useRealTime: true,
                    onValueUpdated: (float fill) => { this.fillImage.fillAmount = fill; },
                    onComplete:
                    () =>
                    {
                        this.fillImage.color = weaponSocket.ColorIdentifier;
                        this.updateAmmoCountRoutine = FrigidCoroutine.Run(UpdateAmmoCount(weaponSocket), this.gameObject);
                        this.transitionRoutine = FrigidCoroutine.Run(
                            TweenCoroutine.Value(
                                transitionDuration / 2,
                                0,
                                1,
                                useRealTime: true,
                                onValueUpdated: (float fill) => { this.fillImage.fillAmount = fill; },
                                onComplete:
                                () =>
                                {
                                    this.updateFillRoutine = FrigidCoroutine.Run(UpdateFill(weaponSocket), this.gameObject);
                                }
                                ),
                            this.gameObject
                            );
                    }
                    ),
                this.gameObject
                );
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void StopTransitionsAndUpdate()
        {
            FrigidCoroutine.Kill(this.transitionRoutine);
            FrigidCoroutine.Kill(this.updateAmmoCountRoutine);
            FrigidCoroutine.Kill(this.updateFillRoutine);
        }

        private IEnumerator<FrigidCoroutine.Delay> UpdateFill(WeaponSocket weaponSocket)
        {
            while (true)
            {
                this.fillImage.fillAmount = weaponSocket.SpawnedWeapon.Cooldown.GetProgress();
                yield return null;
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> UpdateAmmoCount(WeaponSocket weaponSocket)
        {
            while (true)
            {
                this.ammoCountText.text = weaponSocket.SpawnedWeapon.Cooldown.GetAccumulation().ToString();
                yield return null;
            }
        }
        */
    }
}
