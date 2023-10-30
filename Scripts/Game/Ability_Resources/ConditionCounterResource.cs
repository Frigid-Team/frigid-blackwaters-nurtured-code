using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class ConditionCounterResource : AbilityResource
    {
        [SerializeField]
        private IntSerializedReference maxCount;
        [SerializeField]
        private IntSerializedReference startingCount;
        [SerializeField]
        private bool countUpOnCondition;
        [SerializeField]
        [ShowIfBool("countUpOnCondition", true)]
        private Conditional countUpCondition;
        [SerializeField]
        private bool countDownOnCondition;
        [SerializeField]
        [ShowIfBool("countDownOnCondition", true)]
        private Conditional countDownCondition;

        private int currentCount;
        private float durationSinceLastCounter;

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
                return ((float)this.currentCount) / this.maxCount.ImmutableValue;
            }
        }

        public override bool Available
        {
            get
            {
                return this.currentCount > 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentCount = this.startingCount.MutableValue;
            this.durationSinceLastCounter = 0f;
        }

        protected override void Use()
        {
            base.Use();
            this.currentCount = this.startingCount.MutableValue;
            this.durationSinceLastCounter = 0f;
        }

        protected override void Update()
        {
            base.Update();
            this.durationSinceLastCounter += this.LocalDeltaTime;
            if (this.countUpOnCondition)
            {
                int newCount = Mathf.Min(this.currentCount + this.countUpCondition.Tally(this.durationSinceLastCounter, this.LocalDeltaTime), this.maxCount.ImmutableValue);
                if (newCount > this.currentCount)
                {
                    this.currentCount = newCount;
                    this.durationSinceLastCounter = 0f;
                }
            }
            if (this.countDownOnCondition)
            {
                int newCount = Mathf.Max(this.currentCount - this.countDownCondition.Tally(this.durationSinceLastCounter, this.LocalDeltaTime), 0);
                if (newCount < this.currentCount)
                {
                    this.currentCount = newCount;
                    this.durationSinceLastCounter = 0f;
                }
            }
        }
    }
}
