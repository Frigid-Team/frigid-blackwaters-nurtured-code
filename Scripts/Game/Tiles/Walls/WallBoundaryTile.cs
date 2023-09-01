using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class WallBoundaryTile : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private string edgeAnimationName;
        [SerializeField]
        private string cornerAnimationName;

        public void Populate(bool isEdge)
        {
            this.animatorBody.Play(isEdge ? this.edgeAnimationName : this.cornerAnimationName);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
