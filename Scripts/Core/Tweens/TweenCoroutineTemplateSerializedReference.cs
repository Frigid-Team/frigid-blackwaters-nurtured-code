using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class TweenCoroutineTemplateSerializedReference : SerializedReference<TweenCoroutineTemplate>
    {
        public TweenCoroutineTemplateSerializedReference() : base() { }

        public TweenCoroutineTemplateSerializedReference(TweenCoroutineTemplateSerializedReference other) : base(other) { }

        public TweenCoroutineTemplateSerializedReference(SerializedReferenceType referenceType, TweenCoroutineTemplate customValue, ScriptableConstant<TweenCoroutineTemplate> scriptableConstant, List<TweenCoroutineTemplate> selection, ScriptableVariable<TweenCoroutineTemplate> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }

        protected override TweenCoroutineTemplate GetCopyValue(TweenCoroutineTemplate value) { return new TweenCoroutineTemplate(value); }

        protected override TweenCoroutineTemplate GetDefaultCustomValue() { return new TweenCoroutineTemplate(); }
    }
}
