using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEditor.Build;
#endif 

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    public interface IAssetLookupTable
    {
#if UNITY_EDITOR
        public void Populate();
#endif
    }

    public abstract class AssetLookupTable<T> : FrigidMonoBehaviour, IAssetLookupTable where T : Object
    {
        private static AssetLookupTable<T> instance;

        [SerializeField]
        [ReadOnly]
        private List<string> ids;
        [SerializeField]
        [ReadOnly]
        private List<T> assets;

        public static T ForwardLookup(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            int index = instance.ids.IndexOf(id);
            if (index == -1)
            {
                Debug.LogError("AssetLookupTable " + instance.name + " does not contain the id " + id + ".");
                return null;
            }
            return instance.assets[index];
        }

        public static string BackwardLookup(T asset)
        {
            if (asset == null)
            {
                return string.Empty;
            }
            int index = instance.assets.IndexOf(asset);
            if (index == -1)
            {
                Debug.LogError("AssetLookupTable " + instance.name + " does not contain the asset " + asset.name + ".");
                return string.Empty;
            }
            return instance.ids[index];
        }


#if UNITY_EDITOR
        public void Populate()
        {
            if (this.ids == null)
            {
                this.ids = new List<string>();
                this.assets = new List<T>();
            }
            else
            {
                this.ids = new List<string>();
                this.assets = new List<T>();
            }
            foreach (string guid in AssetDatabaseUpdater.FindAssetGUIDs<T>())
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                const string IgnorePrefix = "TEMP_";
                if (asset != null && !asset.name.StartsWith(IgnorePrefix))
                {
                    this.ids.Add(guid);
                    this.assets.Add(asset);
                }
            }
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            DontDestroyInstanceOnLoad(this);
            instance = this;
        }
    }

#if UNITY_EDITOR
    class AssetLookupTableBuilder : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            PopulateAllTables();
        }

        [MenuItem(FrigidPaths.MenuItem.Jobs + "Asset Lookup Tables/Populate All Asset Lookup Tables")]
        private static void PopulateAllTables()
        {
            Scene preloadedScene = EditorSceneManager.GetSceneAt(0);
            bool openedScene = false;
            if (preloadedScene.buildIndex != 0)
            {
                preloadedScene = EditorSceneManager.OpenScene(FrigidPaths.Scenes.Preloaded, OpenSceneMode.Additive);
                openedScene = true;
            }

            foreach (Component component in Object.FindObjectOfType<SaveFileSystem>().GetComponents<Component>())
            {
                IAssetLookupTable assetLookupTable = component as IAssetLookupTable;
                if (assetLookupTable != null)
                {
                    assetLookupTable.Populate();
                    EditorUtility.SetDirty(component);
                }
            }
            EditorSceneManager.SaveScene(preloadedScene, FrigidPaths.Scenes.Preloaded);

            if (openedScene)
            {
                EditorSceneManager.CloseScene(preloadedScene, true);
            }
        }
    }
#endif
}
