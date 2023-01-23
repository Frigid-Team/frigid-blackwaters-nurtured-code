using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class TimePauser
    {
        private static CountingSemaphore paused;

        public static CountingSemaphore Paused
        {
            get
            {
                return paused;
            }
        }

        static TimePauser()
        {
            paused = new CountingSemaphore();
            paused.OnFirstRequest += () => Time.timeScale = 0f;
            paused.OnLastRelease += () => Time.timeScale = 1f;
        }
    }
}
