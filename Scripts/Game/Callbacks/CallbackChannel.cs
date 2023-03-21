using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class CallbackChannel
    {
        [SerializeField]
        private List<Callback> callbacks;

        public void RegisterListener(Action onInvoked)
        {
            foreach (Callback callback in this.callbacks)
            {
                callback.RegisterListener(onInvoked);
            }
        }

        public void ClearListener(Action onInvoked)
        {
            foreach (Callback callback in this.callbacks)
            {
                callback.ClearListener(onInvoked);
            }
        }
    }
}
