#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility 
{
    [CustomInspectorFieldDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeFieldDrawer : InspectorFieldDrawer
    {
        public override void DrawInObjectInspector(SerializedObject parentObject, SerializedProperty drawProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords, out bool drawThrough)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                base.DrawInObjectInspector(parentObject, drawProperty, drawnFieldRecords, out drawThrough);
            }
        }

        public override void DrawInPropertyInspector(Rect position, SerializedProperty parentProperty, SerializedProperty drawProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords, GUIContent label,  out bool drawThrough)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                base.DrawInPropertyInspector(position, parentProperty, drawProperty, drawnFieldRecords, label, out drawThrough);
            }
        }
    }
}
#endif
