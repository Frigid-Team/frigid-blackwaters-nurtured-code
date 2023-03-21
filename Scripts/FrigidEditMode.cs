using System;
using UnityEditor;
using UnityEngine;

namespace FrigidBlackwaters
{
    public static class FrigidEditMode
    {
        private static bool isEditing;
        
        public static bool InEdit
        {
            get
            {
                return isEditing;
            }
        }

        static FrigidEditMode()
        {
            isEditing = false;
        }

        public static void RecordPotentialChanges(UnityEngine.Object objectToEdit)
        {
#if UNITY_EDITOR
            if (isEditing)
            {
                Undo.RecordObject(objectToEdit, "Change " + objectToEdit.name);
            }
#endif
        }

        public static void RecordPotentialChangesToFullHierarchy(UnityEngine.Object objectToEdit)
        {
#if UNITY_EDITOR
            if (isEditing)
            {
                Undo.RegisterFullObjectHierarchyUndo(objectToEdit, "Change " + objectToEdit.name);
            }
#endif
        }

        public static GameObject CreateGameObject(Transform parentTransform)
        {
            GameObject newGameObject = new GameObject();
#if UNITY_EDITOR
            if (isEditing)
            {
                Undo.RegisterCreatedObjectUndo(newGameObject, "Create GameObject");
                Undo.SetTransformParent(newGameObject.transform, parentTransform, "Create GameObject");
            }
            else
            {
                newGameObject.transform.SetParent(parentTransform);
            }
#else
            newGameObject.transform.SetParent(parentTransform);
#endif
            newGameObject.transform.localPosition = Vector2.zero;
            return newGameObject;
        }

        public static void DestroyGameObject(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (isEditing)
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
            if (isEditing)
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
            if (isEditing)
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
                this.previouslyEditing = isEditing;
                isEditing = true;
            }

            protected override void CloseScope()
            {
                isEditing = this.previouslyEditing;
            }
        }
#endif
    }
}
