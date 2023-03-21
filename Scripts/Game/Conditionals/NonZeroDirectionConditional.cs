using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class NonZeroDirectionConditional : Conditional
    {
        [SerializeField]
        private Direction direction;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.direction.Calculate(Vector2.zero, elapsedDuration, elapsedDurationDelta).magnitude > 0;
        }
    }
}
