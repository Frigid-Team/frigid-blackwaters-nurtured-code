namespace FrigidBlackwaters.Game
{
    public class LocalRotationInterpAnimatorProperty : InterpAnimatorProperty<AnimatorProperty>
    {
        private float startLocalRotation;
        private float finishLocalRotation;

        public float StartLocalRotation
        {
            get
            {
                return this.startLocalRotation;
            } 
            set
            {
                if (this.startLocalRotation != value)
                {
                    this.startLocalRotation = value;
                    this.InterpolateValuesInProperties();
                }
            }
        }

        public float FinishLocalRotation
        {
            get
            {
                return this.finishLocalRotation;
            }
            set
            {
                if (this.finishLocalRotation != value)
                {
                    this.finishLocalRotation = value;
                    this.InterpolateValuesInProperties();
                }
            }
        }

        protected override void InterpolateValue(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex, float progress01)
        {
            this.GetParameteredProperty(propertyIndex).SetLocalRotation(animationIndex, frameIndex, orientationIndex, this.StartLocalRotation + (this.FinishLocalRotation - this.StartLocalRotation) * progress01);
        }
    }
}
