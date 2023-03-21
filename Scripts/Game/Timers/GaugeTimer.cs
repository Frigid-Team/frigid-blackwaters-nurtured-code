using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class GaugeTimer : Timer
    {
        [SerializeField]
        private FloatSerializedReference gainRate;
        [SerializeField]
        private FloatSerializedReference drainRate;

        private float currentAmount;

        public override int Quantity
        {
            get
            {
                return this.currentAmount > 0 ? 1 : 0;
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

        protected override void UpdateInUse()
        {
            base.UpdateInUse();
            this.currentAmount = Mathf.Max(0, this.currentAmount - this.drainRate.ImmutableValue * Time.deltaTime * this.LocalTimeScale);
        }

        protected override void UpdateOutOfUse()
        {
            base.UpdateOutOfUse();
            this.currentAmount = Mathf.Min(1f, this.currentAmount + this.gainRate.ImmutableValue * Time.deltaTime * this.LocalTimeScale);
        }
    }
}
