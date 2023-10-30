using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class LocalPositionFieldAnimatorProperty : OrientationFieldAnimatorProperty<AnimatorProperty>
    {
        private Vector2 localPosition;

        public Vector2 LocalPosition
        {
            get
            {
                return this.localPosition;
            }
            set
            {
                if (this.localPosition != value)
                {
                    this.localPosition = value;
                    this.SetValuesInProperties();
                }
            }
        }

        protected override void SetValue(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex)
        {
            this.GetParameteredProperty(propertyIndex).SetLocalPosition(animationIndex, frameIndex, orientationIndex, this.LocalPosition);
        }
    }
}
