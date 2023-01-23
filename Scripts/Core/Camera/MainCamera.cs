using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public class MainCamera : FrigidMonoBehaviour
    {
        [SerializeField]
        private Camera camera;

        private static MainCamera instance;

        public static MainCamera Instance
        {
            get
            {
                return instance;
            }
        }
         
        public Camera Camera
        {
            get
            {
                return this.camera;
            }
        }

        public Bounds WorldBounds
        {
            get
            {
                float screenAspect = (float)Screen.width / (float)Screen.height;
                float cameraHeight = this.camera.orthographicSize * 2;
                return new Bounds(this.transform.position, new Vector2(cameraHeight * screenAspect, cameraHeight));
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (instance != null)
            {
                Debug.LogError("More than one main camera in scene");
            }
            instance = this;
        }
    }
}
