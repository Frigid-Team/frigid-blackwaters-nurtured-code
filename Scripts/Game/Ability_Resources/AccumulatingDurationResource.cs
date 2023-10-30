using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AccumulatingDurationResource : AbilityResource
    {
        [SerializeField]
        private IntSerializedReference maxCount;
        [SerializeField]
        private IntSerializedReference startingCount;
        [SerializeField]
        private IntSerializedReference replenishCount;
        [SerializeField]
        private FloatSerializedReference cooldownDuration;
        [SerializeField]
        private FloatSerializedReference replenishDuration;
        [SerializeField]
        private FloatSerializedReference initialElapsedReplenishDuration;
        [SerializeField]
        private bool refillInUse;

        private int currentCount;
        private float currentReplenishDuration;
        private float nextReplenishDurationThreshold;
        private float currentCooldownDuration;
        private float nextCooldownDurationThreshold;

        public override int Quantity
        {
            get
            {
                return this.currentCount;
            }
        }

        public override float Progress
        {
            get
            {
                if (this.currentCount == 0)
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
                return this.currentCount > 0 && this.Progress >= 1.0f;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentCount = this.startingCount.MutableValue;
            this.currentReplenishDuration = 0;
            this.nextReplenishDurationThreshold = 0;
            if (this.currentCount < this.maxCount.ImmutableValue)
            {
                this.nextReplenishDurationThreshold = Mathf.Max(0, this.replenishDuration.MutableValue - this.initialElapsedReplenishDuration.MutableValue);
            }
            this.currentCooldownDuration = 0;
            this.nextCooldownDurationThreshold = 0;
        }

        protected override void Use()
        {
            base.Use();
            if (this.currentCount > 0)
            {
                this.currentCount--;
                this.currentReplenishDuration = 0;
                this.nextReplenishDurationThreshold = this.replenishDuration.MutableValue;
                this.currentCooldownDuration = 0;
                this.nextCooldownDurationThreshold = this.cooldownDuration.MutableValue;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (this.refillInUse || !this.InUse)
            {
                if (this.currentCount < this.maxCount.ImmutableValue)
                {
                    if (this.currentReplenishDuration < this.nextReplenishDurationThreshold)
                    {
                        this.currentReplenishDuration += this.LocalDeltaTime;
                    }
                    else
                    {
                        this.currentCount = Mathf.Min(this.maxCount.ImmutableValue, this.currentCount + this.replenishCount.MutableValue);
                        this.currentReplenishDuration = 0;
                        this.nextReplenishDurationThreshold = this.replenishDuration.MutableValue;
                    }
                }

                if (this.currentCooldownDuration < this.nextCooldownDurationThreshold)
                {
                    this.currentCooldownDuration += this.LocalDeltaTime;
                }
            }
        }
    }
}
