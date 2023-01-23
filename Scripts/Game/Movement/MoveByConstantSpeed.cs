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

        protected override void StartedMoving(Vector2 movementPosition, float speedBonus)
        {
            base.StartedMoving(movementPosition, speedBonus);
            this.currentMoveDirection = Vector2.zero;
        }

        protected override Vector2 GetVelocity(Vector2 movementPosition, float elapsedDuration, float elapsedDurationDelta, float speedBonus)
        {
            this.currentMoveDirection = this.moveDirection.Calculate(this.currentMoveDirection, elapsedDuration, elapsedDurationDelta);
            return this.currentMoveDirection * Mathf.Max(this.speed.ImmutableValue + speedBonus, 0);
        }
    }
}
