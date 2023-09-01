using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class IntComparisonConditional : NumericalComparisonConditional<int>
    {
        [SerializeField]
        private IntSerializedReference targetValue;

        protected override int GetTargetValue()
        {
            return this.targetValue.ImmutableValue;
        }
    }
}
