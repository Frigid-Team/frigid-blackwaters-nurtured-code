using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class PassivePileTimer : Timer
    {
        [SerializeField]
        private IntSerializedReference maxPileCount;
        [SerializeField]
        private IntSerializedReference startingPileCount;
        [SerializeField]
        private IntSerializedReference pileCountReplenished;
        [SerializeField]
        private FloatSerializedReference cooldownDuration;
        [SerializeField]
        private FloatSerializedReference replenishDuration;
        [SerializeField]
        private bool refillInUse;

        private int currentPileCount;
        private float currentReplenishDuration;
        private float nextReplenishDurationThreshold;
        private float currentCooldownDuration;
        private float nextCooldownDurationThreshold;

        public override int Quantity
        {
            get
            {
                return this.currentPileCount;
            }
        }

        public override float Progress
        {
            get
            {
                if (this.currentPileCount == 0)
                {
                    return this.nextReplenishDurationThreshold == 0 ? 1 : Mathf.Clamp01(this.currentReplenishDuration / this.nextReplenishDurationThreshold);
                }
                else
                {
                    return this.nextCooldownDurationThreshold == 0 ? 1 : Mathf.Clamp01(this.currentCooldownDuration / this.nextCooldownDurationThreshold);
                }
            }
        }

        public override bool Available
        {
            get
            {
                return this.currentPileCount > 0 && this.Progress >= 1.0f;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentPileCount = this.startingPileCount.MutableValue;
            this.currentReplenishDuration = 0;
            this.nextReplenishDurationThreshold = 0;
            if (this.currentPileCount < this.maxPileCount.ImmutableValue)
            {
                this.nextReplenishDurationThreshold = Mathf.Max(0, this.replenishDuration.MutableValue);
            }
            this.currentCooldownDuration = 0;
            this.nextCooldownDurationThreshold = 0;
        }

        protected override void Use()
        {
            base.Use();
            if (this.currentPileCount > 0)
            {
                this.currentPileCount--;
                this.currentReplenishDuration = 0;
                this.nextReplenishDurationThreshold = this.replenishDuration.MutableValue;
                this.currentCooldownDuration = 0;
                this.nextCooldownDurationThreshold = this.cooldownDuration.MutableValue;
            }
        }

        protected override void UpdateInUse()
        {
            base.UpdateInUse();
            if (this.refillInUse) UpdatePileRefill();
        }

        protected override void UpdateOutOfUse()
        {
            base.UpdateOutOfUse();
            UpdatePileRefill();
        }

        private void UpdatePileRefill()
        {
            if (this.currentPileCount < this.maxPileCount.ImmutableValue)
            {
                if (this.currentReplenishDuration < this.nextReplenishDurationThreshold)
                {
                    this.currentReplenishDuration += Time.deltaTime * this.LocalTimeScale;
                }
                else
                {
                    this.currentPileCount = Mathf.Min(this.maxPileCount.ImmutableValue, this.currentPileCount + this.pileCountReplenished.MutableValue);
                    this.currentReplenishDuration = 0;
                    this.nextReplenishDurationThreshold = this.replenishDuration.MutableValue;
                }
            }

            if (this.currentCooldownDuration < this.nextCooldownDurationThreshold)
            {
                this.currentCooldownDuration += Time.deltaTime * this.LocalTimeScale;
            }
        }
    }
}
