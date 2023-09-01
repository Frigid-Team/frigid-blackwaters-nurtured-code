using System;
using UnityEditor;
using UnityEngine;

namespace FrigidBlackwaters
{
    public static class FrigidEdit
    {
        private static bool inEdit;

        public static void RecordChanges(UnityEngine.Object objectToEdit)
        {
#if UNITY_EDITOR
            if (inEdit)
            {
                if (objectToEdit is ScriptableObject)
                {
                    Undo.RecordObject(objectToEdit, "Change " + objectToEdit.name);
                    EditorUtility.SetDirty(objectToEdit);
                }
                else
                {
                    Undo.RegisterCompleteObjectUndo(objectToEdit, "Change " + objectToEdit.name);
                }
            }
#endif
        }

        public static void RecordFullObjectHierarchy(GameObject objectToEdit)
        {
#if UNITY_EDITOR
            if (inEdit)
            {
                Undo.RegisterFullObjectHierarchyUndo(objectToEdit, "Change Full Hierarchy " + objectToEdit.name);
            }
#endif
        }

        public static GameObject CreateGameObject(string name, Transform parent)
        {
            GameObject newGameObject = new GameObject(name);
#if UNITY_EDITOR
            if (inEdit)
            {
                Undo.RegisterCreatedObjectUndo(newGameObject, "Create GameObject");
                Undo.RegisterCompleteObjectUndo(newGameObject, "Create GameObject");
                Undo.SetTransformParent(newGameObject.transform, parent, "Create GameObject");
            }
            else
            {
                newGameObject.transform.SetParent(parent);
            }
#else
            newGameObject.transform.SetParent(parent);
#endif
            newGameObject.transform.localPosition = Vector2.zero;
            return newGameObject;
        }

        public static void DestroyGameObject(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (inEdit)
            {
                Undo.DestroyObjectImmediate(gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(gameObject);
            }
#else
            UnityEngine.Object.DestroyImmediate(gameObject, true);
#endif
        }

        public static Component AddComponent(GameObject gameObject, Type type)
        {
#if UNITY_EDITOR
            if (inEdit)
            {
                return Undo.AddComponent(gameObject, type);
            }
            else
            {
                return gameObject.AddComponent(type);
            }
#else
            return gameObject.AddComponent(type);
#endif
        }

        public static T AddComponent<T>(GameObject gameObject) where T : Component
        {
            return (T)AddComponent(gameObject, typeof(T));
        }

        public static void RemoveComponent(Component component)
        {
#if UNITY_EDITOR
            if (inEdit)
            {
                Undo.DestroyObjectImmediate(component);
            }
            else
            {
                UnityEngine.Object.Destroy(component);
            }
#else
            UnityEngine.Object.Destroy(component);
#endif
        }

#if UNITY_EDITOR
        public class EditingScope : GUI.Scope
        {
            private bool previouslyEditing;

            public EditingScope()
            {
                this.previouslyEditing = inEdit;
                inEdit = true;
            }

            protected override void CloseScope()
            {
                inEdit = this.previouslyEditing;
            }
        }
#endif
    }
}
