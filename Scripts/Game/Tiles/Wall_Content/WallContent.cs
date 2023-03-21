
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class WallContent : FrigidMonoBehaviour
    {
        [SerializeField]
        private int width;
        [SerializeField]
        private int depth;
        [SerializeField]
        private AnimatorBody animatorBody;

        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public int Depth
        {
            get
            {
                return this.depth;
            }
        }

        public virtual void Populated(Vector2 orientationDirection)
        {
            this.animatorBody.Direction = orientationDirection;
        }

        protected AnimatorBody AnimatorBody
        {
            get
            {
                return this.animatorBody;
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
