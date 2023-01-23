using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrigidBlackwaters
{
    public abstract class FrigidScriptableObject : ScriptableObject
    {
        #region Obsolete
        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new T Instantiate<T>(T original, Transform parent) where T : UnityEngine.Object { return null; }

        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation) { return null; }

        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : UnityEngine.Object { return null; }

        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation, Transform parent) { return null; }

        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent, bool instantiateInWorldSpace) { return null; }

        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new T Instantiate<T>(T original) where T : UnityEngine.Object { return null; }

        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : UnityEngine.Object { return null; }

        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : UnityEngine.Object { return null; }

        [Obsolete("Use FrigidInstancing.CreateInstance instead.")]
        public static new UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent) { return null; }

        [Obsolete("Use FrigidInstancing.DestroyInstance instead.")]
        public static new void Destroy(UnityEngine.Object obj, float t) { }

        [Obsolete("Use FrigidInstancing.DestroyInstance instead.")]
        public static new void Destroy(UnityEngine.Object obj) { }

        [Obsolete("Use FrigidInstancing.DontDestroyInstanceOnLoad instead.")]
        public static new void DontDestroyOnLoad(UnityEngine.Object target) { }
        #endregion

        protected virtual void Init() { }

        protected void OnEnable() 
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Init();
            }
#else
            Init();
#endif
        }
    }
}
