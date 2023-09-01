using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class CalculatedVelocityMagnitudeConditional : FloatComparisonConditional
    {
        [SerializeField]
        private Mover mover;

        protected override float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mover.CalculatedVelocity.magnitude;
        }
    }
}
