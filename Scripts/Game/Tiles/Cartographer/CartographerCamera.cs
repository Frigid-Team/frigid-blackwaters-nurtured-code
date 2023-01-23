#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class CartographerCamera : FrigidMonoBehaviour
    {
        [SerializeField]
        private float scrollSpeed;
        [SerializeField]
        private float zoomSpeed;

        protected override void Update()
        {
            base.Update();
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.Translate(Vector3.left * this.scrollSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.Translate(Vector3.right * this.scrollSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.transform.Translate(Vector3.down * this.scrollSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.Translate(Vector3.up * this.scrollSpeed * Time.deltaTime);
            }
            if (Input.mouseScrollDelta.y != 0)
            {
                UnityEngine.Camera.main.orthographicSize = UnityEngine.Camera.main.orthographicSize + Input.mouseScrollDelta.y * this.zoomSpeed * Time.deltaTime;
                if (UnityEngine.Camera.main.orthographicSize <= 1)
                {
                    UnityEngine.Camera.main.orthographicSize = 1;
                }
            }
        }
    }
}
#endif
