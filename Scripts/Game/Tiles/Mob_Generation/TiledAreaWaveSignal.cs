using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledAreaWaveSignal : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private string windUpAnimationName;
        [SerializeField]
        private string windDownAnimationName;

        public void DoSignal(Vector2 absolutePosition, float delayDuration, Action onWindupFinished, Action onComplete)
        {
            this.transform.position = absolutePosition;
            FrigidCoroutine.Run(Signal(delayDuration, onWindupFinished, onComplete), this.gameObject);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private IEnumerator<FrigidCoroutine.Delay> Signal(float delayDuration, Action onWindupFinished, Action onComplete)
        {
            this.animatorBody.Active = false;
            yield return new FrigidCoroutine.DelayForSeconds(delayDuration);

            this.animatorBody.Active = true;
            bool windUpFinished = false;
            this.animatorBody.PlayByName(this.windUpAnimationName, () => { windUpFinished = true; });
            yield return new FrigidCoroutine.DelayUntil(() => { return windUpFinished; });

            onWindupFinished.Invoke();

            bool windDownFinished = false;
            this.animatorBody.PlayByName(this.windDownAnimationName, () => { windDownFinished = true; });
            yield return new FrigidCoroutine.DelayUntil(() => { return windDownFinished; });

            this.animatorBody.Active = false;
            onComplete.Invoke();
        }
    }
}
