using System;

namespace FrigidBlackwaters.Core
{
    public class MultiFlagSemaphore
    {
        private FlagSemaphore[] flags;
        private int numberFlagsSet;
        private Action onAllSet;
        private Action onAnyUnset;

        public static implicit operator bool(MultiFlagSemaphore semaphore)
        {
            return semaphore.numberFlagsSet == semaphore.flags.Length;
        }

        public MultiFlagSemaphore(FlagSemaphore[] flags)
        {
            this.flags = flags;
            foreach (FlagSemaphore flag in this.flags)
            {
                flag.OnSet +=
                    () =>
                    {
                        this.numberFlagsSet++;
                        if (this.numberFlagsSet == this.flags.Length)
                        {
                            this.onAllSet?.Invoke();
                        }
                    };
                flag.OnUnset +=
                    () =>
                    {
                        this.numberFlagsSet--;
                        if (this.numberFlagsSet == this.flags.Length - 1)
                        {
                            this.onAnyUnset?.Invoke();
                        }
                    };
            }
        }

        public Action OnAllSet
        {
            get
            {
                return this.onAllSet;
            }
            set
            {
                this.onAllSet = value;
            }
        }

        public Action OnAnyUnset
        {
            get
            {
                return this.onAnyUnset;
            }
            set
            {
                this.onAnyUnset = value;
            }
        }
    }
}
