using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrigidBlackwaters.Game
{
    public class SceneChanger : FrigidMonoBehaviour
    {
        private static SceneChanger instance;

        public static SceneChanger Instance
        {
            get
            {
                return instance;
            }
        }

        public void ChangeScene(string sceneName, Action onBeforeSceneActivate)
        {
            LoadingOverlay.RequestLoad(() => FrigidCoroutine.Run(instance.Changeover(sceneName, onBeforeSceneActivate, LoadingOverlay.ReleaseLoad), instance.gameObject));
        }

        protected override void Awake()
        {
            base.Awake();
            instance = this;
            FrigidInstancing.DontDestroyInstanceOnLoad(this);
        }

        private IEnumerator<FrigidCoroutine.Delay> Changeover(string sceneName, Action onBeforeSceneActivate, Action onComplete)
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
            loadSceneAsync.allowSceneActivation = false;

            while (!loadSceneAsync.isDone)
            {
                if (loadSceneAsync.progress >= 0.9f)
                {
                    onBeforeSceneActivate?.Invoke();
                    loadSceneAsync.allowSceneActivation = true;
                }
                yield return null;
            }
            yield return null;

            onComplete?.Invoke();
        }
    }
}
