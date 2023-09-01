using UnityEngine;
using FrigidBlackwaters.Core;

using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobPathAndAvoidDirection : Direction
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private Targeter targetTargeter;
        [SerializeField]
        private TargetSamplingMode targetSamplingMode;
        [SerializeField]
        private FloatSerializedReference maxPercentPathLengthDelta;
        [SerializeField]
        private FloatSerializedReference stoppingDistance;
        [SerializeField]
        private bool ignoreTraversableTerrain;

        private PathfindingTask pathfindingTask;
        private Vector2 targetPosition;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currDirections;
            }

            Vector2[] directions = new Vector2[currDirections.Length];

            List<Vector2> pathPoints = this.pathfindingTask.RequestPathPoints(
                    mob.TiledArea,
                    mob.TileSize,
                    this.ignoreTraversableTerrain ? TraversableTerrain.All : mob.TraversableTerrain,
                    Resistance.None,
                    mob.Position,
                    this.targetPosition,
                    mob.Size / 2,
                    this.maxPercentPathLengthDelta.ImmutableValue
                    );
            Vector2 pathAndAvoidDirection = Vector2.zero;
            if (pathPoints.Count > 1 && Vector2.Distance(mob.Position, this.targetPosition) > this.stoppingDistance.ImmutableValue)
            {
                Vector2 pathDirection = (pathPoints[1] - mob.Position).normalized;
                Vector2 summedDirection = pathDirection;
                if (mob.CurrentPushMode == MobPushMode.IgnoreNone)
                {
                    foreach (Mob activeMob in Mob.GetActiveMobsIn(mob.TiledArea))
                    {
                        float avoidanceThresholdDistance = mob.Size.magnitude + activeMob.Size.magnitude;
                        float avoidanceDistance = Vector2.Distance(activeMob.Position, mob.Position);
                        if (activeMob != mob && activeMob.CurrentPushMode == MobPushMode.IgnoreNone && avoidanceDistance <= avoidanceThresholdDistance)
                        {
                            Vector2 avoidanceDirection = (activeMob.Position - mob.Position).normalized;
                            if (Vector2.Dot(avoidanceDirection, pathDirection) >= 0)
                            {
                                float weight = Mathf.Abs(Vector2.Dot(avoidanceDirection, pathDirection));
                                Vector2 partialDirection = weight * -avoidanceDirection * (avoidanceThresholdDistance - avoidanceDistance) / (avoidanceThresholdDistance / 2f);
                                summedDirection += partialDirection;
                            }
                        }
                    }
                }
                summedDirection.Normalize();
                if (Vector2.Dot(summedDirection, pathDirection) < 0)
                {
                    // Not allowed to go backwards. Worst case just stop.
                    summedDirection = Vector2.zero;
                }
                pathAndAvoidDirection = summedDirection;
            }

            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = pathAndAvoidDirection;
            }
            return directions;
        }

        protected override void Awake()
        {
            base.Awake();
            this.pathfindingTask = new PathfindingTask();
        }

        protected override void Start()
        {
            base.Start();
            FrigidCoroutine.Run(this.TargetUpdate(), this.gameObject);
        }

        private IEnumerator<FrigidCoroutine.Delay> TargetUpdate()
        {
            while (true)
            {
                if (this.mob.TryGetValue(out Mob mob))
                {
                    this.targetPosition = this.targetTargeter.Retrieve(mob.Position, 0, 0);
                    while (this.Retrieve(Vector2.zero, 0, 0).sqrMagnitude > 0 && this.targetSamplingMode == TargetSamplingMode.PathFinish)
                    {
                        yield return null;
                    }
                }
                yield return null;
            }
        }

        private enum TargetSamplingMode
        {
            Continuous,
            PathFinish
        }
    }
}
