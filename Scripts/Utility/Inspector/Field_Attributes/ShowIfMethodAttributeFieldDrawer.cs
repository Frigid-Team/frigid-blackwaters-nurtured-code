#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    [CustomInspectorFieldDrawer(typeof(ShowIfMethodAttribute))]
    public class ShowIfMethodAttributeFieldDrawer : ShowIfAttributeFieldDrawer
    {
        protected override bool EvaluateWithRootObject(ShowIfAttribute showIfAttribute, SerializedObject rootObject, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfMethodAttribute showIfMethodAttribute = (ShowIfMethodAttribute)showIfAttribute;
            MethodInfo methodInfo = rootObject.targetObject.GetType().GetDerivedMethod(showIfMethodAttribute.MethodName, InspectorUtility.SearchFlags);
            return methodInfo == null || methodInfo.ReturnType != typeof(bool) || methodInfo.GetParameters().Length > 0 || (bool)methodInfo.Invoke(rootObject.targetObject, null);
        }

        protected override bool EvaluateWithRootProperty(ShowIfAttribute showIfAttribute, SerializedProperty rootProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            FieldInfo propertyFieldInfo = InspectorUtility.GetFieldFromSerializedProperty(rootProperty);
            ShowIfMethodAttribute showIfMethodAttribute = (ShowIfMethodAttribute)showIfAttribute;
            MethodInfo methodInfo = propertyFieldInfo.DeclaringType.GetDerivedMethod(showIfMethodAttribute.MethodName, InspectorUtility.SearchFlags);
            return methodInfo == null || methodInfo.ReturnType != typeof(bool) || methodInfo.GetParameters().Length > 0 || (bool)methodInfo.Invoke(InspectorUtility.GetObjectFromSerializedProperty(rootProperty), null);
        }
    }
}
#endif
