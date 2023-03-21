using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobPathAndAvoidDirection : Direction
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private Targeter targetTargeter;
        [SerializeField]
        private FloatSerializedReference percentPathExtensionForPathFind;
        [SerializeField]
        private FloatSerializedReference stoppingDistance;

        private PathfindingTask pathfindingTask;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Mob mob = this.mob.ImmutableValue;

            Vector2[] directions = new Vector2[currDirections.Length];

            Vector2[] currentPositions = new Vector2[directions.Length];
            for (int i = 0; i < currentPositions.Length; i++) currentPositions[i] = mob.Position;

            Vector2[] targetPositions = this.targetTargeter.Calculate(currentPositions, elapsedDuration, elapsedDurationDelta);

            for (int i = 0; i < directions.Length; i++)
            {
                List<Vector2> pathPoints = this.pathfindingTask.RequestPathPoints(
                    mob.TiledArea,
                    mob.TileSize,
                    mob.TraversableTerrain,
                    mob.Position,
                    targetPositions[i],
                    mob.Size / 2,
                    this.percentPathExtensionForPathFind.ImmutableValue
                    );

                directions[i] = Vector2.zero;
                if (pathPoints.Count > 1 && Vector2.Distance(mob.Position, targetPositions[i]) > this.stoppingDistance.ImmutableValue)
                {
                    Vector2 pathDirection = pathPoints[1] - mob.Position;
                    Vector2 summedDirection = pathDirection.normalized;
                    if (mob.CurrentPushMode == MobPushMode.IgnoreNone)
                    {
                        foreach (Mob activeMob in Mob.GetActiveMobsIn(mob.TiledArea))
                        {
                            if (activeMob != mob && activeMob.CurrentPushMode == MobPushMode.IgnoreNone)
                            {
                                Vector2 offsetDirection = activeMob.Position - mob.Position;
                                float angle = Vector2.Angle(offsetDirection, pathDirection);
                                float avoidanceDistance = mob.Size.magnitude + activeMob.Size.magnitude;
                                if (angle <= 90 && offsetDirection.magnitude <= avoidanceDistance)
                                {
                                    float weightMultiplier = Mathf.Abs(Vector2.Dot(offsetDirection, pathDirection));
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
