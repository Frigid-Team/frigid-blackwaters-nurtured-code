using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class HealthScreenEffectsHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private CanvasGroup damageCanvasGroup;
        [SerializeField]
        private CanvasGroup healCanvasGroup;
        [SerializeField]
        private FloatSerializedReference fadeDuration;

        private FrigidCoroutine currentFadeRoutine;

        protected override void Awake()
        {
            base.Awake();
            this.damageCanvasGroup.alpha = 0;
            this.healCanvasGroup.alpha = 0;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMob.OnExists += Startup;
            PlayerMob.OnUnexists += Teardown;
            Startup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerMob.OnExists -= Startup;
            PlayerMob.OnUnexists -= Teardown;
            Teardown();
        }

        private void Startup()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                player.OnRemainingHealthChanged += ShowHealthEffects;
            }
        }

        private void Teardown()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                player.OnRemainingHealthChanged -= ShowHealthEffects;
            }
        }

        private void ShowHealthEffects(int prevRemainingHealth, int currRemainingHealth)
        {
            FrigidCoroutine.Kill(this.currentFadeRoutine);
            this.healCanvasGroup.alpha = 0;
            this.damageCanvasGroup.alpha = 0;
            if (currRemainingHealth < prevRemainingHealth)
            {
                this.currentFadeRoutine = FrigidCoroutine.Run(TweenCoroutine.Value(this.fadeDuration.MutableValue, 1, 0, useRealTime: true, onValueUpdated: (float alpha) => this.damageCanvasGroup.alpha = alpha));
            }
            else
            {
                this.currentFadeRoutine = FrigidCoroutine.Run(TweenCoroutine.Value(this.fadeDuration.MutableValue, 1, 0, useRealTime: true, onValueUpdated: (float alpha) => this.healCanvasGroup.alpha = alpha));
            }
        }
    }
}
