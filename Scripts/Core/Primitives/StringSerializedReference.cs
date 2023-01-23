using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class StringSerializedReference : SerializedReference<string>
    {
        public StringSerializedReference() : base() { }

        public StringSerializedReference(StringSerializedReference other) : base(other) { }

        public StringSerializedReference(SerializedReferenceType referenceType, string customValue, ScriptableConstant<string> scriptableConstant, List<string> selection, ScriptableVariable<string> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}
