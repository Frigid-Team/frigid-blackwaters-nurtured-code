using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public abstract class ScriptableVariable<T> : FrigidScriptableObject
    {
        [SerializeField]
        private T initialValue;

        private T value;

        public T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        protected override void Init()
        {
            base.Init();
            this.value = this.initialValue;
        }
    }
}
