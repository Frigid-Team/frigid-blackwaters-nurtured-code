#if UNITY_EDITOR
using System;

namespace FrigidBlackwaters.Utility
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomInspectorFieldDrawerAttribute : Attribute
    {
        private Type type;

        public CustomInspectorFieldDrawerAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get
            {
                return this.type;
            }
        }
    }
}
#endif
