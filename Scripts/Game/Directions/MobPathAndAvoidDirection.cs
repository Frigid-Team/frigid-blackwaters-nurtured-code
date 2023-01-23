using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobPathAndAvoidDirection : Direction
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private Targeter targetTargeter;
        [SerializeField]
        private FloatSerializedReference exitExtent;
        [SerializeField]
        private FloatSerializedReference percentPathExtensionForPathFind;
        [SerializeField]
        private FloatSerializedReference avoidanceDetectionDistance;

        private PathfindingTask pathfindingTask;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] directions = new Vector2[currDirections.Length];

            Vector2[] currentPositions = new Vector2[directions.Length];
            for (int i = 0; i < currentPositions.Length; i++) currentPositions[i] = this.mob.AbsolutePosition;

            Vector2[] pathPositions = this.targetTargeter.Calculate(currentPositions, elapsedDuration, elapsedDurationDelta);

            for (int i = 0; i < directions.Length; i++)
            {
                List<Vector2> pathPoints = this.pathfindingTask.RequestPathPoints(
                    this.mob.TiledArea,
                    this.mob.TileSize,
                    this.mob.TraversableTerrain,
                    this.mob.AbsolutePosition,
                    pathPositions[i],
                    this.exitExtent.ImmutableValue,
                    this.percentPathExtensionForPathFind.ImmutableValue
                    );

                directions[i] = Vector2.zero;
                if (pathPoints.Count > 0)
                {
                    Vector2 pathDirection = pathPoints[Mathf.Min(pathPoints.Count - 1, 1)] - this.mob.AbsolutePosition;
                    Vector2 summedDirection = pathDirection;
                    if (this.mob.Physicality.CurrentMode == MobPushMode.IgnoreNone)
                    {
                        foreach (Mob activeMob in Mob.GetMobsInTiledArea(this.mob.TiledArea).ActiveMobs)
                        {
                            if (activeMob != this.mob && activeMob.Physicality.CurrentMode == MobPushMode.IgnoreNone)
                            {
                                Vector2 offsetDirection = activeMob.AbsolutePosition - this.mob.AbsolutePosition;
                                float angle = Vector2.Angle(offsetDirection, pathDirection);
                                float avoidanceDistance = this.mob.TileSize.magnitude + activeMob.TileSize.magnitude + this.avoidanceDetectionDistance.ImmutableValue;
                                if (angle <= 90 && offsetDirection.magnitude <= avoidanceDistance)
                                {
                                    float weightMultiplier = 1 - Mathf.Abs(Vector2.Dot(offsetDirection, pathDirection));
                                    Vector2 avoidanceDirection = weightMultiplier * -offsetDirection.normalized * (avoidanceDistance - offsetDirection.magnitude) / (avoidanceDistance * 0.5f);
                                    summedDirection += avoidanceDirection;
                                    summedDirection.Normalize();
                                }
                            }
                        }
                    }
                    directions[i] = summedDirection;
                }
            }

            return directions;
        }

        protected override void Awake()
        {
            base.Awake();
            this.pathfindingTask = new PathfindingTask();
        }
    }
}
