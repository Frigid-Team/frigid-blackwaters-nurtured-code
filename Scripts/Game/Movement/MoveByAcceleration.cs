using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MoveByAcceleration : Move
    {
        private float buildupDuration;

        [SerializeField]
        private Direction moveDirection;
        [SerializeField]
        private FloatSerializedReference speed;
        [SerializeField]
        private AnimationCurveSerializedReference accelerationCurve;
        [SerializeField]
        private FloatSerializedReference accelerationRate;

        private Vector2 currentMoveDirection;
        private Vector2 previousNonZeroMoveDirection;

        public override Vector2 Velocity
        {
            get
            {
                return this.previousNonZeroMoveDirection * Mathf.Max(this.speed.ImmutableValue + this.Mover.SpeedBonus, 0) * this.accelerationCurve.ImmutableValue.Evaluate(this.buildupDuration);
            }
        }

        public override void StartMoving()
        {
            base.StartMoving();
            this.buildupDuration = 0;
            this.currentMoveDirection = Vector2.zero;
            this.previousNonZeroMoveDirection = Vector2.zero;
        }

        public override void ContinueMovement()
        {
            base.ContinueMovement();
            this.currentMoveDirection = this.moveDirection.Retrieve(this.currentMoveDirection, this.MovingDuration, this.MovingDurationDelta);
            this.buildupDuration = this.ModifyBuildupDuration(this.buildupDuration, this.currentMoveDirection);
            if (this.currentMoveDirection.magnitude > 0)
            {
                this.previousNonZeroMoveDirection = this.currentMoveDirection;
                this.buildupDuration += this.MovingDurationDelta * this.accelerationRate.ImmutableValue;
            }
            else
            {
                this.buildupDuration -= this.MovingDurationDelta * this.accelerationRate.ImmutableValue;
            }
            this.buildupDuration = Mathf.Clamp(this.buildupDuration, 0, 1);
        }

        protected virtual float ModifyBuildupDuration(float buildupDuration, Vector2 movementDirection) { return buildupDuration; }
    }
}