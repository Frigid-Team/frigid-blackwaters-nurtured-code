using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class ResistanceSerializedReference : SerializedReference<Resistance>
    {
        public ResistanceSerializedReference() : base() { }

        public ResistanceSerializedReference(ResistanceSerializedReference other) : base(other) { }

        public ResistanceSerializedReference(SerializedReferenceType referenceType, Resistance customValue, ScriptableConstant<Resistance> scriptableConstant, List<Resistance> selection, ScriptableVariable<Resistance> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}
