using UnityEngine;
using System;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class MoveForDuration : Move
    {
        [SerializeField]
        private FloatSerializedReference delayDuration;
        private bool inDuration;
        private float movementDuration;

        private Action onDurationStarted;
        private Action onDurationFinished;

        public Action OnDurationStarted
        {
            get
            {
                return this.onDurationStarted;
            }
            set
            {
                this.onDurationStarted = value;
            }
        }

        public Action OnDurationFinished
        {
            get
            {
                return this.onDurationFinished;
            }
            set
            {
                this.onDurationFinished = value;
            }
        }

        public float TotalDuration
        {
            get
            {
                return this.movementDuration + this.delayDuration.ImmutableValue;
            }
        }

        protected override void StartedMoving(Vector2 movementPosition, float speedBonus)
        {
            base.StartedMoving(movementPosition, speedBonus);
            this.inDuration = false;
            this.movementDuration = GetDuration(movementPosition, speedBonus);
        }

        protected override void StoppedMoving()
        {
            base.StoppedMoving();
            if (this.inDuration)
            {
                this.inDuration = false;
                this.onDurationFinished?.Invoke();
            }
            this.movementDuration = 0;
        }

        protected override void ContinueMoving(Vector2 movementPosition, float elapsedDuration, float elapsedDurationDelta, float speedBonus)
        {
            base.ContinueMoving(movementPosition, elapsedDuration, elapsedDurationDelta, speedBonus);
            
            if (elapsedDuration <= this.movementDuration + this.delayDuration.ImmutableValue)
            {
                if (!this.inDuration)
                {
                    this.inDuration = true;
                    DurationStarted(elapsedDuration);
                    this.onDurationStarted?.Invoke();
                }
                return;
            }

            if (this.inDuration)
            {
                this.inDuration = false;
                DurationFinished(elapsedDuration);
                this.onDurationFinished?.Invoke();
            }
        }

        protected override Vector2 GetVelocity(Vector2 movementPosition, float elapsedDuration, float elapsedDurationDelta, float speedBonus)
        {
            return this.inDuration ? GetVelocityDuringDuration((elapsedDuration - this.delayDuration.ImmutableValue) / this.movementDuration) : Vector2.zero;
        }

        protected abstract float GetDuration(Vector2 movementPosition, float speedBonus);

        protected virtual void DurationStarted(float elapsedDuration) { }

        protected virtual void DurationFinished(float elapsedDuration) { }

        protected abstract Vector2 GetVelocityDuringDuration(float normalizedProgress); 
    }
}
