#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public static class GUIStyling
    {
        private static GUIStyle emptyStyle;

        static GUIStyling()
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

        public static Color Darken(Color baseColor, int times = 1)
        {
            return baseColor * new Color(Mathf.Pow(0.5f, times), Mathf.Pow(0.5f, times), Mathf.Pow(0.5f, times), 1);
        }
    }
}
#endif
