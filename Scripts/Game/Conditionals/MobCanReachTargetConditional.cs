using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobCanReachTargetConditional : Conditional
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private Targeter reachTargeter;
        [SerializeField]
        private FloatSerializedReference exitExtent;
        [SerializeField]
        private FloatSerializedReference percentPathExtensionForPathFind;
        [SerializeField]
        private FloatSerializedReference acceptableDistanceAway;

        private PathfindingTask pathfindingTask;

        protected override bool CustomValidate()
        {
            Vector2 absoluteTargetPosition = this.reachTargeter.Calculate(this.mob.AbsolutePosition, 0, 0);
            if (TilePositioning.RectAbsolutePositionWithinBounds(absoluteTargetPosition, this.mob.TiledArea.AbsoluteCenterPosition, this.mob.TiledArea.MainAreaDimensions, this.mob.TileSize))
            {
                List<Vector2> pathPoints = this.pathfindingTask.RequestPathPoints(this.mob.TiledArea, this.mob.TileSize, this.mob.TraversableTerrain, this.mob.AbsolutePosition, absoluteTargetPosition, this.exitExtent.ImmutableValue, this.percentPathExtensionForPathFind.ImmutableValue);
                if (pathPoints.Count == 0)
                {
                    return false;
                }
                return Vector2.Distance(pathPoints[pathPoints.Count - 1], absoluteTargetPosition) < this.acceptableDistanceAway.ImmutableValue;
            }
            return false;
        }

        protected override void Awake()
        {
            base.Awake();
            this.pathfindingTask = new PathfindingTask();
        }
    }
}
