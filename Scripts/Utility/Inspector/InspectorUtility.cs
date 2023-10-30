#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    public static class InspectorUtility
    {
        public const BindingFlags SearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        private static Dictionary<Type, InspectorFieldDrawer> inspectorFieldDrawersToAttributeTypes;

        static InspectorUtility()
        {
            inspectorFieldDrawersToAttributeTypes = new Dictionary<Type, InspectorFieldDrawer>();
            TypeCache.TypeCollection inspectorDrawerTypes = TypeCache.GetTypesWithAttribute(typeof(CustomInspectorFieldDrawerAttribute));
            foreach (Type inspectorDrawerType in inspectorDrawerTypes)
            {
                if (inspectorDrawerType.IsSubclassOf(typeof(InspectorFieldDrawer)))
                {
                    CustomInspectorFieldDrawerAttribute customInspectorDrawerAttribute = (CustomInspectorFieldDrawerAttribute)inspectorDrawerType.GetCustomAttribute(typeof(CustomInspectorFieldDrawerAttribute));
                    if (!inspectorFieldDrawersToAttributeTypes.ContainsKey(customInspectorDrawerAttribute.Type))
                    {
                        if (customInspectorDrawerAttribute.Type.IsSubclassOf(typeof(InspectorFieldAttribute)))
                        {
                            inspectorFieldDrawersToAttributeTypes.Add(customInspectorDrawerAttribute.Type, (InspectorFieldDrawer)Activator.CreateInstance(inspectorDrawerType));
                        }
                    }
                }
            } 
        }

        public static List<InspectorFieldDrawer> CreateFieldDrawersForSerializedProperty(SerializedProperty property)
        {
            List<InspectorFieldDrawer> fieldDrawers = new List<InspectorFieldDrawer>();
            if (property != null)
            {
                FieldInfo fieldInfo = GetFieldFromSerializedProperty(property);
                if (fieldInfo != null)
                {
                    foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
                    {
                        Type attributeType = attribute.GetType();
                        if (attributeType.IsSubclassOf(typeof(InspectorFieldAttribute)))
                        {
                            if (inspectorFieldDrawersToAttributeTypes.ContainsKey(attributeType))
                            {
                                fieldDrawers.Add(inspectorFieldDrawersToAttributeTypes[attributeType].Copy((InspectorFieldAttribute)attribute));
                            }
                            else
                            {
                                Debug.LogError("There is no InspectorFieldDrawer for " + attributeType.Name + ".");
                            }
                        }
                    }
                }
            }
            return fieldDrawers;
        }

        public static bool IsBaseSerializedProperty(SerializedProperty property)
        {
            return property != null && property.propertyType - SerializedPropertyType.Integer >= 0 && !property.isArray;
        }

        public static object GetObjectFromSerializedProperty(SerializedProperty property)
        {
            object GetValue(object obj, string name)
            {
                if (obj == null)
                {
                    return null;
                }
                Type currType = obj.GetType();

                while (currType != null && currType != typeof(FrigidMonoBehaviour) && currType != typeof(FrigidScriptableObject))
                {
                    FieldInfo fieldInfo = currType.GetField(name, SearchFlags);
                    if (fieldInfo != null) return fieldInfo.GetValue(obj);
                    currType = currType.BaseType;
                }
                return null;
            }

            object GetValueInArray(object obj, string name, int index)
            {
                IEnumerable enumerable = (IEnumerable)GetValue(obj, name);
                if (enumerable == null) return null;
                IEnumerator enumerator = enumerable.GetEnumerator();

                for (int i = 0; i <= index; i++)
                {
                    if (!enumerator.MoveNext()) return null;
                }
                return enumerator.Current;
            }

            string adjustedPropertyPath = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            string[] elements = adjustedPropertyPath.Split('.');
            foreach (string element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValueInArray(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public static FieldInfo GetFieldFromSerializedProperty(SerializedProperty property)
        {
            FieldInfo GetFieldViaPath(Type type, string path)
            {
                Type currType = type;
                FieldInfo fieldInfo = currType.GetField(path, SearchFlags);
                string[] elements = path.Split('.');

                int i = 0;
                while (i < elements.Length)
                {
                    fieldInfo = currType.GetField(elements[i], SearchFlags);
                    if (fieldInfo != null)
                    {
                        i++;
                        if (fieldInfo.FieldType.IsArray)
                        {
                            currType = fieldInfo.FieldType.GetElementType();
                            i += 2;
                            continue;
                        }
                        if (fieldInfo.FieldType.IsGenericType)
                        {
                            if (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                currType = fieldInfo.FieldType.GetGenericArguments()[0];
                                i += 2;
                            }
                            else
                            {
                                currType = fieldInfo.FieldType;
                            }
                            continue;
                        }
                        currType = fieldInfo.FieldType;
                    }
                    else
                    {
                        if (currType.BaseType != null)
                        {
                            currType = currType.BaseType;
                            continue;
                        }
                        break;
                    }
                }

                if (fieldInfo == null)
                {
                    if (type.BaseType != null && type != typeof(FrigidScriptableObject) && type != typeof(FrigidMonoBehaviour))
                    {
                        return GetFieldViaPath(type.BaseType, path);
                    }
                    else
                    {
                        return null;
                    }
                }

                return fieldInfo;
            }

            return GetFieldViaPath(property.serializedObject.targetObject.GetType(), property.propertyPath);
        }
    }
}
#endif
