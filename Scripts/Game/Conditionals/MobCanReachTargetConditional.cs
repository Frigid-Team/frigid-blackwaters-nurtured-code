using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobCanReachTargetConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private Targeter reachTargeter;

        private PathfindingTask pathfindingTask;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            Mob mob = this.mob.ImmutableValue;
            Vector2 targetPosition = this.reachTargeter.Calculate(mob.Position, elapsedDuration, elapsedDurationDelta);
            if (!mob.TiledArea.NavigationGrid.IsOnTerrain(TilePositioning.RectIndicesFromPosition(targetPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize), mob.TileSize, mob.TraversableTerrain))
            {
                return false;
            }
            if (TilePositioning.RectPositionWithinBounds(targetPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize))
            {
                List<Vector2> pathPoints = this.pathfindingTask.RequestPathPoints(mob.TiledArea, mob.TileSize, mob.TraversableTerrain, mob.Position, targetPosition, mob.TileSize - mob.Size, 0);
                if (pathPoints.Count == 0)
                {
                    return false;
                }
                return Mathf.Abs(pathPoints[pathPoints.Count - 1].x - targetPosition.x) < mob.TileSize.x && Mathf.Abs(pathPoints[pathPoints.Count - 1].y - targetPosition.y) < mob.TileSize.y;
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
