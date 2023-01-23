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

        protected override void StartedMoving(Vector2 movementPosition, float speedBonus)
        {
            base.StartedMoving(movementPosition, speedBonus);
            this.buildupDuration = 0;
            this.currentMoveDirection = Vector2.zero;
            this.previousNonZeroMoveDirection = Vector2.zero;
        }

        protected override void ContinueMoving(Vector2 movementPosition, float elapsedDuration, float elapsedDurationDelta, float speedBonus)
        {
            base.ContinueMoving(movementPosition, elapsedDuration, elapsedDurationDelta, speedBonus);
            this.currentMoveDirection = this.moveDirection.Calculate(this.currentMoveDirection, elapsedDuration, elapsedDurationDelta);
            this.buildupDuration = ModifyBuildupDuration(this.buildupDuration, this.currentMoveDirection);
            if (this.currentMoveDirection.magnitude > 0)
            {
                this.previousNonZeroMoveDirection = this.currentMoveDirection;
                this.buildupDuration += Time.deltaTime * this.accelerationRate.ImmutableValue;
            }
            else
            {
                this.buildupDuration -= Time.deltaTime * this.accelerationRate.ImmutableValue;
            }
            this.buildupDuration = Mathf.Clamp(this.buildupDuration, 0, 1);
        }

        protected virtual float ModifyBuildupDuration(float buildupDuration, Vector2 movementDirection) { return buildupDuration; }

        protected override Vector2 GetVelocity(Vector2 movementPosition, float elapsedDuration, float elapsedDurationDelta, float speedBonus)
        {
            return this.previousNonZeroMoveDirection * Mathf.Max(this.speed.ImmutableValue + speedBonus, 0) * this.accelerationCurve.ImmutableValue.Evaluate(this.buildupDuration);
        }
    }
}