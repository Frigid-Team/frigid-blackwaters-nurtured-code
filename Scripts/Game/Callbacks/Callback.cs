using System;

namespace FrigidBlackwaters.Game
{
    public abstract class Callback : FrigidMonoBehaviour
    {
        public abstract void RegisterListener(Action onInvoked);

        public abstract void ClearListener(Action onInvoked);
    }
}
