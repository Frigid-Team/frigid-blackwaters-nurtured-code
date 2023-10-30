using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class DistanceBetweenTargetersConditional : FloatComparisonConditional
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private Targeter destinationTargeter;

        protected override float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            return Vector2.Distance(this.destinationTargeter.Retrieve(Vector2.positiveInfinity, 0, 0), this.originTargeter.Retrieve(Vector2.zero, 0, 0));
        }
    }
}
