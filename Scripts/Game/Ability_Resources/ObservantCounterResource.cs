using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class ObservantCounterResource : AbilityResource
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
        [SerializeField]
        private bool resetOnCondition;
        [SerializeField]
        [ShowIfBool("resetOnCondition", true)]
        private Conditional resetCondition;

        private int currentCount;

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

        protected override void Use()
        {
            base.Use();
            this.currentCount = this.startingCount.MutableValue;
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentCount = this.startingCount.MutableValue;
        }

        protected override void Update()
        {
            base.Update();
            if (this.countUpOnCondition) this.currentCount = Mathf.Min(this.currentCount + this.countUpCondition.Tally(0, 0), this.maxCount.ImmutableValue);
            if (this.countDownOnCondition) this.currentCount = Mathf.Max(this.currentCount - this.countDownCondition.Tally(0, 0), 0);
            if (this.resetOnCondition && this.resetCondition.Evaluate(0, 0))
            {
                this.currentCount = this.startingCount.MutableValue;
            }
        }
    }
}
