#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public static class UtilityGUIUtility
    {
        public static float LargeLineHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }
        }

        public static Color Darken(Color baseColor, int times = 1)
        {
            return baseColor * new Color(Mathf.Pow(0.5f, times), Mathf.Pow(0.5f, times), Mathf.Pow(0.5f, times), 1);
        }
    }
}
#endif
