using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrigidBlackwaters
{
    public abstract class FrigidMonoBehaviour : MonoBehaviour
    {
        #region Obsolete
        [Obsolete("Use FrigidCoroutine.Run instead.")]
        public new Coroutine StartCoroutine(string methodName) { return null; }

        [Obsolete("Use FrigidCoroutine.Run instead.")]
        public new Coroutine StartCoroutine(IEnumerator routine) { return null; }

        [Obsolete("Use FrigidCoroutine.Run instead.")]
        public new Coroutine StartCoroutine(string methodName, object value) { return null; }

        [Obsolete("Use FrigidCoroutine.Kill instead.")]
        public new void StopAllCoroutines() { }

        [Obsolete("Use FrigidCoroutine.Kill instead.")]
        public new void StopCoroutine(IEnumerator routine) { }

        [Obsolete("Use FrigidCoroutine.Kill instead.")]
        public new void StopCoroutine(Coroutine routine) { }

        [Obsolete("Use FrigidCoroutine.Kill instead.")]
        public new void StopCoroutine(string methodName) { }

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

        public static T CreateInstance<T>(T original) where T : FrigidMonoBehaviour
        {
            return CreateInstance<T>(original, Vector3.zero, Quaternion.identity, null, true);
        }

        public static T CreateInstance<T>(T original, Vector3 position) where T : FrigidMonoBehaviour
        {
            return CreateInstance<T>(original, position, Quaternion.identity, null, true);
        }

        public static T CreateInstance<T>(T original, Vector3 position, Quaternion rotation) where T : FrigidMonoBehaviour
        {
            return CreateInstance<T>(original, position, rotation, null, true);
        }

        public static T CreateInstance<T>(T original, Transform parent) where T : FrigidMonoBehaviour
        {
            return CreateInstance<T>(original, Vector3.zero, Quaternion.identity, parent, true);
        }

        public static T CreateInstance<T>(T original, Transform parent, bool worldTransformStays) where T : FrigidMonoBehaviour
        {
            return CreateInstance<T>(original, Vector3.zero, Quaternion.identity, parent, worldTransformStays);
        }

        public static T CreateInstance<T>(T original, Vector3 position, Transform parent) where T : FrigidMonoBehaviour
        {
            return CreateInstance<T>(original, position, Quaternion.identity, parent, true);
        }

        public static T CreateInstance<T>(T original, Vector3 position, Transform parent, bool worldTransformStays) where T : FrigidMonoBehaviour
        {
            return CreateInstance<T>(original, position, Quaternion.identity, parent, worldTransformStays);
        }

        public static T CreateInstance<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : FrigidMonoBehaviour
        {
            return CreateInstance<T>(original, position, rotation, parent, true);
        }

        public static T CreateInstance<T>(T original, Vector3 position, Quaternion rotation, Transform parent, bool worldTransformStays) where T : FrigidMonoBehaviour
        {
            T spawnedObject = UnityEngine.Object.Instantiate<T>(original, position, rotation);
            spawnedObject.transform.SetParent(parent, worldTransformStays);
            return spawnedObject;
        }

        public static void DestroyInstance(FrigidMonoBehaviour instance)
        {
            UnityEngine.Object.Destroy(instance.gameObject);
        }

        public static void DontDestroyInstanceOnLoad(FrigidMonoBehaviour instance)
        {
            UnityEngine.Object.DontDestroyOnLoad(instance.gameObject);
        }

        protected virtual void Awake() { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected virtual void Start() { }

        protected virtual void OnDestroy() { }

#if UNITY_EDITOR
        protected virtual bool OwnsGameObject() { return false; }

        private void Reset()
        {
            if (this.OwnsGameObject())
            {
                foreach (FrigidMonoBehaviour frigidMonoBehaviour in this.GetComponents<FrigidMonoBehaviour>())
                {
                    if (frigidMonoBehaviour != this && frigidMonoBehaviour.OwnsGameObject())
                    {
                        Debug.LogError(this.GetType().Name + " should be on its own GameObject. It is conflicting with " + frigidMonoBehaviour.GetType().Name + ".");
                        Undo.DestroyObjectImmediate(this);
                    }
                }
            }
        }
#endif
    }
}
