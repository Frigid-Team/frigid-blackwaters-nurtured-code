using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TerrainTile : FrigidMonoBehaviour
    {
        [SerializeField]
        private PolygonCollider2D pushCollider;
        [SerializeField]
        private float innerExtent;
        [SerializeField]
        private float outerExtent;
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private string[] animationNames;

        public void Preview()
        {
            this.animatorBody.Preview(this.animationNames[0], 0, Vector2.zero);
        }

        public void Populate(bool pushEnabled, List<Vector2Int> cornerPoints, NavigationGrid navigationGrid, Vector2Int tileIndexPosition)
        {
            switch (navigationGrid[tileIndexPosition].Terrain)
            {
                case TileTerrain.Land:
                    this.gameObject.layer = (int)FrigidLayer.LandTerrain;
                    break;
                case TileTerrain.Water:
                    this.gameObject.layer = (int)FrigidLayer.WaterTerrain;
                    break;
                default:
                    this.gameObject.layer = (int)FrigidLayer.Default;
                    break;
            }
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

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
