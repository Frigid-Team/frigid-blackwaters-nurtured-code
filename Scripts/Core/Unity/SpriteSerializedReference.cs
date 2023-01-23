using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class SpriteSerializedReference : SerializedReference<Sprite>
    {
        public SpriteSerializedReference() : base() { }

        public SpriteSerializedReference(SpriteSerializedReference other) : base(other) { }

        public SpriteSerializedReference(SerializedReferenceType referenceType, Sprite customValue, ScriptableConstant<Sprite> scriptableConstant, List<Sprite> selection, ScriptableVariable<Sprite> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}
