using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class CursorDisplay
    {
        private static ControlCounter hidden;

        public static ControlCounter Hidden
        {
            get
            {
                return hidden;
            }
        }

        static CursorDisplay()
        {
            hidden = new ControlCounter();
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
