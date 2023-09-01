using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MoveByOtherForDuration : MoveForDuration
    {
        [SerializeField]
        private Move subMove;
        [SerializeField]
        private FloatSerializedReference duration;

        protected override void DurationStarted()
        {
            base.DurationStarted();
            this.Mover.AddMove(this.subMove, this.Mover.GetPriority(this), this.Mover.GetIsIgnoringTimeScale(this));
        }

        protected override void DurationFinished()
        {
            base.DurationFinished();
            this.Mover.RemoveMove(this.subMove);
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
                return Vector2.zero;
            }
        }
    }
}
