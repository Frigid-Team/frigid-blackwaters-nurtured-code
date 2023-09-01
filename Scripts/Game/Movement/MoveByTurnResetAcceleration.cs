using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MoveByTurnResetAcceleration : MoveByAcceleration
    {
        private Vector2 previousMovementDirection;

        public override void StartMoving()
        {
            base.StartMoving();
            this.previousMovementDirection = Vector2.zero;
        }

        protected override float ModifyBuildupDuration(float buildupDuration, Vector2 movementDirection)
        {
            if (movementDirection.magnitude > 0 && Vector2.Distance(this.previousMovementDirection, movementDirection) > FrigidConstants.SMALLEST_WORLD_SIZE)
            {
                float turnAngle = Mathf.Acos(Mathf.Clamp(Vector2.Dot(this.previousMovementDirection, movementDirection), -1, 1));
                this.previousMovementDirection = movementDirection;
                return buildupDuration * 1 - turnAngle / Mathf.PI;
            }
            return buildupDuration;
        }
    }
}
