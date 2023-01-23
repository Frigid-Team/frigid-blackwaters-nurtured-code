#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    [CustomInspectorFieldDrawer(typeof(ShowIfBoolAttribute))]
    public class ShowIfBoolAttributeFieldDrawer : ShowIfAttributeFieldDrawer
    {
        protected override bool EvaluateWithRootObject(ShowIfAttribute showIfAttribute, SerializedObject rootObject, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfBoolAttribute showIfBoolAttribute = (ShowIfBoolAttribute)showIfAttribute;
            SerializedProperty boolProperty = rootObject.FindProperty(showIfBoolAttribute.BoolPropertyPath);
            return boolProperty == null || boolProperty.boolValue;
        }

        protected override bool EvaluateWithRootProperty(ShowIfAttribute showIfAttribute, SerializedProperty rootProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfBoolAttribute showIfBoolAttribute = (ShowIfBoolAttribute)showIfAttribute;
            SerializedProperty boolProperty = rootProperty.FindPropertyRelative(showIfBoolAttribute.BoolPropertyPath);
            return boolProperty == null || boolProperty.boolValue;
        }
    }
}
#endif
