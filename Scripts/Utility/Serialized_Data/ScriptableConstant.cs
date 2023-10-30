using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public class ScriptableConstant<T> : FrigidScriptableObject
    {
        [SerializeField]
        // temp: set to private
        public T constantValue;

        public T Value
        {
            get
            {
                return this.constantValue;
            }
        }
    }
}
