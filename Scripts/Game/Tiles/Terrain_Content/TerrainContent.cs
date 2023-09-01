using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public abstract class TerrainContent : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;

        private List<Vector2Int> tileIndexPositions;
        private NavigationGrid navigationGrid;

        public virtual void Preview(Vector2 orientationDirection) { }

        public virtual void Populate(Vector2 orientationDirection, NavigationGrid navigationGrid, List<Vector2Int> tileIndexPositions) 
        {
            this.animatorBody.Direction = orientationDirection;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = DamageAlignment.Environment;
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.DamageAlignment = DamageAlignment.Environment;
            }
            this.navigationGrid = navigationGrid;
            this.tileIndexPositions = tileIndexPositions;
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

        protected List<Vector2Int> TileIndexPositions
        {
            get
            {
                return this.tileIndexPositions;
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
