#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public static class AssetDatabaseUpdater
    {
        public static string[] FindPrefabGUIDs()
        {
            return FindPrefabGUIDs(string.Empty);
        }

        public static string[] FindPrefabGUIDs(string filter)
        {
            return AssetDatabase.FindAssets(filter + " t:prefab", new string[] { FrigidPaths.ProjectFolder.Assets });
        }

        public static string[] FindAssetGUIDs<T>() where T : UnityEngine.Object
        {
            return FindAssetGUIDs<T>(string.Empty);
        }

        public static string[] FindAssetGUIDs<T>(string filter) where T : UnityEngine.Object
        {
            return AssetDatabase.FindAssets(filter + " t:" + typeof(T).Name, new string[] { FrigidPaths.ProjectFolder.Assets });
        }

        public static void EditPrefabs<T>(Action<T> onVisited) where T : Component
        {
            string[] guids = FindPrefabGUIDs();
            foreach (string guid in guids)
            {
                using (PrefabUtility.EditPrefabContentsScope editPrefabContentsScope = new PrefabUtility.EditPrefabContentsScope(AssetDatabase.GUIDToAssetPath(guid)))
                {
                    if (editPrefabContentsScope.prefabContentsRoot.TryGetComponent<T>(out T prefab))
                    {
                        onVisited?.Invoke(prefab);
                    }
                }
            }
        }

        public static void EditPrefabComponents<T>(Action<T> onVisited) where T : Component
        {
            string[] guids = FindPrefabGUIDs();
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

        public static void EditAssets<T>(Action<T> onVisited) where T : UnityEngine.Object
        {
            string[] guids = FindAssetGUIDs<T>();
            foreach (string guid in guids)
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null)
                {
                    onVisited?.Invoke(asset);
                    EditorUtility.SetDirty(asset);
                }
            }
            AssetDatabase.SaveAssets();
        }

        public static T[] FindPrefabComponents<T>() where T : Component
        {
            return FindPrefabComponents<T>(string.Empty);
        }

        public static T[] FindPrefabComponents<T>(string filter) where T : Component
        {
            return FindPrefabComponents<T>(filter, null);
        }

        public static T[] FindPrefabComponents<T>(string filter, Func<T, bool> predicate) where T : Component
        {
            string[] guids = FindPrefabGUIDs(filter);
            List<T> components = new List<T>();
            foreach (string guid in guids)
            {
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (gameObject != null)
                {
                    foreach (T component in gameObject.GetComponentsInChildren<T>())
                    {
                        if (predicate == null || predicate.Invoke(component))
                        {
                            components.Add(component);
                        }
                    }
                }
            }
            return components.ToArray();
        }

        public static T[] FindPrefabs<T>() where T : Component
        {
            return FindPrefabs<T>(string.Empty);
        }

        public static T[] FindPrefabs<T>(string filter) where T : Component
        {
            return FindPrefabs<T>(filter, null);
        }

        public static T[] FindPrefabs<T>(string filter, Func<T, bool> predicate) where T : Component
        {
            string[] guids = FindPrefabGUIDs(filter);
            T[] prefabs = new T[guids.Length];
            int numFound = 0;
            foreach (string guid in guids)
            {
                GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (gameObject != null && gameObject.TryGetComponent<T>(out T prefab) && (predicate == null || predicate.Invoke(prefab)))
                {
                    prefabs[numFound] = prefab;
                    numFound++;
                }
            }
            Array.Resize(ref prefabs, numFound);
            return prefabs;
        }

        public static bool TryFindPrefab<T>(out T prefab) where T : Component
        {
            return TryFindPrefab<T>(string.Empty, out prefab);
        }

        public static bool TryFindPrefab<T>(string filter, out T prefab) where T : Component
        {
            return TryFindPrefab(filter, null, out prefab);
        }

        public static bool TryFindPrefab<T>(string filter, Func<T, bool> predicate, out T prefab) where T : Component
        {
            T[] prefabsfound = FindPrefabs<T>(filter, predicate);
            foreach (T prefabFound in prefabsfound)
            {
                prefab = prefabFound;
                return true;
            }
            prefab = null;
            return false;
        }

        public static T[] FindAssets<T>() where T : UnityEngine.Object
        {
            return FindAssets<T>(string.Empty);
        }

        public static T[] FindAssets<T>(string filter) where T : UnityEngine.Object
        {
            return FindAssets<T>(filter, null);
        }

        public static T[] FindAssets<T>(string filter, Func<T, bool> predicate) where T : UnityEngine.Object
        {
            string[] guids = FindAssetGUIDs<T>(filter);
            T[] assets = new T[guids.Length];
            int numFound = 0;
            foreach (string guid in guids)
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null && (predicate == null || predicate.Invoke(asset)))
                {
                    assets[numFound] = asset;
                    numFound++;
                }
            }
            Array.Resize(ref assets, numFound);
            return assets;
        }

        public static bool TryFindAsset<T>(out T asset) where T : UnityEngine.Object
        {
            return TryFindAsset<T>(string.Empty, out asset);
        }

        public static bool TryFindAsset<T>(string filter, out T asset) where T : UnityEngine.Object
        {
            return TryFindAsset<T>(filter, null, out asset);
        }

        public static bool TryFindAsset<T>(string filter, Func<T, bool> predicate, out T asset) where T : UnityEngine.Object
        {
            T[] assetsFound = FindAssets<T>(filter, predicate);
            foreach (T assetFound in assetsFound)
            {
                asset = assetFound;
                return true;
            }
            asset = null;
            return false;
        }

        [MenuItem(FrigidPaths.MenuItem.Jobs + "Asset Database Updater/Find Assets")]
        private static void FindAssets()
        {
            List<Type> assetTypes = TypeUtility.GetCompleteTypesDerivedFrom(typeof(UnityEngine.Object));
            string[] searchEntries = new string[assetTypes.Count];
            for (int i = 0; i < searchEntries.Length; i++) searchEntries[i] = assetTypes[i].Name;

            FrigidPopup.Show(
                new Rect(Vector2.zero, Vector2.zero),
                new SearchPopup(
                    searchEntries,
                    (int index) =>
                    {
                        Type assetType = assetTypes[index];
                        string[] guids = FindAssetGUIDs<UnityEngine.Object>();
                        foreach (string guid in guids)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                            if (asset != null)
                            {
                                Debug.Log(assetPath);
                            }
                        }
                    }
                    )
                );
        }

        [MenuItem(FrigidPaths.MenuItem.Jobs + "Asset Database Updater/Find Prefab Components")]
        private static void FindPrefabComponents()
        {
            List<Type> componentTypes = TypeUtility.GetCompleteTypesDerivedFrom(typeof(Component));
            string[] searchEntries = new string[componentTypes.Count];
            for (int i = 0; i < searchEntries.Length; i++) searchEntries[i] = componentTypes[i].Name;

            FrigidPopup.Show(
                new Rect(Vector2.zero, Vector2.zero), 
                new SearchPopup(
                    searchEntries, 
                    (int index) => 
                    {
                        Type componentType = componentTypes[index];
                        string[] guids = FindPrefabGUIDs();
                        foreach (string guid in guids)
                        {
                            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                            if (gameObject != null)
                            {
                                foreach (Component component in gameObject.GetComponentsInChildren(componentType))
                                {
                                    Debug.Log(component.transform.root.name + " " + component.name);
                                }
                            }
                        }
                    }
                    )
                );
        }

        [MenuItem(FrigidPaths.MenuItem.Jobs + "Asset Database Updater/Force Reserialize All Assets")]
        private static void ForceReserializeAllAssets()
        {
            AssetDatabase.ForceReserializeAssets();
        }

        [MenuItem(FrigidPaths.MenuItem.Jobs + "Asset Database Updater/Force Reserialize Prefabs")]
        private static void ForceReserializePrefabs()
        {
            string[] guids = FindPrefabGUIDs();
            string[] prefabPaths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                prefabPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            AssetDatabase.ForceReserializeAssets(prefabPaths);
        }

        [MenuItem(FrigidPaths.MenuItem.Jobs + "Asset Database Updater/Force Reserialize Scriptable Objects")]
        private static void ForceReserializeScriptableObjects()
        {
            string[] guids = FindAssetGUIDs<ScriptableObject>();
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
