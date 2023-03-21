using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class TerrainTile : FrigidMonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private PolygonCollider2D pushCollider;
        [SerializeField]
        private float innerExtent;
        [SerializeField]
        private float outerExtent;
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private string[] animationNames;

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        public void Populated(bool pushEnabled, List<Vector2Int> cornerPoints)
        {
            this.pushCollider.enabled = pushEnabled;
            if (pushEnabled)
            {
                Vector2[] colliderPoints = new Vector2[cornerPoints.Count];
                for (int i = 0; i < cornerPoints.Count; i++)
                {
                    colliderPoints[i] = new Vector2(
                        Mathf.Abs(cornerPoints[i].x) == 1 ? this.innerExtent * cornerPoints[i].x : this.outerExtent / 2 * cornerPoints[i].x,
                        Mathf.Abs(cornerPoints[i].y) == 1 ? this.innerExtent * cornerPoints[i].y : this.outerExtent / 2 * cornerPoints[i].y
                        );
                }
                this.pushCollider.points = colliderPoints;
            }
            this.animatorBody.Play(this.animationNames[Random.Range(0, this.animationNames.Length)]);
        }
    }
}
