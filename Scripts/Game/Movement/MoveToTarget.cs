using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MoveToTarget : MoveForDuration
    {
        [SerializeField]
        private Targeter destinationTargeter;
        [SerializeField]
        private AnimationCurveSerializedReference speedCurve;
        [SerializeField]
        private FloatSerializedReference duration;

        private Vector2 destinationPosition;
        private Vector2 moveDirection;
        private float moveDistance;

        public Vector2 DestinationPosition
        {
            get
            {
                return this.destinationPosition;
            }
        }

        public override void StartMoving()
        {
            base.StartMoving();
            this.destinationPosition = this.destinationTargeter.Calculate(this.Mover.Position, this.MovingDuration, this.MovingDurationDelta);
            this.moveDirection = (this.destinationPosition - this.Mover.Position).normalized;
            this.moveDistance = Vector2.Distance(this.Mover.Position, this.destinationPosition);
        }

        protected override float CustomDuration
        {
            get
            {
                return this.duration.ImmutableValue;
            }
        }

        protected override Vector2 VelocityDuringDuration
        {
            get
            {
                return
                    this.moveDirection *
                    this.speedCurve.ImmutableValue.Evaluate(Mathf.Clamp01((this.MovingDuration - this.DelayDuration) / this.CustomDuration)) *
                    this.moveDistance / this.CustomDuration / Calculus.Integral(this.speedCurve.ImmutableValue.Evaluate, 0, 1, 10);
            }
        }
    }
}
