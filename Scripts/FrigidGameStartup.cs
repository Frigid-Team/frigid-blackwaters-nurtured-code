using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace FrigidBlackwaters
{
    internal static class FrigidGameStartup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void PreloadBeforePlay()
        {
            SceneManager.LoadScene(0);
#if UNITY_EDITOR
            EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().name);
#else
            SceneManager.LoadScene(1);
#endif
        }
    }
}
