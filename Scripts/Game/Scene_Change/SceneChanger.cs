using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SceneChanger : FrigidMonoBehaviour
    {
        private static SceneChanger instance;
        private FrigidScene queuedScene;
        private ControlCounter isSceneChanging;

        public static SceneChanger Instance
        {
            get
            {
                return instance;
            }
        }

        public FrigidScene ActiveOrQueuedScene
        {
            get
            {
                if (this.isSceneChanging)
                {
                    return this.queuedScene;
                }
                else
                {
                    return (FrigidScene)SceneManager.GetActiveScene().buildIndex;
                }
            }
        }

        public void ChangeScene(FrigidScene scene)
        {
            this.isSceneChanging.Request();
            this.queuedScene = scene;
            ActivelyBusy.Request(() => FrigidCoroutine.Run(instance.Changeover(scene, ActivelyBusy.Release), instance.gameObject));
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyInstanceOnLoad(this);
            instance = this;
            this.queuedScene = FrigidScene.Preloaded;
            this.isSceneChanging = new ControlCounter();
        }

        private IEnumerator<FrigidCoroutine.Delay> Changeover(FrigidScene scene, Action onComplete)
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync((int)scene);
            loadSceneAsync.allowSceneActivation = false;
            while (!loadSceneAsync.isDone)
            {
                if (loadSceneAsync.progress >= 0.9f)
                {
                    loadSceneAsync.allowSceneActivation = true;
                }
                yield return null;
            }
            yield return null;

            this.isSceneChanging.Release();
            onComplete?.Invoke();
        }
    }
}
