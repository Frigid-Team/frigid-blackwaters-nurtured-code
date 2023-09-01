#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    public static class Snapshot
    {
        private const int DEPTH_BUFFER = 24;
        private const string SNAPSHOT_PREFAB_PATH = "Assets/Prefabs/Editor/Snapshot.prefab";

        public static void SnapPrefab<T>(T prefab, Action<T> onSetup, out Texture2D texture, out Vector2 pivot) where T : MonoBehaviour
        {
            Scene scene = EditorSceneManager.NewPreviewScene();
            PrefabUtility.LoadPrefabContentsIntoPreviewScene(AssetDatabase.GetAssetPath(prefab), scene);

            GameObject prefabObject = scene.GetRootGameObjects()[0];
            onSetup?.Invoke(prefabObject.GetComponent<T>());
            foreach (Transform transform in prefabObject.GetComponentsInChildren<Transform>(false))
            {
                transform.gameObject.layer = (int)FrigidLayer.Snapshot;
            }

            Renderer[] renderers = prefabObject.GetComponentsInChildren<Renderer>();
            Bounds bounds = new Bounds();
            if (renderers.Length > 0)
            {
                bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }

            texture = Snap(Mathf.RoundToInt(bounds.size.x * FrigidConstants.PIXELS_PER_UNIT), Mathf.RoundToInt(bounds.size.y * FrigidConstants.PIXELS_PER_UNIT), bounds.center, scene);
            pivot = bounds.center * FrigidConstants.PIXELS_PER_UNIT * new Vector2(1, -1);
            EditorSceneManager.ClosePreviewScene(scene);
        }

        private static Texture2D Snap(int widthPixels, int heightPixels, Vector2 pos, Scene previewScene)
        {
            PrefabUtility.LoadPrefabContentsIntoPreviewScene(SNAPSHOT_PREFAB_PATH, previewScene);
            foreach (GameObject rootGameObject in previewScene.GetRootGameObjects())
            {
                if (rootGameObject.TryGetComponent<Camera>(out Camera camera))
                {
                    GameObject snapObject = camera.gameObject;
                    snapObject.transform.position = new Vector3(pos.x, pos.y, 0.0f);
                    snapObject.layer = (int)FrigidLayer.Snapshot;

                    camera.cullingMask = 1 << (int)FrigidLayer.Snapshot;
                    camera.orthographic = true;
                    camera.orthographicSize = (float)heightPixels / FrigidConstants.PIXELS_PER_UNIT / 2f;
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.backgroundColor = Color.clear;
                    camera.nearClipPlane = 0.0f;
                    camera.enabled = false;
                    camera.cameraType = CameraType.Preview;
                    camera.scene = previewScene;

                    camera.targetTexture = RenderTexture.GetTemporary(widthPixels, heightPixels, DEPTH_BUFFER);
                    camera.Render();
                    RenderTexture previouslyActiveRenderTexture = RenderTexture.active;
                    RenderTexture.active = camera.targetTexture;
                    Texture2D texture = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.ARGB32, false);
                    texture.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
                    texture.Apply(false);
                    RenderTexture.active = previouslyActiveRenderTexture;
                    RenderTexture.ReleaseTemporary(camera.targetTexture);
                    camera.targetTexture = null;

                    return texture;
                }
            }
            return null;
        }
    }
}
#endif
