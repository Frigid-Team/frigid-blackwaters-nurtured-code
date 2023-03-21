using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class DistanceFromTargetConditional : MagnitudeConditional
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private Targeter destinationTargeter;

        protected override float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            return Vector2.Distance(this.destinationTargeter.Calculate(Vector2.zero, 0, 0), this.originTargeter.Calculate(Vector2.zero, 0, 0));
        }
    }
}
