using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobLeapTerrainState : MobActionState
    {
        [SerializeField]
        private MoveByDashingForConstantDistance moveByDashingForConstantDistance;

        private bool crossingTerrain;

        public override bool MovePositionSafe
        {
            get
            {
                return false;
            }
        }

        public override bool MoveTiledAreaSafe
        {
            get
            {
                return false;
            }
        }

        public override void EnterSelf()
        {
            base.EnterSelf();
            this.StartLeap();
        }

        public override void ExitSelf()
        {
            base.ExitSelf();
            this.CancelLeap();
        }

        private void BeginLeaping()
        {
            if (this.IsCrossable())
            {
                this.crossingTerrain = true;
                this.Owner.RequestPushMode(MobPushMode.IgnoreMobsAndTerrain);
            }
        }

        private void EndLeaping()
        {
            if (this.crossingTerrain)
            {
                this.crossingTerrain = false;
                this.Owner.ReleasePushMode(MobPushMode.IgnoreMobsAndTerrain);
            }
        }

        private void StartLeap()
        {
            this.crossingTerrain = false;
            if (!this.Owner.StopVelocities && this.Owner.SetForcedMove(this.moveByDashingForConstantDistance, null, this.BeginLeaping, this.EndLeaping))
            {
                this.Owner.StopVelocities.OnFirstRequest += this.CancelLeap;
            }
        }

        private void CancelLeap()
        {
            this.EndLeaping();
            if (this.Owner.ClearForcedMove(this.moveByDashingForConstantDistance))
            {
                if (this.crossingTerrain) this.Owner.RelocateToTraversableSpace();
                this.Owner.StopVelocities.OnFirstRequest -= this.CancelLeap;
            }
        }

        private bool IsCrossable()
        {
            Vector2 direction = this.moveByDashingForConstantDistance.DashDirection;
            float distance = this.moveByDashingForConstantDistance.DashDistance;

            if (direction == Vector2.zero) return false;

            Vector2Int startingIndexPosition = this.Owner.IndexPosition;
            Vector2Int destinationIndexPosition = AreaTiling.RectIndexPositionFromPosition(this.Owner.Position + direction * distance, this.Owner.TiledArea.CenterPosition, this.Owner.TiledArea.MainAreaDimensions, this.Owner.TileSize);

            List<Vector2Int> lineIndexPositions = Geometry.GetAllSquaresAlongLine(startingIndexPosition, destinationIndexPosition);

            Resistance highestResistance = Resistance.None;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.OwnerAnimatorBody.GetCurrentReferencedProperties<BreakBoxAnimatorProperty>())
            {
                highestResistance = (Resistance)Mathf.Max((int)breakBoxProperty.OffensiveResistance, (int)highestResistance);
            }
            foreach (Vector2Int lineIndexPosition in lineIndexPositions)
            {
                if (lineIndexPosition == startingIndexPosition) continue;

                if (!AreaTiling.RectIndexPositionWithinBounds(lineIndexPosition, this.Owner.TiledArea.MainAreaDimensions, this.Owner.TileSize) ||
                    !this.Owner.TiledArea.NavigationGrid.IsTraversable(lineIndexPosition, this.Owner.TileSize, TraversableTerrain.All, highestResistance) ||
                    lineIndexPosition == destinationIndexPosition && !this.Owner.TiledArea.NavigationGrid.IsTraversable(destinationIndexPosition, this.Owner.TileSize, this.Owner.TraversableTerrain, highestResistance))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
