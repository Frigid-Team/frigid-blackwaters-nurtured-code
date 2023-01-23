using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class IntSerializedReference : SerializedReference<int>
    {
        [SerializeField]
        [ShowIfInt("referenceType", 2, true)]
        private int lowerValue;
        [SerializeField]
        [ShowIfPreviouslyShown(true)]
        private int upperValue;

        public IntSerializedReference() : base()
        {
            this.lowerValue = 0;
            this.upperValue = 0;
        }

        public IntSerializedReference(IntSerializedReference other) : base(other)
        {
            this.lowerValue = other.lowerValue;
            this.upperValue = other.upperValue;
        }

        public IntSerializedReference(
            SerializedReferenceType referenceType,
            int customValue,
            ScriptableConstant<int> scriptableConstant,
            int lowerValue, 
            int upperValue,
            List<int> selection,
            ScriptableVariable<int> scriptableVariable
            ) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable)
        {
            this.lowerValue = lowerValue;
            this.upperValue = upperValue;
        }

        public int LowerValue
        {
            get
            {
                return this.lowerValue;
            }
        }

        public int UpperValue
        {
            get
            {
                return this.upperValue;
            }
        }

        protected override int GetRandomFromRangeImmutableValue()
        {
            return new System.Random(GetHashCode()).Next(this.lowerValue, this.upperValue);
        }

        protected override int GetRandomFromRangeMutableValue()
        {
            return new System.Random().Next(this.lowerValue, this.upperValue);
        }

        protected override bool RangeEquals(SerializedReference<int> other)
        {
            IntSerializedReference otherCasted = (IntSerializedReference)other;
            return this.lowerValue == otherCasted.lowerValue && this.upperValue == otherCasted.upperValue;
        }

        protected override int GetHashCodeFromRange()
        {
            return (this.lowerValue, this.upperValue).GetHashCode();
        }
    }
}
