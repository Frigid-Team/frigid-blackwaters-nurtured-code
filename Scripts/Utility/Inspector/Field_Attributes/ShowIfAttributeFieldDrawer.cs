#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    public abstract class ShowIfAttributeFieldDrawer : InspectorFieldDrawer
    {
        public override void DrawInObjectInspector(SerializedObject rootObject, SerializedProperty drawProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords, out bool drawThrough)
        {
            drawThrough = this.IsShownInObjectInspector(rootObject, drawnFieldRecords);
        }

        public override float GetHeightInPropertyInspector(SerializedProperty rootProperty, SerializedProperty drawProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords, GUIContent label, out bool drawThrough)
        {
            drawThrough = this.IsShownInPropertyInspector(rootProperty, drawnFieldRecords);
            return 0;
        }

        public override void DrawInPropertyInspector(Rect position, SerializedProperty rootProperty, SerializedProperty drawProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords, GUIContent label, out bool drawThrough)
        {
            drawThrough = this.IsShownInPropertyInspector(rootProperty, drawnFieldRecords);
        }

        public bool IsShownInObjectInspector(SerializedObject rootObject, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfAttribute showIfAttribute = (ShowIfAttribute)this.Attribute;
            bool isShown = this.EvaluateWithRootObject(showIfAttribute, rootObject, drawnFieldRecords);
            return showIfAttribute.Evaluation && isShown || !showIfAttribute.Evaluation && !isShown;
        }

        public bool IsShownInPropertyInspector(SerializedProperty rootProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords)
        {
            ShowIfAttribute showIfAttribute = (ShowIfAttribute)this.Attribute;
            bool isShown;
            if (showIfAttribute.CheckEnclosedObject) isShown = this.EvaluateWithRootObject(showIfAttribute, rootProperty.serializedObject, drawnFieldRecords);
            else isShown = this.EvaluateWithRootProperty(showIfAttribute, rootProperty, drawnFieldRecords);
            return showIfAttribute.Evaluation && isShown || !showIfAttribute.Evaluation && !isShown;
        }

        protected abstract bool EvaluateWithRootObject(ShowIfAttribute showIfAttribute, SerializedObject rootObject, List<InspectorDrawnFieldRecord> drawnFieldRecords);

        protected abstract bool EvaluateWithRootProperty(ShowIfAttribute showIfAttribute, SerializedProperty rootProperty, List<InspectorDrawnFieldRecord> drawnFieldRecords);
    }
}
#endif
