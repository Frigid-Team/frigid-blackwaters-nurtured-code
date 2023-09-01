using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class WallTile : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private string edgeAnimationName;
        [SerializeField]
        private string cornerAnimationName;

        public void Preview(bool isEdge)
        {
            this.animatorBody.Preview(isEdge ? this.edgeAnimationName : this.cornerAnimationName, 0, Vector2.zero);
        }

        public void Populate(bool isEdge)
        {
            this.animatorBody.Play(isEdge ? this.edgeAnimationName : this.cornerAnimationName);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
