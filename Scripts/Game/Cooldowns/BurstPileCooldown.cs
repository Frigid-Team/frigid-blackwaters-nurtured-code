using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class BurstPileCooldown : Cooldown
    {
        [SerializeField]
        private IntSerializedReference maxPileCount;
        [SerializeField]
        private IntSerializedReference startingPileCount;
        [SerializeField]
        private FloatSerializedReference betweenPileDuration;
        [SerializeField]
        private FloatSerializedReference replenishDuration;
        [SerializeField]
        private FloatSerializedReference initialElapsedReplenishDuration;

        private float lastPausedTime;
        private int currentPileCount;
        private float lastResetTime;

        public override int GetAccumulation()
        {
            if (this.LastActiveTime - this.lastResetTime >= this.replenishDuration.ImmutableValue && this.currentPileCount <= 0)
            {
                return this.maxPileCount.ImmutableValue;
            }
            return this.currentPileCount;
        }

        public override float GetProgress()
        {
            if (this.currentPileCount > 0)
            {
                return Mathf.Clamp01((this.LastActiveTime - this.lastResetTime) / this.betweenPileDuration.ImmutableValue);
            }
            else
            {
                return Mathf.Clamp01((this.LastActiveTime - this.lastResetTime) / this.replenishDuration.ImmutableValue);
            }
        }

        public override bool OnCooldown()
        {
            if (this.currentPileCount > 0)
            {
                return this.LastActiveTime - this.lastResetTime < this.betweenPileDuration.ImmutableValue;
            }
            else
            {
                return this.LastActiveTime - this.lastResetTime < this.replenishDuration.ImmutableValue;
            }
        }

        public override void ResetCooldown()
        {
            this.currentPileCount = GetAccumulation();
            if (this.currentPileCount > 0)
            {
                this.currentPileCount--;
            }
            this.lastResetTime = this.LastActiveTime;
        }

        protected override void Awake()
        {
            base.Awake();
            this.lastPausedTime = Time.time;
            this.currentPileCount = this.startingPileCount.ImmutableValue;
            this.lastResetTime = Time.time;
            if (this.currentPileCount == 0)
            {
                this.lastResetTime = Time.time - this.initialElapsedReplenishDuration.ImmutableValue;
            }
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
