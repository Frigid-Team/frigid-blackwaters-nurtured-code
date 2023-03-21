using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class AudioPauser
    {
        private static ControlCounter paused;

        public static ControlCounter Paused
        {
            get
            {
                return paused;
            }
        }

        static AudioPauser()
        {
            paused = new ControlCounter();
            paused.OnFirstRequest += () => AudioListener.pause = true;
            paused.OnLastRelease += () => AudioListener.pause = false;
        }
    }
}
