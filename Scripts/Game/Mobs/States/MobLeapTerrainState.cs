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

        public override bool CanSetPosition
        {
            get
            {
                return false;
            }
        }

        public override bool CanSetTiledArea
        {
            get
            {
                return false;
            }
        }

        protected override void EnterSelf()
        {
            base.EnterSelf();
            StartLeap();
        }

        protected override void ExitSelf()
        {
            base.ExitSelf();
            CancelLeap();
        }

        private void BeginLeaping()
        {
            if (IsCrossable())
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
            if (!this.Owner.StopVelocities && this.Owner.SetForcedMove(this.moveByDashingForConstantDistance, null, BeginLeaping, EndLeaping))
            {
                this.Owner.StopVelocities.OnFirstRequest += CancelLeap;
            }
        }

        private void CancelLeap()
        {
            EndLeaping();
            if (this.Owner.ClearForcedMove(this.moveByDashingForConstantDistance))
            {
                if (this.crossingTerrain) this.Owner.RelocateToTraversableSpace();
                this.Owner.StopVelocities.OnFirstRequest -= CancelLeap;
            }
        }

        private bool IsCrossable()
        {
            Vector2 direction = this.moveByDashingForConstantDistance.DashDirection;
            float distance = this.moveByDashingForConstantDistance.DashDistance;

            if (direction == Vector2.zero) return false;

            Vector2Int startingPositionIndices = this.Owner.PositionIndices;
            Vector2Int destinationPositionIndices = TilePositioning.RectIndicesFromPosition(this.Owner.Position + direction * distance, this.Owner.TiledArea.CenterPosition, this.Owner.TiledArea.MainAreaDimensions, this.Owner.TileSize);

            List<Vector2Int> bresenhamLineIndices = Geometry.GetAllSquaresAlongLine(startingPositionIndices, destinationPositionIndices);

            Resistance highestResistance = Resistance.None;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.OwnerAnimatorBody.GetCurrentProperties<BreakBoxAnimatorProperty>())
            {
                highestResistance = (Resistance)Mathf.Max((int)breakBoxProperty.OffensiveResistance, (int)highestResistance);
            }
            foreach (Vector2Int lineIndices in bresenhamLineIndices)
            {
                if (lineIndices == startingPositionIndices) continue;

                if (!TilePositioning.RectIndicesWithinBounds(lineIndices, this.Owner.TiledArea.MainAreaDimensions, this.Owner.TileSize) ||
                    !this.Owner.TiledArea.NavigationGrid.IsTraversable(lineIndices, this.Owner.TileSize, TraversableTerrain.All, highestResistance) ||
                    lineIndices == destinationPositionIndices && !this.Owner.TiledArea.NavigationGrid.IsTraversable(destinationPositionIndices, this.Owner.TileSize, this.Owner.TraversableTerrain, highestResistance))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
