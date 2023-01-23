using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DurationCooldown : Cooldown
    {
        [SerializeField]
        private FloatSerializedReference cooldownDuration;
        [SerializeField]
        private FloatSerializedReference initialElapsedCooldownDuration;

        private float lastPausedTime;
        private float lastResetTime;

        public override float GetProgress()
        {
            return this.cooldownDuration.ImmutableValue <= 0 ? 1 : Mathf.Clamp01((this.LastActiveTime - this.lastResetTime) / this.cooldownDuration.ImmutableValue);
        }

        public override bool OnCooldown()
        {
            return this.LastActiveTime - this.lastResetTime < this.cooldownDuration.ImmutableValue;
        }

        public override void ResetCooldown()
        {
            this.lastResetTime = this.LastActiveTime;
        }

        protected override void Awake()
        {
            base.Awake();
            this.lastPausedTime = Time.time;
            this.lastResetTime = Time.time - this.initialElapsedCooldownDuration.MutableValue;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.lastResetTime = Time.time - (this.lastPausedTime - this.lastResetTime);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.lastPausedTime = Time.time;
        }

        private float LastActiveTime
        {
            get
            {
                return this.isActiveAndEnabled ? Time.time : this.lastPausedTime;
            }
        }
    }
}
