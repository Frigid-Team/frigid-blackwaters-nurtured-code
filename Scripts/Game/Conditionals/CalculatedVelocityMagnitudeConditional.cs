using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class CalculatedVelocityMagnitudeConditional : MagnitudeConditional
    {
        [SerializeField]
        private Mover mover;

        protected override float GetComparisonValue()
        {
            return this.mover.CalculatedVelocity.magnitude;
        }
    }
}
