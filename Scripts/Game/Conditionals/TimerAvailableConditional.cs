using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TimerAvailableConditional : Conditional
    {
        [SerializeField]
        private Timer timer;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.timer.Available;
        }
    }
}
