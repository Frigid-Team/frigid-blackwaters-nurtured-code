using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobCanPathToTargeterConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private Targeter targetTargeter;

        private PathfindingTask pathfindingTask;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return false;
            }

            Vector2 targetPosition = this.targetTargeter.Retrieve(Vector2.zero, elapsedDuration, elapsedDurationDelta);
            if (!mob.TiledArea.NavigationGrid.IsOnTerrain(AreaTiling.RectIndexPositionFromPosition(targetPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize), mob.TileSize, mob.TraversableTerrain))
            {
                return false;
            }
            if (AreaTiling.RectPositionWithinBounds(targetPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions, mob.TileSize))
            {
                List<Vector2> pathPoints = this.pathfindingTask.RequestPathPoints(mob.TiledArea, mob.TileSize, mob.TraversableTerrain, Resistance.None, mob.Position, targetPosition, mob.TileSize - mob.Size, 0);
                if (pathPoints.Count == 0)
                {
                    return true;
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
