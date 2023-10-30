using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class LocalPositionInterpAnimatorProperty : InterpAnimatorProperty<AnimatorProperty>
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
                    this.InterpolateValuesInProperties();
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
                    this.InterpolateValuesInProperties();
                }
            }
        }

        protected override void InterpolateValue(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex, float progress01)
        {
            this.GetParameteredProperty(propertyIndex).SetLocalPosition(animationIndex, frameIndex, orientationIndex, this.StartLocalPosition + (this.FinishLocalPosition - this.StartLocalPosition) * progress01);
        }
    }
}
