using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DurationResource : AbilityResource
    {
        [SerializeField]
        private FloatSerializedReference refillDuration;
        [SerializeField]
        private FloatSerializedReference initialElapsedRefillDuration;
        [SerializeField]
        private bool refillInUse;

        private float currentRefillDuration;
        private float nextRefillDurationThreshold;

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
                return this.nextRefillDurationThreshold == 0 ? 1 : Mathf.Clamp01(this.currentRefillDuration / this.nextRefillDurationThreshold);
            }
        }

        public override bool Available
        {
            get
            {
                return this.Progress >= 1;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.currentRefillDuration = 0;
            this.nextRefillDurationThreshold = Mathf.Max(0, this.refillDuration.MutableValue - this.initialElapsedRefillDuration.MutableValue);
        }

        protected override void Use()
        {
            base.Use();
            this.currentRefillDuration = 0;
            this.nextRefillDurationThreshold = Mathf.Max(0, this.refillDuration.MutableValue);
        }

        protected override void Update()
        {
            base.Update();
            if (this.refillInUse || !this.InUse)
            {
                if (this.currentRefillDuration < this.nextRefillDurationThreshold)
                {
                    this.currentRefillDuration += this.LocalDeltaTime;
                }
            }
        }
    }
}
