#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    public class SearchPopup : FrigidPopup
    {
        private string[] entries;
        private Action<int> onSelected;

        private Vector2 scrollPos;
        private string searchFilter;

        public SearchPopup(string[] entries, Action<int> onSelected) : base()
        {
            this.entries = entries;
            this.onSelected = onSelected;
            this.searchFilter = "";
        }

        protected override void Draw()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                this.searchFilter = EditorGUILayout.TextField(this.searchFilter, EditorStyles.toolbarSearchField);
                using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(this.scrollPos))
                {
                    this.scrollPos = scrollViewScope.scrollPosition;
                    for (int i = 0; i < this.entries.Length; i++)
                    {
                        string entry = this.entries[i];
                        if (entry.IndexOf(this.searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (GUILayout.Button(entry, EditorStyles.toolbarButton, GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                            {
                                this.onSelected?.Invoke(i);
                                this.editorWindow.Close();
                                return;
                            }
                            EditorGUILayout.Space();
                        }
                    }
                }
                EditorGUILayout.Space();
            }
        }
    }
}
#endif
