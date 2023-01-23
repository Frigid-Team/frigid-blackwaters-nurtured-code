using System;
using System.Reflection;

namespace FrigidBlackwaters.Utility
{
    public static class TypeExtensions
    {
        public static MethodInfo GetDerivedMethod(this Type type, string methodName, BindingFlags bindingAttr)
        {
            while (type.BaseType != null)
            {
                MethodInfo methodInfo = type.GetMethod(methodName, bindingAttr);
                if (methodInfo != null) return methodInfo;
                type = type.BaseType;
            }
            return null;
        }

        public static FieldInfo GetDerivedField(this Type type, string fieldName, BindingFlags bindingAttr)
        {
            while (type.BaseType != null)
            {
                FieldInfo fieldInfo = type.GetField(fieldName, bindingAttr);
                if (fieldInfo != null) return fieldInfo;
                type = type.BaseType;
            }
            return null;
        }

        public static PropertyInfo GetDerivedProperty(this Type type, string propertyName, BindingFlags bindingAttr)
        {
            while (type.BaseType != null)
            {
                PropertyInfo propertyInfo = type.GetProperty(propertyName, bindingAttr);
                if (propertyInfo != null) return propertyInfo;
                type = type.BaseType;
            }
            return null;
        }

        public static EventInfo GetDerivedEvent(this Type type, string eventName, BindingFlags bindingAttr)
        {
            while (type.BaseType != null)
            {
                EventInfo eventInfo = type.GetEvent(eventName, bindingAttr);
                if (eventInfo != null) return eventInfo;
                type = type.BaseType;
            }
            return null;
        }

        public static bool IsSubClassOfGeneric(this Type type, Type genericTypeDefinition)
        {
            return type.GetGenericSubClass(genericTypeDefinition) != null;
        }

        public static Type GetGenericSubClass(this Type type, Type genericTypeDefinition)
        {
            Type currType = type;
            while (currType != null)
            {
                if (currType.IsGenericType && currType.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return currType;
                }
                currType = currType.BaseType;
            }
            return null;
        }
    }
}
