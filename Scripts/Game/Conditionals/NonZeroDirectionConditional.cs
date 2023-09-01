using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class NonZeroDirectionConditional : Conditional
    {
        [SerializeField]
        private Direction direction;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.direction.Retrieve(Vector2.zero, elapsedDuration, elapsedDurationDelta).magnitude > 0;
        }
    }
}
