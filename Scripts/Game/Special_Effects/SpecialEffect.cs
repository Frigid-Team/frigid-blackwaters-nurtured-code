namespace FrigidBlackwaters.Game
{
    public abstract class SpecialEffect : FrigidMonoBehaviour
    {
        private bool isPlaying;

        public void Play(float effectsTimeScaling = 1f)
        {
            if (this.isPlaying)
            {
                Stop();
            }
            Played(effectsTimeScaling);
            this.isPlaying = true;
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
        }

        protected abstract void Played(float effectsTimeScaling = 1f);

        protected abstract void Stopped();
    }
}
