using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class MoveForDuration : Move
    {
        [SerializeField]
        private FloatSerializedReference delayDuration;
        private bool inDuration;

        public override bool IsFinished
        {
            get
            {
                return this.MovingDuration >= this.TotalDuration;
            }
        }

        public override bool IsInMotion
        {
            get
            {
                return this.inDuration;
            }
        }

        public override Vector2 Velocity
        {
            get
            {
                return this.inDuration ? this.VelocityDuringDuration : Vector2.zero;
            }
        }

        public float TotalDuration
        {
            get
            {
                return this.CustomDuration + this.DelayDuration;
            }
        }

        public override void StartMoving()
        {
            base.StartMoving();
            this.inDuration = false;
            if (this.MovingDuration < this.DelayDuration)
            {
                return;
            }
            if (this.MovingDuration < this.TotalDuration)
            {
                this.inDuration = true;
                DurationStarted();
                return;
            }
            DurationStarted();
            DurationFinished();
        }

        public override void StopMoving()
        {
            base.StopMoving();
            this.inDuration = false;
        }

        public override void ContinueMovement()
        {
            base.ContinueMovement();
            if (this.MovingDuration < this.DelayDuration)
            {
                return;
            }
            if (this.MovingDuration < this.TotalDuration)
            {
                if (!this.inDuration)
                {
                    this.inDuration = true;
                    DurationStarted();
                }
                return;
            }
            if (this.inDuration)
            {
                this.inDuration = false;
                DurationFinished();
            }
        }

        protected float DelayDuration
        {
            get
            {
                return this.delayDuration.ImmutableValue;
            }
        }

        protected abstract float CustomDuration
        {
            get;
        }

        protected abstract Vector2 VelocityDuringDuration
        {
            get;
        }

        protected virtual void DurationStarted() { }

        protected virtual void DurationFinished() { }
    }
}
