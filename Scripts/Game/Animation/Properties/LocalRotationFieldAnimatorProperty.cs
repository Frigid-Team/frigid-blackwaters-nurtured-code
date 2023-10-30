namespace FrigidBlackwaters.Game
{
    public class LocalRotationFieldAnimatorProperty : OrientationFieldAnimatorProperty<AnimatorProperty>
    {
        private float localRotation;
        
        public float LocalRotation
        {
            get
            {
                return this.localRotation;
            }
            set
            {
                if (this.localRotation != value)
                {
                    this.localRotation = value;
                    this.SetValuesInProperties();
                }
            }
        }

        protected override void SetValue(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex)
        {
            this.GetParameteredProperty(propertyIndex).SetLocalRotation(animationIndex, frameIndex, orientationIndex, this.LocalRotation);
        }
    }
}
