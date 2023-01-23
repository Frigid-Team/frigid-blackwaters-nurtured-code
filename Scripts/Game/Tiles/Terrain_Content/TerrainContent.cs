using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public abstract class TerrainContent : FrigidMonoBehaviour
    {
        [SerializeField]
        private Vector2Int dimensions;
        [SerializeField]
        private AnimatorBody animatorBody;

        private List<Vector2Int> allTileIndices;
        private NavigationGrid navigationGrid;

        public Vector2Int Dimensions
        {
            get
            {
                return this.dimensions;
            }
        }

        public virtual void Populated(Vector2 orientationDirection, NavigationGrid navigationGrid, List<Vector2Int> allTileIndices) 
        {
            this.animatorBody.Direction = orientationDirection;
            this.navigationGrid = navigationGrid;
            this.allTileIndices = allTileIndices;
        }

        protected AnimatorBody AnimatorBody
        {
            get
            {
                return this.animatorBody;
            }
        }

        protected NavigationGrid NavigationGrid
        {
            get
            {
                return this.navigationGrid;
            }
        }

        protected List<Vector2Int> AllTileIndices
        {
            get
            {
                return this.allTileIndices;
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
