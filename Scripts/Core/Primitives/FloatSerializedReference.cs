using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class FloatSerializedReference : SerializedReference<float>
    {
        [SerializeField]
        [ShowIfInt("referenceType", 2, true)]
        private float lowerValue;
        [SerializeField]
        [ShowIfPreviouslyShown(true)]
        private float upperValue;

        public FloatSerializedReference() : base()
        {
            this.lowerValue = 0;
            this.upperValue = 0;
        }

        public FloatSerializedReference(FloatSerializedReference other) : base(other)
        {
            this.lowerValue = other.lowerValue;
            this.upperValue = other.upperValue;
        }

        public FloatSerializedReference(
            SerializedReferenceType referenceType,
            float customValue,
            ScriptableConstant<float> scriptableConstant,
            float lowerValue,
            float upperValue,
            List<float> selection,
            ScriptableVariable<float> scriptableVariable
            ) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable)
        {
            this.lowerValue = lowerValue;
            this.upperValue = upperValue;
        }

        public float LowerValue
        {
            get
            {
                return this.lowerValue;
            }
        }

        public float UpperValue
        {
            get
            {
                return this.upperValue;
            }
        }

        protected override float GetRandomFromRangeImmutableValue()
        {
            return this.lowerValue + ((float)new System.Random(GetHashCode()).NextDouble()) * (this.upperValue - this.lowerValue);
        }

        protected override float GetRandomFromRangeMutableValue()
        {
            return this.lowerValue + ((float)new System.Random().NextDouble()) * (this.upperValue - this.lowerValue);
        }

        protected override bool RangeEquals(SerializedReference<float> other)
        {
            FloatSerializedReference otherCasted = (FloatSerializedReference)other;
            return this.lowerValue == otherCasted.lowerValue && this.upperValue == otherCasted.upperValue;
        }

        protected override int GetHashCodeFromRange()
        {
            return (this.lowerValue, this.upperValue).GetHashCode();
        }
    }
}
