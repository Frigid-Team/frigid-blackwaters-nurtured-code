using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public class ScriptableConstant<T> : FrigidScriptableObject
    {
        [SerializeField]
        private T constantValue;

        public T Value
        {
            get
            {
                return this.constantValue;
            }
        }
    }
}
