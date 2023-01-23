using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class CursorDisplay
    {
        private static CountingSemaphore hidden;

        public static CountingSemaphore Hidden
        {
            get
            {
                return hidden;
            }
        }

        static CursorDisplay()
        {
            hidden = new CountingSemaphore();
            hidden.OnFirstRequest += () => Cursor.visible = false;
            hidden.OnLastRelease += () => Cursor.visible = true;
            Application.focusChanged += EvaluateCursorHidden;
        }
       
        private static void EvaluateCursorHidden(bool hasFocus)
        {
            Cursor.visible = !hidden;
        }
    }
}
