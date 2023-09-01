using UnityEngine;
using System;

namespace FrigidBlackwaters.Utility
{
    public abstract class ScriptableVariable<T> : FrigidScriptableObject
    {
        [SerializeField]
        private T initialValue;

        private Action onValueChanged;

        private T value;

        public T InitialValue
        {
            get
            {
                return this.initialValue;
            }
        }

        public T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (!this.value.Equals(value))
                {
                    this.value = value;
                    this.onValueChanged?.Invoke();
                }
            }
        }

        public Action OnValueChanged
        {
            get
            {
                return this.onValueChanged;
            }
            set
            {
                this.onValueChanged = value;
            }
        }

        protected override void OnBegin()
        {
            base.OnBegin();
            this.value = this.initialValue;
        }
    }
}
