using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class LocalPositionInterpAnimatorProperty : FrameInterpAnimatorProperty
    {
        private Vector2 startLocalPosition;
        private Vector2 finishLocalPosition;

        public Vector2 StartLocalPosition 
        {
            get
            {
                return this.startLocalPosition;
            }
            set
            {
                if (this.startLocalPosition != value)
                {
                    this.startLocalPosition = value;
                    this.InterpolateProperties();
                }
            }
        }

        public Vector2 FinishLocalPosition
        {
            get
            {
                return this.finishLocalPosition;
            }
            set
            {
                if (this.finishLocalPosition != value)
                {
                    this.finishLocalPosition = value;
                    this.InterpolateProperties();
                }
            }
        }

        protected override void InterpolateValue(AnimatorProperty interpolatedProperty, int animationIndex, int frameIndex, int orientationIndex, float progress01)
        {
            interpolatedProperty.SetLocalPosition(animationIndex, frameIndex, orientationIndex, this.StartLocalPosition + (this.FinishLocalPosition - this.StartLocalPosition) * progress01);
        }
    }
}
