using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class MaterialTweenOptionSetSerializedReference : SerializedReference<MaterialTweenOptionSet>
    {
        public MaterialTweenOptionSetSerializedReference() : base() { }

        public MaterialTweenOptionSetSerializedReference(MaterialTweenOptionSetSerializedReference other) : base(other) { }

        public MaterialTweenOptionSetSerializedReference(SerializedReferenceType referenceType, MaterialTweenOptionSet customValue, ScriptableConstant<MaterialTweenOptionSet> scriptableConstant, List<MaterialTweenOptionSet> selection, ScriptableVariable<MaterialTweenOptionSet> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }

        protected override MaterialTweenOptionSet GetCopyValue(MaterialTweenOptionSet value) { return new MaterialTweenOptionSet(value); }

        protected override MaterialTweenOptionSet GetDefaultCustomValue() { return new MaterialTweenOptionSet(); }
    }
}
