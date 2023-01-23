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

        protected abstract float GetComparisonValue();

        protected override bool CustomValidate()
        {
            switch (this.comparisonType)
            {
                case ComparisonType.EqualTo:
                    return GetComparisonValue() == this.targetValue.ImmutableValue;
                case ComparisonType.NotEqualTo:
                    return GetComparisonValue() != this.targetValue.ImmutableValue;
                case ComparisonType.GreaterThan:
                    return GetComparisonValue() > this.targetValue.ImmutableValue;
                case ComparisonType.LessThan:
                    return GetComparisonValue() < this.targetValue.ImmutableValue;
                case ComparisonType.GreaterEqualThan:
                    return GetComparisonValue() >= this.targetValue.ImmutableValue;
                case ComparisonType.LessEqualThan:
                    return GetComparisonValue() <= this.targetValue.ImmutableValue;
            }
            return false;
        }

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
