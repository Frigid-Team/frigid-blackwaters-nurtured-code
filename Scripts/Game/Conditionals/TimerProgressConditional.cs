using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TimerProgressConditional : MagnitudeConditional
    {
        [SerializeField]
        private Timer timer;

        protected override float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.timer.Progress;
        }
    }
}
