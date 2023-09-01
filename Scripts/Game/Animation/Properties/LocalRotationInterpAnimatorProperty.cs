namespace FrigidBlackwaters.Game
{
    public class LocalRotationInterpAnimatorProperty : FrameInterpAnimatorProperty
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
                    this.InterpolateProperties();
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
                    this.InterpolateProperties();
                }
            }
        }

        protected override void InterpolateValue(AnimatorProperty interpolatedProperty, int animationIndex, int frameIndex, int orientationIndex, float progress01)
        {
            interpolatedProperty.SetLocalRotation(animationIndex, frameIndex, orientationIndex, this.StartLocalRotation + (this.FinishLocalRotation - this.StartLocalRotation) * progress01);
        }
    }
}
