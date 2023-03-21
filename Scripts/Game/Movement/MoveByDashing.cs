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

        private float dashSpeed;
        private float speedMultiplier;
        private Vector2 dashDirection;

        public Vector2 DashDirection
        {
            get
            {
                return this.dashDirection;
            }
        }

        public override void StartMoving()
        {
            this.dashSpeed = Mathf.Max(this.speed.ImmutableValue + this.Mover.SpeedBonus, 0);
            this.speedMultiplier = this.dashSpeed / Calculus.Integral(this.speedCurve.ImmutableValue.Evaluate, 0, 1, 10);
            base.StartMoving();
        }

        protected override float CustomDuration
        {
            get
            {
                return this.dashSpeed != 0 ? GetMovementDistance(this.dashSpeed) / this.dashSpeed : float.PositiveInfinity;
            }
        }

        protected override Vector2 VelocityDuringDuration
        {
            get
            {
                return this.dashDirection * this.speedCurve.ImmutableValue.Evaluate(Mathf.Clamp01((this.MovingDuration - this.DelayDuration) / this.CustomDuration)) * this.speedMultiplier;
            }
        }

        protected override void DurationStarted()
        {
            base.DurationStarted();
            this.dashDirection = this.dashWindupDirection.Calculate(this.dashDirection, this.MovingDuration, 0);
        }

        protected abstract float GetMovementDistance(float dashSpeed);
    }
}