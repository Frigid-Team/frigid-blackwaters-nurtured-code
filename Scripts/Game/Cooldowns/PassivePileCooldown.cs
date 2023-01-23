using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class PassivePileCooldown : Cooldown
    {
        [SerializeField]
        private IntSerializedReference maxPileCount;
        [SerializeField]
        private IntSerializedReference startingPileCount;
        [SerializeField]
        private FloatSerializedReference betweenPileDuration;
        [SerializeField]
        private FloatSerializedReference replenishDuration;

        private float lastPausedTime;
        private int currentPileCount;
        private float lastResetTime;
        private float lastReplenishTime;

        public override int GetAccumulation()
        {
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
                return Mathf.Clamp01((this.LastActiveTime - this.lastReplenishTime) / this.replenishDuration.ImmutableValue);
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
                return this.LastActiveTime - this.lastReplenishTime < this.replenishDuration.ImmutableValue;
            }
        }

        public override void ResetCooldown()
        {
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
            this.lastReplenishTime = Time.time;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.lastResetTime = Time.time - (this.lastPausedTime - this.lastResetTime);
            this.lastReplenishTime = Time.time - (this.lastPausedTime - this.lastReplenishTime);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.lastPausedTime = Time.time;
        }

        protected override void Update()
        {
            base.Update();
            if (this.currentPileCount < this.maxPileCount.ImmutableValue) 
            {
                if (Time.time - this.lastReplenishTime > this.replenishDuration.ImmutableValue)
                {
                    this.currentPileCount++;
                    this.lastReplenishTime = Time.time;
                }
            }
            else
            {
                this.lastReplenishTime = Time.time;
            }
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
