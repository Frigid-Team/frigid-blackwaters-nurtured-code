using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class NumericalComparisonConditional<T> : Conditional where T : IComparable<T>
    {
        [SerializeField]
        private ComparisonType comparisonType;

        protected sealed override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            switch (this.comparisonType)
            {
                case ComparisonType.EqualTo:
                    return this.GetComparisonValue(elapsedDuration, elapsedDurationDelta).CompareTo(this.GetTargetValue()) == 0;
                case ComparisonType.NotEqualTo:
                    return this.GetComparisonValue(elapsedDuration, elapsedDurationDelta).CompareTo(this.GetTargetValue()) != 0;
                case ComparisonType.GreaterThan:
                    return this.GetComparisonValue(elapsedDuration, elapsedDurationDelta).CompareTo(this.GetTargetValue()) > 0;
                case ComparisonType.LessThan:
                    return this.GetComparisonValue(elapsedDuration, elapsedDurationDelta).CompareTo(this.GetTargetValue()) < 0;
                case ComparisonType.GreaterEqualThan:
                    return this.GetComparisonValue(elapsedDuration, elapsedDurationDelta).CompareTo(this.GetTargetValue()) >= 0;
                case ComparisonType.LessEqualThan:
                    return this.GetComparisonValue(elapsedDuration, elapsedDurationDelta).CompareTo(this.GetTargetValue()) <= 0;
            }
            return false;
        }

        protected abstract T GetTargetValue();

        protected abstract T GetComparisonValue(float elapsedDuration, float elapsedDurationDelta);

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
