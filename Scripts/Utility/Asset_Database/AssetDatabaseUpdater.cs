#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public static class AssetDatabaseUpdater
    {
        public static void EditPrefabComponents<T>(Action<T> onVisited) where T : Component
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Prefabs" });
            foreach (string guid in guids)
            {
                using (PrefabUtility.EditPrefabContentsScope editPrefabContentsScope = new PrefabUtility.EditPrefabContentsScope(AssetDatabase.GUIDToAssetPath(guid)))
                {
                    foreach (T component in editPrefabContentsScope.prefabContentsRoot.GetComponentsInChildren<T>())
                    {
                        onVisited?.Invoke(component);
                    }
                }
            }
        }

        [MenuItem(FrigidPaths.MenuItem.JOBS + "Asset Database Updater/Force Reserialize All Assets")]
        private static void ForceReserializeAllAssets()
        {
            AssetDatabase.ForceReserializeAssets();
        }

        [MenuItem(FrigidPaths.MenuItem.JOBS + "Asset Database Updater/Force Reserialize Prefabs")]
        private static void ForceReserializePrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            string[] prefabPaths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                prefabPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            AssetDatabase.ForceReserializeAssets(prefabPaths);
        }

        [MenuItem(FrigidPaths.MenuItem.JOBS + "Asset Database Updater/Force Reserialize Scriptable Objects")]
        private static void ForceReserializeScriptableObjects()
        {
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
            string[] scriptableObjectPaths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                scriptableObjectPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            AssetDatabase.ForceReserializeAssets(scriptableObjectPaths);
        }
    }
}
#endif
