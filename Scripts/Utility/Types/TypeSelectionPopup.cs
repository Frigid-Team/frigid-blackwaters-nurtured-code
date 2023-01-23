#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    public class TypeSelectionPopup : FrigidPopupWindow
    {
        private List<Type> types;
        private Action<Type> onTypeSelected;

        private Vector2 scrollPos;

        public TypeSelectionPopup(List<Type> types, Action<Type> onTypeSelected) : base()
        {
            this.types = types;
            this.onTypeSelected = onTypeSelected;
        }

        protected override void Draw()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        GUILayout.Label("Select Type", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
                    }
                    using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(this.scrollPos))
                    {
                        this.scrollPos = scrollViewScope.scrollPosition;
                        Vector2 windowSize = GetWindowSize();
                        foreach (Type type in this.types)
                        {
                            if (GUILayout.Button(type.Name, GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                            {
                                this.onTypeSelected?.Invoke(type);
                                this.editorWindow.Close();
                                return;
                            }
                        }
                    }
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
            }
        }
    }
}
#endif
