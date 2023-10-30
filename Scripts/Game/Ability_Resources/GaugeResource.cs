using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class GaugeResource : AbilityResource
    {
        [SerializeField]
        private FloatSerializedReference onUseDelta;
        [SerializeField]
        private FloatSerializedReference onUnuseDelta;
        [SerializeField]
        private FloatSerializedReference gainRate;
        [SerializeField]
        private FloatSerializedReference drainRate;

        private float currentAmount;

        public override int Quantity
        {
            get
            {
                return -1;
            }
        }

        public override float Progress
        {
            get
            {
                return this.currentAmount;
            }
        }

        public override bool Available 
        {
            get
            {
                return this.currentAmount > 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentAmount = 0;
        }

        protected override void Use()
        {
            base.Use();
            this.currentAmount = Mathf.Clamp01(this.currentAmount + this.onUseDelta.MutableValue);
        }

        protected override void Unuse()
        {
            base.Unuse();
            this.currentAmount = Mathf.Clamp01(this.currentAmount + this.onUnuseDelta.MutableValue);
        }

        protected override void Update()
        {
            base.Update();
            if (this.InUse)
            {
                this.currentAmount = Mathf.Max(0, this.currentAmount - this.drainRate.ImmutableValue * this.LocalDeltaTime);
            }
            else
            {
                this.currentAmount = Mathf.Min(1f, this.currentAmount + this.gainRate.ImmutableValue * this.LocalDeltaTime);
            }
        }
    }
}
