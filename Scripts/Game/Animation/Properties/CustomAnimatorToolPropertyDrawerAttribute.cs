#if UNITY_EDITOR
using System;

namespace FrigidBlackwaters.Game
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAnimatorToolPropertyDrawerAttribute : Attribute
    {
        private Type propertyType;

        public CustomAnimatorToolPropertyDrawerAttribute(Type propertyType)
        {
            this.propertyType = propertyType;
        }

        public Type PropertyType
        {
            get
            {
                return this.propertyType;
            }
        }
    }
}
#endif
