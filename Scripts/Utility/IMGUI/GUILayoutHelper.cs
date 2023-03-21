#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    public static class GUILayoutHelper
    {
        public static void DrawIndexedList(int count, Action<int> onAdd, Action<int> onRemove, Action<int> onDrawElement)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("+"))
                    {
                        onAdd.Invoke(0);
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        onDrawElement.Invoke(i);
                        if (GUILayout.Button("+"))
                        {
                            onAdd.Invoke(i + 1);
                            break;
                        }
                        if (GUILayout.Button("-"))
                        {
                            onRemove.Invoke(i);
                            break;
                        }
                    }
                }
            }
        }

        public static UnityEngine.Object[] DragAndDropBox(string label, float width, float height, bool allowMultiple, Type type, params GUILayoutOption[] options)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.Space();
                Rect lastRect = GUILayoutUtility.GetLastRect();
                Rect dragAndDropRect = GUILayoutUtility.GetRect(width, height, options);
                GUI.Label(dragAndDropRect, label, GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
                if (dragAndDropRect.Contains(Event.current.mousePosition))
                {
                    GUI.Box(dragAndDropRect, "", GUIStyling.WordWrapAndCenter(EditorStyles.selectionRect));
                    if (DragAndDrop.objectReferences.Length == 0 || DragAndDrop.objectReferences.Length > 1 && !allowMultiple)
                    {
                        return new UnityEngine.Object[0];
                    }

                    foreach (UnityEngine.Object objectReference in DragAndDrop.objectReferences)
                    {
                        if (objectReference.GetType() != type)
                        {
                            return new UnityEngine.Object[0];
                        }
                    }

                    if (Event.current.type == EventType.DragUpdated)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                    }
                    else if (Event.current.type == EventType.DragPerform)
                    {
                        Event.current.Use();
                        return DragAndDrop.objectReferences;
                    }
                }
                else
                {
                    using (new GUIHelper.ColorScope(Color.black))
                    {
                        GUI.Box(dragAndDropRect, "", GUIStyling.WordWrapAndCenter(EditorStyles.selectionRect));
                    }
                }
                EditorGUILayout.Space();
            }
            return new UnityEngine.Object[0];
        }

        public static SF SerializedReferenceField<SF, T>(
            string label, 
            SF serializedReference, 
            Func<string, T, T> toDrawValue, 
            Func<T[]> toGetRange = null, 
            Func<T[]> toDrawRange = null, 
            Func<T[]> toGetInherited = null,
            Func<T[]> toDrawInherited = null
            ) where SF : SerializedReference<T>
        {
            if (serializedReference == null) return null;

            SerializedReferenceType referenceType = serializedReference.ReferenceType;
            T customValue = serializedReference.CustomValue;
            ScriptableConstant<T> scriptableConstant = serializedReference.ScriptableConstant;
            T[] rangeParameters = toGetRange?.Invoke();
            List<T> selection = serializedReference.Selection;
            ScriptableVariable<T> scriptableVariable = serializedReference.ScriptableVariable;
            T[] inheritedParameters = toGetInherited?.Invoke();

            using (new EditorGUILayout.VerticalScope())
            {
                if (!string.IsNullOrWhiteSpace(label))
                {
                    EditorGUILayout.LabelField(label);
                }
                using (new GUIHelper.IndentScope())
                {
                    referenceType = (SerializedReferenceType)EditorGUILayout.EnumPopup("Reference Type", serializedReference.ReferenceType);
                    switch (referenceType)
                    {
                        case SerializedReferenceType.Custom:
                            customValue = toDrawValue.Invoke("Custom Value", customValue);
                            break;
                        case SerializedReferenceType.ScriptableConstant:
                            scriptableConstant = (ScriptableConstant<T>)EditorGUILayout.ObjectField("Scriptable Constant", scriptableConstant, typeof(ScriptableConstant<T>), false);
                            break;
                        case SerializedReferenceType.RandomFromRange:
                            rangeParameters = toDrawRange?.Invoke();
                            break;
                        case SerializedReferenceType.RandomFromSelection:
                            selection = new List<T>(selection);
                            GUILayoutHelper.DrawIndexedList(
                                selection.Count,
                                (int index) => selection.Insert(index, default(T)),
                                selection.RemoveAt,
                                (int index) =>
                                {
                                    selection[index] = toDrawValue("", selection[index]);
                                }
                                );
                            break;
                        case SerializedReferenceType.ScriptableVariable:
                            scriptableVariable = (ScriptableVariable<T>)EditorGUILayout.ObjectField("Scriptable Variable", scriptableVariable, typeof(ScriptableVariable<T>), false);
                            break;
                        case SerializedReferenceType.Inherited:
                            inheritedParameters = toDrawInherited?.Invoke();
                            break;
                    }
                }
            }

            int rangeParametersLength = 0;
            if (rangeParameters != null) rangeParametersLength = rangeParameters.Length;
            int inheritedParametersLength = 0;
            if (inheritedParameters != null) inheritedParametersLength = inheritedParameters.Length;

            object[] parameters = new object[5 + rangeParametersLength];
            parameters[0] = referenceType;
            parameters[1] = customValue;
            parameters[2] = scriptableConstant;
            for (int i = 0; i < rangeParametersLength; i++) parameters[3 + i] = rangeParameters[i];
            parameters[3 + rangeParametersLength] = selection;
            parameters[4 + rangeParametersLength] = scriptableVariable;
            for (int i = 0; i < inheritedParametersLength; i++) parameters[5 + rangeParametersLength + i] = inheritedParameters[i];

            return (SF)Activator.CreateInstance(typeof(SF), parameters);
        }
    }
}
#endif
