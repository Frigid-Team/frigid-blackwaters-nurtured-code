namespace FrigidBlackwaters.Game
{
    public abstract class FrameEventConditional : Conditional
    {
        private int numNotified;
        private int numNoticed;

        protected sealed override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.numNoticed > 0;
        }

        protected sealed override int CustomTally(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.numNoticed;
        }

        protected void Notify()
        {
            this.numNotified++;
        }

        protected override void Awake()
        {
            base.Awake();
            this.numNotified = 0;
            this.numNoticed = 0;
        }

        protected virtual void LateUpdate()
        {
            // When an event is triggered, we need to guarantee that it is can only be read on a single frame.
            // At the end of the frame that an event is triggered, this class records that event to be able to be read the following frame
            // while also clearing notifications from the previous frame.
            // This means that things are detected up to one frame late.. which shouldn't be a problem for code using this class.

            this.numNoticed = this.numNotified;
            this.numNotified = 0;
        }
    }
}
