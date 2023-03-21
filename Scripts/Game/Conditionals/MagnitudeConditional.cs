using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class MagnitudeConditional : Conditional
    {
        [SerializeField]
        private ComparisonType comparisonType;
        [SerializeField]
        private FloatSerializedReference targetValue;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            switch (this.comparisonType)
            {
                case ComparisonType.EqualTo:
                    return GetComparisonValue(elapsedDuration, elapsedDurationDelta) == this.targetValue.ImmutableValue;
                case ComparisonType.NotEqualTo:
                    return GetComparisonValue(elapsedDuration, elapsedDurationDelta) != this.targetValue.ImmutableValue;
                case ComparisonType.GreaterThan:
                    return GetComparisonValue(elapsedDuration, elapsedDurationDelta) > this.targetValue.ImmutableValue;
                case ComparisonType.LessThan:
                    return GetComparisonValue(elapsedDuration, elapsedDurationDelta) < this.targetValue.ImmutableValue;
                case ComparisonType.GreaterEqualThan:
                    return GetComparisonValue(elapsedDuration, elapsedDurationDelta) >= this.targetValue.ImmutableValue;
                case ComparisonType.LessEqualThan:
                    return GetComparisonValue(elapsedDuration, elapsedDurationDelta) <= this.targetValue.ImmutableValue;
            }
            return false;
        }

        protected abstract float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta);

        private enum ComparisonType
        {
            GreaterThan,
            LessThan,
            GreaterEqualThan,
            LessEqualThan,
            EqualTo,
            NotEqualTo
        }
    }
}
