using System;

namespace FrigidBlackwaters.Core
{
    public class FlagSemaphore
    {
        private bool isSet;
        private Action onSet;
        private Action onUnset;


        public static implicit operator bool(FlagSemaphore semaphore)
        {
            return semaphore.isSet;
        }

        public Action OnSet
        {
            get
            {
                return this.onSet;
            }
            set
            {
                this.onSet = value;
            }
        }

        public Action OnUnset
        {
            get
            {
                return this.onUnset;
            }
            set
            {
                this.onUnset = value;
            }
        }

        public void Set()
        {
            if (!this.isSet)
            {
                this.isSet = true;
                this.onSet?.Invoke();
            }
        }

        public void Unset()
        {
            if (this.isSet)
            {
                this.isSet = false;
                this.onUnset?.Invoke();
            }
        }
    }
}
