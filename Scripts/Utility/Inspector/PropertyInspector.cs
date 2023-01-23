#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    [CustomPropertyDrawer(typeof(object), true)]
    public class PropertyInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (InspectorUtility.IsBaseSerializedProperty(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (property.isExpanded) 
            {
                EditorGUI.indentLevel++;
                int separators = property.propertyPath.Count(c => c == '.');
                List<InspectorDrawnFieldRecord> drawnFieldRecords = new List<InspectorDrawnFieldRecord>();
                foreach (SerializedProperty childProperty in property.Copy())
                {
                    if (childProperty.propertyPath.Count((char c) => c == '.') == separators + 1)
                    {
                        List<InspectorFieldDrawer> fieldDrawers = InspectorUtility.CreateFieldDrawersForSerializedProperty(childProperty);
                        List<InspectorFieldDrawer> drawnFieldDrawers = new List<InspectorFieldDrawer>();
                        bool drawThrough = true;
                        float origY = position.y;
                        foreach (InspectorFieldDrawer fieldDrawer in fieldDrawers)
                        {
                            float drawerHeight = fieldDrawer.GetHeightInPropertyInspector(property, childProperty, drawnFieldRecords, label, out drawThrough);
                            position.height = drawerHeight;
                            fieldDrawer.DrawInPropertyInspector(position, property, childProperty, drawnFieldRecords, label, out drawThrough);
                            position.y += drawerHeight;
                            drawnFieldDrawers.Add(fieldDrawer);
                            if (!drawThrough)
                            {
                                break;
                            }
                        }
                        if (drawThrough)
                        {
                            float propertyHeight = EditorGUI.GetPropertyHeight(childProperty, true);
                            position.height = propertyHeight;
                            EditorGUI.PropertyField(position, childProperty, true);
                            position.y += propertyHeight;
                        }
                        if (position.y - origY > 0)
                        {
                            position.y += EditorGUIUtility.standardVerticalSpacing;
                        }
                        drawnFieldRecords.Add(new InspectorDrawnFieldRecord(drawnFieldDrawers));
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (InspectorUtility.IsBaseSerializedProperty(property))
            {
                return EditorGUI.GetPropertyHeight(property, true);
            }

            float totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (property.isExpanded)
            {
                int numberDrawn = 0;
                int separators = property.propertyPath.Count((char c) => c == '.');
                List<InspectorDrawnFieldRecord> drawnFieldRecords = new List<InspectorDrawnFieldRecord>();
                foreach (SerializedProperty childProperty in property.Copy())
                {
                    if (childProperty.propertyPath.Count((char c) => c == '.') == separators + 1)
                    {
                        List<InspectorFieldDrawer> fieldDrawers = InspectorUtility.CreateFieldDrawersForSerializedProperty(childProperty);
                        List<InspectorFieldDrawer> drawnFieldDrawers = new List<InspectorFieldDrawer>();
                        bool drawThrough = true;
                        float childHeight = 0;
                        foreach (InspectorFieldDrawer fieldDrawer in fieldDrawers)
                        {
                            childHeight += fieldDrawer.GetHeightInPropertyInspector(property, childProperty, drawnFieldRecords, label, out drawThrough);
                            drawnFieldDrawers.Add(fieldDrawer);
                            if (!drawThrough)
                            {
                                break;
                            }
                        }
                        if (drawThrough)
                        {
                            childHeight += EditorGUI.GetPropertyHeight(childProperty, true);
                        }
                        if (childHeight > 0)
                        {
                            totalHeight += childHeight;
                            totalHeight += EditorGUIUtility.standardVerticalSpacing;
                        }
                        drawnFieldRecords.Add(new InspectorDrawnFieldRecord(drawnFieldDrawers));
                        numberDrawn++;
                    }
                }

                if (numberDrawn == 0)
                {
                    totalHeight += EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return totalHeight;
        }
    }
}
#endif
