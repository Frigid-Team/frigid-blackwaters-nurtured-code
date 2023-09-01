using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class WallContent : FrigidMonoBehaviour
    {
        [Space]
        [SerializeField]
        private AnimatorBody animatorBody;

        public virtual void Preview(Vector2 orientationDirection) { }

        public virtual void Populate(Vector2 orientationDirection)
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
