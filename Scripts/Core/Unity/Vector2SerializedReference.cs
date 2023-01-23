using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class Vector2SerializedReference : SerializedReference<Vector2>
    {
        public Vector2SerializedReference() : base() { }

        public Vector2SerializedReference(Vector2SerializedReference other) : base(other) { }

        public Vector2SerializedReference(SerializedReferenceType referenceType, Vector2 customValue, ScriptableConstant<Vector2> scriptableConstant, List<Vector2> selection, ScriptableVariable<Vector2> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}
