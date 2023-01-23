using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class AnimationCurveSerializedReference : SerializedReference<AnimationCurve>
    {
        public AnimationCurveSerializedReference() : base() { }

        public AnimationCurveSerializedReference(AnimationCurveSerializedReference other) : base(other) { }

        public AnimationCurveSerializedReference(SerializedReferenceType referenceType, AnimationCurve customValue, ScriptableConstant<AnimationCurve> scriptableConstant, List<AnimationCurve> selection, ScriptableVariable<AnimationCurve> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }

        protected override AnimationCurve GetCopyValue(AnimationCurve value) { return new AnimationCurve(value.keys); }

        protected override AnimationCurve GetDefaultCustomValue() { return new AnimationCurve(); }
    }
}
