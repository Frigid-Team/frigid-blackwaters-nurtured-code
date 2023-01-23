using UnityEngine;

namespace FrigidBlackwaters
{
    public static class FrigidInstancing
    {
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
            T spawnedObject = Object.Instantiate<T>(original, position, rotation);
            spawnedObject.transform.SetParent(parent, worldTransformStays);
            return spawnedObject;
        }

        public static void DestroyInstance(FrigidMonoBehaviour instance)
        {
            Object.Destroy(instance.gameObject);
        }

        public static void DontDestroyInstanceOnLoad(FrigidMonoBehaviour instance)
        {
            Object.DontDestroyOnLoad(instance.gameObject);
        }
    }
}
