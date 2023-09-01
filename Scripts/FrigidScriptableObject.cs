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
        [Obsolete("Use CreateInstance instead.")]
        public static new T Instantiate<T>(T original, Transform parent) where T : UnityEngine.Object { return null; }

        [Obsolete("Use CreateInstance instead.")]
        public static new UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation) { return null; }

        [Obsolete("Use CreateInstance instead.")]
        public static new T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : UnityEngine.Object { return null; }

        [Obsolete("Use CreateInstance instead.")]
        public static new UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation, Transform parent) { return null; }

        [Obsolete("Use CreateInstance instead.")]
        public static new UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent, bool instantiateInWorldSpace) { return null; }

        [Obsolete("Use CreateInstance instead.")]
        public static new T Instantiate<T>(T original) where T : UnityEngine.Object { return null; }

        [Obsolete("Use CreateInstance instead.")]
        public static new T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : UnityEngine.Object { return null; }

        [Obsolete("Use CreateInstance instead.")]
        public static new T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : UnityEngine.Object { return null; }

        [Obsolete("Use CreateInstance instead.")]
        public static new UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent) { return null; }

        [Obsolete("Use DestroyInstance instead.")]
        public static new void Destroy(UnityEngine.Object obj, float t) { }

        [Obsolete("Use DestroyInstance instead.")]
        public static new void Destroy(UnityEngine.Object obj) { }

        [Obsolete("Use DontDestroyInstanceOnLoad instead.")]
        public static new void DontDestroyOnLoad(UnityEngine.Object target) { }
        #endregion

        public static T CreateInstance<T>() where T : FrigidScriptableObject
        {
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        public static T CreateInstance<T>(T original) where T : FrigidScriptableObject
        {
            return UnityEngine.Object.Instantiate(original);
        }

        public static void DestroyInstance(FrigidScriptableObject instance)
        {
            UnityEngine.Object.Destroy(instance);
        }

        protected virtual void Initialize() { }

        protected virtual void OnBegin() { }

        protected virtual void OnEnd() { }

        protected virtual void Terminate() { }

#if UNITY_EDITOR
        private void OnEnable()
        {
            this.Initialize();
            EditorApplication.playModeStateChanged += this.OnPlayStateChange;
        }

        private void OnDisable()
        {
            this.Terminate();
            EditorApplication.playModeStateChanged -= this.OnPlayStateChange;
        }

        void OnPlayStateChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                this.OnBegin();
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                this.OnEnd();
            }
        }
#else
        private void OnEnable()
        {
            Initialize();
            OnBegin();
        }
 
        private void OnDisable()
        {
            Terminate();
            OnEnd();
        }
#endif
    }
}
