using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class FloatComparisonConditional : NumericalComparisonConditional<float>
    {
        [SerializeField]
        private FloatSerializedReference targetValue;

        protected override float GetTargetValue()
        {
            return this.targetValue.ImmutableValue;
        }
    }
}
