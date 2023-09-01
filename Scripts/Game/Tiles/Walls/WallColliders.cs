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
            if (this.wallSides.Count == 4)
            {
                this.wallSides[0].PushCollider.offset = new Vector2(0, (mainAreaDimensions.y + 1) / 2f * FrigidConstants.UNIT_WORLD_SIZE);
                this.wallSides[0].PushCollider.size = new Vector2((mainAreaDimensions.x + 2) * FrigidConstants.UNIT_WORLD_SIZE, FrigidConstants.UNIT_WORLD_SIZE);
                this.wallSides[0].BreakBoxCollider.offset = new Vector2(0, (mainAreaDimensions.y + 1) / 2f * FrigidConstants.UNIT_WORLD_SIZE + this.breakBoxInsetDistance / 2);
                this.wallSides[0].BreakBoxCollider.size = new Vector2((mainAreaDimensions.x + 2) * FrigidConstants.UNIT_WORLD_SIZE, FrigidConstants.UNIT_WORLD_SIZE - this.breakBoxInsetDistance);

                this.wallSides[1].PushCollider.offset = new Vector2(0, (-mainAreaDimensions.y - 1) / 2f * FrigidConstants.UNIT_WORLD_SIZE);
                this.wallSides[1].PushCollider.size = new Vector2((mainAreaDimensions.x + 2) * FrigidConstants.UNIT_WORLD_SIZE, FrigidConstants.UNIT_WORLD_SIZE);
                this.wallSides[1].BreakBoxCollider.offset = new Vector2(0, (-mainAreaDimensions.y - 1) / 2f * FrigidConstants.UNIT_WORLD_SIZE - this.breakBoxInsetDistance / 2);
                this.wallSides[1].BreakBoxCollider.size = new Vector2((mainAreaDimensions.x + 2) * FrigidConstants.UNIT_WORLD_SIZE, FrigidConstants.UNIT_WORLD_SIZE - this.breakBoxInsetDistance);

                this.wallSides[2].PushCollider.offset = new Vector2((-mainAreaDimensions.x - 1) / 2f * FrigidConstants.UNIT_WORLD_SIZE, 0);
                this.wallSides[2].PushCollider.size = new Vector2(FrigidConstants.UNIT_WORLD_SIZE, (mainAreaDimensions.y + 2) * FrigidConstants.UNIT_WORLD_SIZE);
                this.wallSides[2].BreakBoxCollider.offset = new Vector2((-mainAreaDimensions.x - 1) / 2f * FrigidConstants.UNIT_WORLD_SIZE - this.breakBoxInsetDistance / 2, 0);
                this.wallSides[2].BreakBoxCollider.size = new Vector2(FrigidConstants.UNIT_WORLD_SIZE - this.breakBoxInsetDistance, (mainAreaDimensions.y + 2) * FrigidConstants.UNIT_WORLD_SIZE);

                this.wallSides[3].PushCollider.offset = new Vector2((mainAreaDimensions.x + 1) / 2f * FrigidConstants.UNIT_WORLD_SIZE, 0);
                this.wallSides[3].PushCollider.size = new Vector2(FrigidConstants.UNIT_WORLD_SIZE, (mainAreaDimensions.y + 2) * FrigidConstants.UNIT_WORLD_SIZE);
                this.wallSides[3].BreakBoxCollider.offset = new Vector2((mainAreaDimensions.x + 1) / 2f * FrigidConstants.UNIT_WORLD_SIZE + this.breakBoxInsetDistance / 2, 0);
                this.wallSides[3].BreakBoxCollider.size = new Vector2(FrigidConstants.UNIT_WORLD_SIZE - this.breakBoxInsetDistance, (mainAreaDimensions.y + 2) * FrigidConstants.UNIT_WORLD_SIZE);
            }
            else
            {
                Debug.LogError("Invalid number of wall sides in WallColliders " + this.name + ".");
            }
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
