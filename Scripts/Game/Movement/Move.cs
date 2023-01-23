using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Move : FrigidMonoBehaviour
    {
        private float elapsedDuration;
        private float elapsedDurationDelta;
        private Vector2 velocity;

        public Vector2 Velocity
        {
            get
            {
                return this.velocity;
            }
        }

        public void StartMoving(Vector2 movementPosition, float speedBonus) 
        { 
            this.elapsedDuration = 0;
            this.elapsedDurationDelta = 0;
            StartedMoving(movementPosition, speedBonus);
            this.velocity = GetVelocity(movementPosition, this.elapsedDuration, this.elapsedDurationDelta, speedBonus);
        }

        public void StopMoving() 
        {
            StoppedMoving();
            this.velocity = Vector2.zero;
        }

        public void ContinueMovement(Vector2 movementPosition, float speedBonus)
        {
            this.elapsedDuration += Time.deltaTime;
            this.elapsedDurationDelta = Time.deltaTime;
            ContinueMoving(movementPosition, this.elapsedDuration, this.elapsedDurationDelta, speedBonus);
            this.velocity = GetVelocity(movementPosition, this.elapsedDuration, this.elapsedDurationDelta, speedBonus);
        }

        protected virtual void StartedMoving(Vector2 movementPosition, float speedBonus) { }

        protected virtual void StoppedMoving() { }

        protected virtual void ContinueMoving(Vector2 movementPosition, float elapsedDuration, float elapsedDurationDelta, float speedBonus) { }

        protected abstract Vector2 GetVelocity(Vector2 movementPosition, float elapsedDuration, float elapsedDurationDelta, float speedBonus);
    }
}
