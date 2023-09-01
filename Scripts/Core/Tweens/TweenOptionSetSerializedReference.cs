using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class TweenOptionSetSerializedReference : SerializedReference<TweenOptionSet>
    {
        public TweenOptionSetSerializedReference() : base() { }

        public TweenOptionSetSerializedReference(TweenOptionSetSerializedReference other) : base(other) { }

        public TweenOptionSetSerializedReference(SerializedReferenceType referenceType, TweenOptionSet customValue, ScriptableConstant<TweenOptionSet> scriptableConstant, List<TweenOptionSet> selection, ScriptableVariable<TweenOptionSet> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }

        protected override TweenOptionSet GetCopyValue(TweenOptionSet value) { return new TweenOptionSet(value); }

        protected override TweenOptionSet GetDefaultCustomValue() { return new TweenOptionSet(); }
    }
}
