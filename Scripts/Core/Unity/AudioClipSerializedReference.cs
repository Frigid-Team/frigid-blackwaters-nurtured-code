using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class AudioClipSerializedReference : SerializedReference<AudioClip>
    {
        public AudioClipSerializedReference() : base() { }

        public AudioClipSerializedReference(AudioClipSerializedReference other) : base(other) { }
        
        public AudioClipSerializedReference(SerializedReferenceType referenceType, AudioClip customValue, ScriptableConstant<AudioClip> scriptableConstant, List<AudioClip> selection, ScriptableVariable<AudioClip> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}
