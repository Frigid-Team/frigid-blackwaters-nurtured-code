using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class MaterialTweenCoroutineTemplateSerializedReference : SerializedReference<MaterialTweenCoroutineTemplate>
    {
        public MaterialTweenCoroutineTemplateSerializedReference() : base() { }

        public MaterialTweenCoroutineTemplateSerializedReference(MaterialTweenCoroutineTemplateSerializedReference other) : base(other) { }

        public MaterialTweenCoroutineTemplateSerializedReference(SerializedReferenceType referenceType, MaterialTweenCoroutineTemplate customValue, ScriptableConstant<MaterialTweenCoroutineTemplate> scriptableConstant, List<MaterialTweenCoroutineTemplate> selection, ScriptableVariable<MaterialTweenCoroutineTemplate> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }

        protected override MaterialTweenCoroutineTemplate GetCopyValue(MaterialTweenCoroutineTemplate value) { return new MaterialTweenCoroutineTemplate(value); }

        protected override MaterialTweenCoroutineTemplate GetDefaultCustomValue() { return new MaterialTweenCoroutineTemplate(); }
    }
}
