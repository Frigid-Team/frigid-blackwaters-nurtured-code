#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FrigidBlackwaters.Utility
{
    public class InspectorPopup : FrigidPopup
    {
        private GameObject gameObject;
        private HashSet<Component> componentsToNotDraw;
        private HashSet<Component> componentsToNotDeleteOrMove;
        private Vector2 scrollPos;

        public InspectorPopup(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.componentsToNotDraw = new HashSet<Component>();
            this.componentsToNotDeleteOrMove = new HashSet<Component>();
        }

        public InspectorPopup DoNotDraw(Component component)
        {
            this.componentsToNotDraw.Add(component);
            return this;
        }

        public InspectorPopup DoNotMoveOrDelete(Component componentToNotDelete)
        {
            this.componentsToNotDeleteOrMove.Add(componentToNotDelete);
            return this;
        }

        protected override void Draw()
        {
            using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(this.scrollPos))
            {
                this.scrollPos = scrollViewScope.scrollPosition;
                foreach (Component component in this.gameObject.GetComponents<Component>())
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField(component.GetType().Name, EditorStyles.largeLabel);
                        if (!this.componentsToNotDraw.Contains(component))
                        {
                            using (new EditorGUI.DisabledScope(this.componentsToNotDeleteOrMove.Contains(component)))
                            {
                                if (GUILayout.Button("Move Up"))
                                {
                                    ComponentUtility.MoveComponentUp(component);
                                }
                                if (GUILayout.Button("Move Down"))
                                {
                                    ComponentUtility.MoveComponentDown(component);
                                }
                                if (GUILayout.Button("Remove"))
                                {
                                    Undo.DestroyObjectImmediate(component);
                                    continue;
                                }
                            }
                        }
                    }
                    if (!this.componentsToNotDraw.Contains(component))
                    {
                        Editor editor = Editor.CreateEditor(component);
                        editor.DrawDefaultInspector();
                    }
                }
            }
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                if (GUILayout.Button("Add"))
                {
                    List<Type> allComponentTypes = TypeUtility.GetCompleteTypesDerivedFrom(typeof(Component));
                    SearchPopup typeSelectionPopup = new SearchPopup(
                        allComponentTypes.Select((Type type) => type.Name).ToArray(),
                        (int typeIndex) => Undo.AddComponent(this.gameObject, allComponentTypes[typeIndex])
                        );
                    Show(GUILayoutUtility.GetLastRect(), typeSelectionPopup);
                }
            }
        }
    }
}
#endif
