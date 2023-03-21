#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class ObjectInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            List<InspectorDrawnFieldRecord> drawnFieldRecords = new List<InspectorDrawnFieldRecord>();
            this.serializedObject.Update();
            using (var iterator = this.serializedObject.GetIterator())
            {
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        if (iterator.name.Equals("m_Script", System.StringComparison.Ordinal))
                        {
                            using (new EditorGUI.DisabledScope(true))
                            {
                                EditorGUILayout.PropertyField(iterator);
                            }
                        }
                        else
                        {
                            List<InspectorFieldDrawer> fieldDrawers = InspectorUtility.CreateFieldDrawersForSerializedProperty(iterator);
                            List<InspectorFieldDrawer> drawnFieldDrawers = new List<InspectorFieldDrawer>();
                            bool drawThrough = true;
                            foreach (InspectorFieldDrawer fieldDrawer in fieldDrawers)
                            {
                                fieldDrawer.DrawInObjectInspector(this.serializedObject, iterator, drawnFieldRecords, out drawThrough);
                                drawnFieldDrawers.Add(fieldDrawer);
                                if (!drawThrough)
                                {
                                    break;
                                }
                            }
                            if (drawThrough && iterator != null)
                            {
                                EditorGUILayout.PropertyField(iterator, true);
                            }
                            drawnFieldRecords.Add(new InspectorDrawnFieldRecord(fieldDrawers));
                        }
                    }
                    while (iterator.NextVisible(false));
                }
            }
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
