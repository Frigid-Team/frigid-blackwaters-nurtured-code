#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    [CustomInspectorFieldDrawer(typeof(ShowIfIntAttribute))]
    public class ShowIfIntAttributeFieldDrawer : ShowIfAttributeFieldDrawer
    {
        protected override bool EvaluateWithRootObject(ShowIfAttribute showIfAttribute, SerializedObject rootObject, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfIntAttribute showIfIntAttribute = (ShowIfIntAttribute)showIfAttribute;
            SerializedProperty intProperty = rootObject.FindProperty(showIfIntAttribute.IntPropertyPath);
            return intProperty == null || intProperty.intValue >= showIfIntAttribute.MinValue && intProperty.intValue <= showIfIntAttribute.MaxValue;
        }

        protected override bool EvaluateWithRootProperty(ShowIfAttribute showIfAttribute, SerializedProperty rootProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfIntAttribute showIfIntAttribute = (ShowIfIntAttribute)showIfAttribute;
            SerializedProperty intProperty = rootProperty.FindPropertyRelative(showIfIntAttribute.IntPropertyPath);
            return intProperty == null || intProperty.intValue >= showIfIntAttribute.MinValue && intProperty.intValue <= showIfIntAttribute.MaxValue;
        }
    }
}
#endif
