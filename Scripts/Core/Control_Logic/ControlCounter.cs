using System;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public class ControlCounter
    {
        private int counter;
        private Action onFirstRequest;
        private Action onLastRelease;

        public static implicit operator bool(ControlCounter semaphore)
        {
            return semaphore == null || semaphore.counter > 0;
        }

        public ControlCounter(int counter = 0) 
        { 
            this.counter = counter; 
        }

        public int Counter
        {
            get
            {
                return this.counter;
            }
        }

        public Action OnFirstRequest
        {
            get
            {
                return this.onFirstRequest;
            }
            set
            {
                this.onFirstRequest = value;
            }
        }

        public Action OnLastRelease
        {
            get
            {
                return this.onLastRelease;
            }
            set
            {
                this.onLastRelease = value;
            }
        }

        public void Request()
        {
            this.counter++;
            if (this.counter == 1)
            {
                this.onFirstRequest?.Invoke();
            }
        }

        public void Release()
        {
            if (this.counter == 1)
            {
                this.onLastRelease?.Invoke();
            }
            this.counter = Mathf.Max(0, this.counter - 1);
        }
    }
}
