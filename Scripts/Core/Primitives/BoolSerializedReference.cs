using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class BoolSerializedReference : SerializedReference<bool>
    {
        public BoolSerializedReference() : base() { }

        public BoolSerializedReference(BoolSerializedReference other) : base(other) { }

        public BoolSerializedReference(SerializedReferenceType referenceType, bool customValue, ScriptableConstant<bool> scriptableConstant, List<bool> selection, ScriptableVariable<bool> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}
