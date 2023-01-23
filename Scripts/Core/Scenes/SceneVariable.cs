using System;
using UnityEngine.SceneManagement;

namespace FrigidBlackwaters.Core
{
    public class SceneVariable<T> where T : new()
    {
        private Scene? currentScene;
        private T currentInstance;
        private Func<T> toGetNewInstance;

        public SceneVariable(Func<T> toGetNewInstance)
        {
            this.toGetNewInstance = toGetNewInstance;
        }

        public T Current
        {
            get
            {
                Scene activeScene = SceneManager.GetActiveScene();
                if (!this.currentScene.HasValue || this.currentScene.Value != activeScene)
                {
                    this.currentScene = activeScene;
                    this.currentInstance = this.toGetNewInstance.Invoke();
                }
                return this.currentInstance;
            }
            set
            {
                Scene activeScene = SceneManager.GetActiveScene();
                if (!this.currentScene.HasValue || this.currentScene.Value != activeScene)
                {
                    this.currentScene = activeScene;
                    this.currentInstance = this.toGetNewInstance.Invoke();
                }
                this.currentInstance = value;
            }
        }
    }
}
