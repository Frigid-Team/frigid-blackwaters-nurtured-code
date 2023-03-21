using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Move : FrigidMonoBehaviour
    {
        private Mover mover;
        private float movingDuration;
        private float movingDurationDelta;

        public virtual bool IsFinished
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsInMotion
        {
            get
            {
                return true;
            }
        }

        public abstract Vector2 Velocity
        {
            get;
        }

        public void Assign(Mover mover)
        {
            this.mover = mover;
        }

        public void Unassign()
        {
            this.mover = null;
        }

        public virtual void StartMoving() 
        { 
            this.movingDuration = 0;
            this.movingDurationDelta = 0;
        }

        public virtual void StopMoving() { }

        public virtual void ContinueMovement()
        {
            this.movingDurationDelta = Time.fixedDeltaTime * (this.Mover.GetIsIgnoringTimeScale(this) ? 1f : this.Mover.TimeScale);
            this.movingDuration += this.movingDurationDelta;
        }

        protected Mover Mover
        {
            get
            {
                return this.mover;
            }
        }

        protected float MovingDuration
        {
            get
            {
                return this.movingDuration;
            }
        }

        protected float MovingDurationDelta
        {
            get
            {
                return this.movingDurationDelta;
            }
        }
    }
}
