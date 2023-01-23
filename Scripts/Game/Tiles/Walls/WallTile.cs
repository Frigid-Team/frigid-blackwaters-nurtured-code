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

        public void Populated(bool isEdge)
        {
            this.animatorBody.PlayByName(isEdge ? this.edgeAnimationName : this.cornerAnimationName);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
