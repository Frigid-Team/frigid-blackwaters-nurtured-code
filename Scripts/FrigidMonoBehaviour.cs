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

        protected virtual void Awake() { }

        protected virtual void Start() { }

        protected virtual void Update() { }

        protected virtual void OnTriggerEnter2D(Collider2D collision) { }

        protected virtual void OnTriggerStay2D(Collider2D collision) { }

        protected virtual void OnTriggerExit2D(Collider2D collision) { }

        protected virtual void OnCollisionEnter2D(Collision2D collision) { }

        protected virtual void OnCollisionStay2D(Collision2D collision) { }

        protected virtual void OnCollisionExit2D(Collision2D collision) { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected virtual void OnDestroy() { }

        protected virtual void OnApplicationFocus(bool hasFocus) { }

#if UNITY_EDITOR

        protected virtual bool OwnsGameObject() { return false; }

        protected virtual void OnDrawGizmos() { }

        protected virtual void OnDrawGizmosSelected() { }

        private void Reset()
        {
            if (OwnsGameObject())
            {
                foreach (FrigidMonoBehaviour frigidMonoBehaviour in GetComponents<FrigidMonoBehaviour>())
                {
                    if (frigidMonoBehaviour != this && frigidMonoBehaviour.OwnsGameObject())
                    {
                        Debug.LogError(GetType().Name + " should be on its own GameObject. It is conflicting with " + frigidMonoBehaviour.GetType().Name + ".");
                        Undo.DestroyObjectImmediate(this);
                    }
                }
            }
        }
#endif
    }
}
