#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    [CustomInspectorFieldDrawer(typeof(ShowIfFloatAttribute))]
    public class ShowIfFloatAttributeFieldDrawer : ShowIfAttributeFieldDrawer
    {
        protected override bool EvaluateWithRootObject(ShowIfAttribute showIfAttribute, SerializedObject rootObject, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfFloatAttribute showIfFloatAttribute = (ShowIfFloatAttribute)showIfAttribute;
            SerializedProperty floatProperty = rootObject.FindProperty(showIfFloatAttribute.FloatPropertyPath);
            return floatProperty == null || floatProperty.floatValue >= showIfFloatAttribute.MinValue && floatProperty.floatValue <= showIfFloatAttribute.MaxValue;
        }

        protected override bool EvaluateWithRootProperty(ShowIfAttribute showIfAttribute, SerializedProperty rootProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfFloatAttribute showIfFloatAttribute = (ShowIfFloatAttribute)showIfAttribute;
            SerializedProperty floatProperty = rootProperty.FindPropertyRelative(showIfFloatAttribute.FloatPropertyPath);
            return floatProperty == null || floatProperty.floatValue >= showIfFloatAttribute.MinValue && floatProperty.floatValue <= showIfFloatAttribute.MaxValue;
        }
    }
}
#endif
