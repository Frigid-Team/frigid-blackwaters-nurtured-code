using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class AudioPauser
    {
        private static CountingSemaphore paused;

        public static CountingSemaphore Paused
        {
            get
            {
                return paused;
            }
        }

        static AudioPauser()
        {
            paused = new CountingSemaphore();
            paused.OnFirstRequest += () => AudioListener.pause = true;
            paused.OnLastRelease += () => AudioListener.pause = false;
        }
    }
}
