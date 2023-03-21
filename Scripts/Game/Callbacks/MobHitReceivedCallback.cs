using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobHitReceivedCallback : Callback
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private IntSerializedReference numberHitsRequired;
        [SerializeField]
        private FloatSerializedReference chanceOfNotOccuring;
        [SerializeField]
        private IntSerializedReference minimumDamage;

        private Dictionary<Action, Action<HitInfo>> translatedListeners;
 
        public override void RegisterListener(Action onInvoked)
        {
            Action<HitInfo> translatedListener = 
                (HitInfo hitInfo) =>
                {
                    if ((this.numberHitsRequired.ImmutableValue == 0 || this.mob.ImmutableValue.HitsReceived.Count % this.numberHitsRequired.ImmutableValue == 0) &&
                        UnityEngine.Random.Range(0f, 1f) >= this.chanceOfNotOccuring.ImmutableValue &&
                        hitInfo.Damage >= this.minimumDamage.ImmutableValue)
                    {
                        onInvoked?.Invoke();
                    }
                };
            if (this.translatedListeners.TryAdd(onInvoked, translatedListener))
            {
                this.mob.ImmutableValue.OnHitReceived += translatedListener;
            }
        }

        public override void ClearListener(Action onInvoked)
        {
            if (this.translatedListeners.ContainsKey(onInvoked))
            {
                this.mob.ImmutableValue.OnHitReceived -= this.translatedListeners[onInvoked];
                this.translatedListeners.Remove(onInvoked);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.translatedListeners = new Dictionary<Action, Action<HitInfo>>();
        }
    }
}
