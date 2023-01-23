namespace FrigidBlackwaters.Game
{
    public abstract class Cooldown : FrigidMonoBehaviour
    {
        public virtual int GetAccumulation() { return 1; }

        public abstract float GetProgress();

        public abstract void ResetCooldown();

        public abstract bool OnCooldown();
    }
}
