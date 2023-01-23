using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class ColorSerializedReference : SerializedReference<Color>
    {
        public ColorSerializedReference() : base() { }

        public ColorSerializedReference(ColorSerializedReference other) : base(other) { }

        public ColorSerializedReference(SerializedReferenceType referenceType, Color customValue, ScriptableConstant<Color> scriptableConstant, List<Color> selection, ScriptableVariable<Color> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}
