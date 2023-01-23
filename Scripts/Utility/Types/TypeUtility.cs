#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    public static class TypeUtility
    {
        public static List<Type> GetCompleteTypesDerivedFrom(Type parentType)
        {
            TypeCache.TypeCollection derivedTypes = TypeCache.GetTypesDerivedFrom(parentType);
            List<Type> completeDerivedTypes = new List<Type>();
            foreach (Type derivedType in derivedTypes)
            {
                if (derivedType.IsClass && !derivedType.IsAbstract)
                {
                    completeDerivedTypes.Add(derivedType);
                }
            }
            return completeDerivedTypes;
        }
    }
}
#endif
