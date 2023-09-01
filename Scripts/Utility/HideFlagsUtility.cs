#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    public static class HideFlagsUtility
    {
        [MenuItem(FrigidPaths.MenuItem.HELP + "Show All Hidden Objects")]
        private static void ShowAllHiddenObjects()
        {
            var allGameObjects = Object.FindObjectsOfType<GameObject>();
            foreach (var go in allGameObjects)
            {
                switch (go.hideFlags)
                {
                    case HideFlags.HideAndDontSave:
                        go.hideFlags = HideFlags.DontSave;
                        break;
                    case HideFlags.HideInHierarchy:
                    case HideFlags.HideInInspector:
                        go.hideFlags = HideFlags.None;
                        break;
                }
            }
        }
    }
}
#endif
