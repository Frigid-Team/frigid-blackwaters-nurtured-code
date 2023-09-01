using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class HealthScreenEffectsHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private Image borderImage;
        [SerializeField]
        private ColorSerializedReference healColor;
        [SerializeField]
        private ColorSerializedReference damageColor;
        [SerializeField]
        private FloatSerializedReference fadeDuration;

        private FrigidCoroutine currentFadeRoutine;

        protected override void Awake()
        {
            base.Awake();
            this.borderImage.color = Color.clear;
        }

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

        private void Startup()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                player.OnHealed += this.ShowHeal;
                player.OnDamaged += this.ShowDamage;
            }
        }

        private void Teardown()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                player.OnHealed -= this.ShowHeal;
                player.OnDamaged -= this.ShowDamage;
            }
        }

        private void ShowHeal(int heal)
        {
            if (heal == 0) return;

            Color healColor = this.healColor.MutableValue;
            FrigidCoroutine.Kill(this.currentFadeRoutine);
            this.currentFadeRoutine = FrigidCoroutine.Run(Tween.Value(this.fadeDuration.MutableValue, 1, 0, useRealTime: true, onValueUpdated: (float alpha) => this.borderImage.color = new Color(healColor.r, healColor.g, healColor.b, alpha)));
        }

        private void ShowDamage(int damage)
        {
            if (damage == 0) return;

            Color damageColor = this.damageColor.MutableValue;
            FrigidCoroutine.Kill(this.currentFadeRoutine);
            this.currentFadeRoutine = FrigidCoroutine.Run(Tween.Value(this.fadeDuration.MutableValue, 1, 0, useRealTime: true, onValueUpdated: (float alpha) => this.borderImage.color = new Color(damageColor.r, damageColor.g, damageColor.b, alpha)));
        }
    }
}
