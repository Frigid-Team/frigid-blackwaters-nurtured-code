using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class TimePauser
    {
        private static ControlCounter paused;

        public static ControlCounter Paused
        {
            get
            {
                return paused;
            }
        }

        static TimePauser()
        {
            paused = new ControlCounter();
            paused.OnFirstRequest += () => Time.timeScale = 0f;
            paused.OnLastRelease += () => Time.timeScale = 1f;
        }
    }
}
