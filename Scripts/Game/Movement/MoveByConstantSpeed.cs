using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MoveByConstantSpeed : Move
    {
        [SerializeField]
        private Direction moveDirection;
        [SerializeField]
        private FloatSerializedReference speed;

        private Vector2 currentMoveDirection;

        public override Vector2 Velocity
        {
            get
            {
                this.currentMoveDirection = this.moveDirection.Calculate(this.currentMoveDirection, this.MovingDuration, this.MovingDurationDelta);
                return this.currentMoveDirection * Mathf.Max(this.speed.ImmutableValue + this.Mover.SpeedBonus, 0);
            }
        }

        public override void StartMoving()
        {
            base.StartMoving();
            this.currentMoveDirection = Vector2.zero;
        }
    }
}
