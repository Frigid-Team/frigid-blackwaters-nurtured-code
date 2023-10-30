using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class WallColliders : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<WallSide> wallSides;
        [SerializeField]
        private float breakBoxInsetDistance;

        public void PositionColliders(Vector2Int mainAreaDimensions)
        {
            Debug.Assert(this.wallSides.Count == 4, "Invalid number of wall sides in WallColliders " + this.name + ".");

            this.wallSides[0].PushCollider.offset = new Vector2(0, (mainAreaDimensions.y + 1) / 2f * FrigidConstants.UnitWorldSize);
            this.wallSides[0].PushCollider.size = new Vector2((mainAreaDimensions.x + 2) * FrigidConstants.UnitWorldSize, FrigidConstants.UnitWorldSize);
            this.wallSides[0].BreakBoxCollider.offset = new Vector2(0, (mainAreaDimensions.y + 1) / 2f * FrigidConstants.UnitWorldSize + this.breakBoxInsetDistance / 2);
            this.wallSides[0].BreakBoxCollider.size = new Vector2((mainAreaDimensions.x + 2) * FrigidConstants.UnitWorldSize, FrigidConstants.UnitWorldSize - this.breakBoxInsetDistance);

            this.wallSides[1].PushCollider.offset = new Vector2(0, (-mainAreaDimensions.y - 1) / 2f * FrigidConstants.UnitWorldSize);
            this.wallSides[1].PushCollider.size = new Vector2((mainAreaDimensions.x + 2) * FrigidConstants.UnitWorldSize, FrigidConstants.UnitWorldSize);
            this.wallSides[1].BreakBoxCollider.offset = new Vector2(0, (-mainAreaDimensions.y - 1) / 2f * FrigidConstants.UnitWorldSize - this.breakBoxInsetDistance / 2);
            this.wallSides[1].BreakBoxCollider.size = new Vector2((mainAreaDimensions.x + 2) * FrigidConstants.UnitWorldSize, FrigidConstants.UnitWorldSize - this.breakBoxInsetDistance);

            this.wallSides[2].PushCollider.offset = new Vector2((-mainAreaDimensions.x - 1) / 2f * FrigidConstants.UnitWorldSize, 0);
            this.wallSides[2].PushCollider.size = new Vector2(FrigidConstants.UnitWorldSize, (mainAreaDimensions.y + 2) * FrigidConstants.UnitWorldSize);
            this.wallSides[2].BreakBoxCollider.offset = new Vector2((-mainAreaDimensions.x - 1) / 2f * FrigidConstants.UnitWorldSize - this.breakBoxInsetDistance / 2, 0);
            this.wallSides[2].BreakBoxCollider.size = new Vector2(FrigidConstants.UnitWorldSize - this.breakBoxInsetDistance, (mainAreaDimensions.y + 2) * FrigidConstants.UnitWorldSize);

            this.wallSides[3].PushCollider.offset = new Vector2((mainAreaDimensions.x + 1) / 2f * FrigidConstants.UnitWorldSize, 0);
            this.wallSides[3].PushCollider.size = new Vector2(FrigidConstants.UnitWorldSize, (mainAreaDimensions.y + 2) * FrigidConstants.UnitWorldSize);
            this.wallSides[3].BreakBoxCollider.offset = new Vector2((mainAreaDimensions.x + 1) / 2f * FrigidConstants.UnitWorldSize + this.breakBoxInsetDistance / 2, 0);
            this.wallSides[3].BreakBoxCollider.size = new Vector2(FrigidConstants.UnitWorldSize - this.breakBoxInsetDistance, (mainAreaDimensions.y + 2) * FrigidConstants.UnitWorldSize);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        [Serializable]
        private struct WallSide
        {
            [SerializeField]
            private BoxCollider2D pushCollider;
            [SerializeField]
            private BoxCollider2D breakBoxCollider;

            public BoxCollider2D PushCollider
            {
                get
                {
                    return this.pushCollider;
                }
            }

            public BoxCollider2D BreakBoxCollider
            {
                get
                {
                    return this.breakBoxCollider;
                }
            }
        }
    }
}
