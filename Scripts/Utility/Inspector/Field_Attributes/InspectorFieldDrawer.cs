#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    public abstract class InspectorFieldDrawer
    {
        private InspectorFieldAttribute attribute;

        public InspectorFieldDrawer Copy(InspectorFieldAttribute attribute)
        {
            InspectorFieldDrawer copiedFieldDrawer = (InspectorFieldDrawer)this.MemberwiseClone();
            copiedFieldDrawer.attribute = attribute;
            return copiedFieldDrawer;
        }

        public InspectorFieldAttribute Attribute
        {
            get
            {
                return this.attribute;
            }
        }

        public virtual void DrawInObjectInspector(SerializedObject rootObject, SerializedProperty drawProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords, out bool drawThrough)
        {
            drawThrough = false;
            EditorGUILayout.PropertyField(drawProperty);
        }

        public virtual float GetHeightInPropertyInspector(SerializedProperty rootProperty, SerializedProperty drawProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords, GUIContent label, out bool drawThrough)
        {
            drawThrough = false;
            return EditorGUI.GetPropertyHeight(drawProperty, true);
        }

        public virtual void DrawInPropertyInspector(Rect position, SerializedProperty rootProperty, SerializedProperty drawProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords, GUIContent label, out bool drawThrough)
        {
            drawThrough = false;
            EditorGUI.PropertyField(position, drawProperty, true);
        }
    }
}
#endif
