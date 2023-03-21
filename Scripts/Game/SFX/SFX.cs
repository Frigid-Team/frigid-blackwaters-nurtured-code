namespace FrigidBlackwaters.Game
{
    public abstract class SFX : FrigidMonoBehaviour
    {
        private bool isPlaying;
        private float timeScaling;

        public float TimeScaling
        {
            get
            {
                return this.timeScaling;
            }
            set
            {
                this.timeScaling = value;
            }
        }

        public void Play(AnimatorBody animatorBody)
        {
            if (!this.isPlaying)
            {
                Played(animatorBody);
                this.isPlaying = true;
            }
        }

        public void Stop()
        {
            if (this.isPlaying)
            {
                Stopped();
                this.isPlaying = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.isPlaying = false;
            this.timeScaling = 1f;
        }

        protected abstract void Played(AnimatorBody animatorBody);

        protected abstract void Stopped();
    }
}
