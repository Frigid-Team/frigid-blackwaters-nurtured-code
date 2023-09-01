#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public static class UtilityStyles
    {
        private static GUIStyle emptyStyle;

        static UtilityStyles()
        {
            emptyStyle = new GUIStyle();
        }

        public static GUIStyle EmptyStyle
        {
            get
            {
                return emptyStyle;
            }
        }

        public static GUIStyle WordWrapAndCenter(GUIStyle baseStyle)
        {
            GUIStyle newStyle = new GUIStyle(baseStyle);
            newStyle.wordWrap = true;
            newStyle.alignment = TextAnchor.MiddleCenter;
            return newStyle;
        }
    }
}
#endif
