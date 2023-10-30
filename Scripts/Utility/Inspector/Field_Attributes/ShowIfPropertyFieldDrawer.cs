#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    [CustomInspectorFieldDrawer(typeof(ShowIfPropertyAttribute))]
    public class ShowIfPropertyFieldDrawer : ShowIfAttributeFieldDrawer
    {
        protected override bool EvaluateWithRootObject(ShowIfAttribute showIfAttribute, SerializedObject rootObject, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfPropertyAttribute showIfPropertyAttribute = (ShowIfPropertyAttribute)showIfAttribute;
            PropertyInfo propertyInfo = rootObject.targetObject.GetType().GetDerivedProperty(showIfPropertyAttribute.PropertyName, InspectorUtility.SearchFlags);
            return propertyInfo == null || propertyInfo.PropertyType != typeof(bool) || (bool)propertyInfo.GetValue(rootObject.targetObject);
        }

        protected override bool EvaluateWithRootProperty(ShowIfAttribute showIfAttribute, SerializedProperty rootProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            FieldInfo propertyFieldInfo = InspectorUtility.GetFieldFromSerializedProperty(rootProperty);
            ShowIfPropertyAttribute showIfPropertyAttribute = (ShowIfPropertyAttribute)showIfAttribute;
            PropertyInfo propertyInfo = propertyFieldInfo.DeclaringType.GetDerivedProperty(showIfPropertyAttribute.PropertyName, InspectorUtility.SearchFlags);
            return propertyInfo == null || propertyInfo.PropertyType != typeof(bool) || (bool)propertyInfo.GetValue(InspectorUtility.GetObjectFromSerializedProperty(rootProperty));
        }
    }
}
#endif
