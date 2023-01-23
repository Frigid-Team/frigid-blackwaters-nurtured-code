using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class MoveByDashing : MoveForDuration
    {
        [SerializeField]
        private Direction dashWindupDirection;
        [SerializeField]
        private AnimationCurveSerializedReference speedCurve;
        [SerializeField]
        private FloatSerializedReference speed;

        private float speedMultiplier;
        private Vector2 dashDirection;

        public Vector2 DashDirection
        {
            get
            {
                return this.dashDirection;
            }
        }

        protected override void StartedMoving(Vector2 movementPosition, float speedBonus)
        {
            base.StartedMoving(movementPosition, speedBonus);
            this.speedMultiplier = Mathf.Max(this.speed.ImmutableValue + speedBonus, 0) / Calculus.Integral(this.speedCurve.ImmutableValue.Evaluate, 0, 1, 10);
            this.dashDirection = Vector2.zero;
        }

        protected override float GetDuration(Vector2 movementPosition, float speedBonus)
        {
            float dashSpeed = Mathf.Max(this.speed.ImmutableValue + speedBonus, 0);
            return dashSpeed != 0 ? GetMovementDistance(dashSpeed) / dashSpeed : float.MaxValue;
        }

        protected override void DurationStarted(float elapsedDuration)
        {
            base.DurationStarted(elapsedDuration);
            this.dashDirection = this.dashWindupDirection.Calculate(this.dashDirection, elapsedDuration, 0);
        }

        protected abstract float GetMovementDistance(float dashSpeed);

        protected override Vector2 GetVelocityDuringDuration(float normalizedProgress)
        {
            return this.dashDirection * this.speedCurve.ImmutableValue.Evaluate(Mathf.Clamp01(normalizedProgress)) * this.speedMultiplier;
        }
    }
}