using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    public class SerializedReference<T> : IEquatable<SerializedReference<T>>
    {
        [SerializeField]
        private SerializedReferenceType referenceType;
        [SerializeField]
        [ShowIfInt("referenceType", 0, true)]
        private T customValue;
        [SerializeField]
        [ShowIfInt("referenceType", 1, true)]
        private ScriptableConstant<T> scriptableConstant;
        [SerializeField]
        [ShowIfInt("referenceType", 3, true)]
        private List<T> selection;
        [SerializeField]
        [ShowIfInt("referenceType", 4, true)]
        private ScriptableVariable<T> scriptableVariable;

        public SerializedReference()
        {
            this.referenceType = SerializedReferenceType.Custom;
            this.customValue = this.GetDefaultCustomValue();
            this.scriptableConstant = null;
            this.selection = new List<T>();
            this.scriptableVariable = null;
        }

        public SerializedReference(SerializedReference<T> other)
        {
            this.referenceType = other.referenceType;
            this.customValue = this.GetCopyValue(other.customValue);
            this.scriptableConstant = other.scriptableConstant;
            this.selection = new List<T>();
            foreach (T selectionValue in other.selection)
            {
                this.selection.Add(this.GetCopyValue(selectionValue));
            }
            this.scriptableVariable = other.scriptableVariable;
        }

        public SerializedReference(
            SerializedReferenceType referenceType,
            T customValue,
            ScriptableConstant<T> scriptableConstant, 
            List<T> selection, 
            ScriptableVariable<T> scriptableVariable
            )
        {
            this.referenceType = referenceType;
            this.customValue = customValue;
            this.scriptableConstant = scriptableConstant;
            this.selection = selection;
            this.scriptableVariable = scriptableVariable;
        }

        public T ImmutableValue
        {
            get
            {
                switch (this.referenceType)
                {
                    case SerializedReferenceType.Custom:
                        return this.customValue;
                    case SerializedReferenceType.ScriptableConstant:
                        return this.scriptableConstant.Value;
                    case SerializedReferenceType.RandomFromRange:
                        return this.GetRandomFromRangeImmutableValue();
                    case SerializedReferenceType.RandomFromSelection:
                        return this.selection.Count == 0 ? default(T) : this.selection[new System.Random(this.GetHashCode()).Next(this.selection.Count)];
                    case SerializedReferenceType.ScriptableVariable:
                        return this.scriptableVariable.InitialValue;
                    case SerializedReferenceType.Inherited:
                        return this.GetInheritedImmutableValue();
                }
                return this.customValue;
            }
        }

        public T MutableValue
        {
            get
            {
                switch (this.referenceType)
                {
                    case SerializedReferenceType.Custom:
                        return this.customValue;
                    case SerializedReferenceType.ScriptableConstant:
                        return this.scriptableConstant.Value;
                    case SerializedReferenceType.RandomFromRange:
                        return this.GetRandomFromRangeMutableValue();
                    case SerializedReferenceType.RandomFromSelection:
                        return this.selection[UnityEngine.Random.Range(0, this.selection.Count)];
                    case SerializedReferenceType.ScriptableVariable:
                        return this.scriptableVariable.Value;
                    case SerializedReferenceType.Inherited:
                        return this.GetInheritedMutableValue();
                }
                return this.customValue;
            }
        }

        public SerializedReferenceType ReferenceType
        {
            get
            {
                return this.referenceType;
            }
        }

        public T CustomValue
        {
            get
            {
                return this.customValue;
            }
        }

        public ScriptableConstant<T> ScriptableConstant
        {
            get
            {
                return this.scriptableConstant;
            }
        }
 
        public List<T> Selection
        {
            get
            {
                return this.selection;
            }
        }

        public ScriptableVariable<T> ScriptableVariable
        {
            get
            {
                return this.scriptableVariable;
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SerializedReference<T>);
        }

        public bool Equals(SerializedReference<T> other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;

            if (this.referenceType != other.referenceType) return false;

            switch (this.referenceType)
            {
                case SerializedReferenceType.Custom:
                    return EqualityComparer<T>.Default.Equals(this.customValue, other.customValue);
                case SerializedReferenceType.ScriptableConstant:
                    return this.scriptableConstant == other.scriptableConstant;
                case SerializedReferenceType.RandomFromRange:
                    return this.RangeEquals(other);
                case SerializedReferenceType.RandomFromSelection:
                    return this.selection.SequenceEqual(other.selection);
                case SerializedReferenceType.ScriptableVariable:
                    return this.scriptableVariable = other.scriptableVariable;
                case SerializedReferenceType.Inherited:
                    return this.InheritEquals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            switch (this.referenceType)
            {
                case SerializedReferenceType.Custom:
                    return this.customValue.GetHashCode();
                case SerializedReferenceType.ScriptableConstant:
                    return this.scriptableConstant.GetHashCode();
                case SerializedReferenceType.RandomFromRange:
                    return this.GetHashCodeFromRange();
                case SerializedReferenceType.RandomFromSelection:
                    return this.selection.GetHashCode();
                case SerializedReferenceType.ScriptableVariable:
                    return this.scriptableVariable.GetHashCode();
                case SerializedReferenceType.Inherited:
                    return this.GetHashCodeFromInherited();
            }
            return base.GetHashCode();
        }

        public static bool operator ==(SerializedReference<T> lhs, SerializedReference<T> rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SerializedReference<T> lhs, SerializedReference<T> rhs)
        {
            return !(lhs == rhs);
        }

        protected virtual T GetRandomFromRangeImmutableValue()
        {
            return this.customValue;
        }

        protected virtual T GetInheritedImmutableValue()
        {
            return this.customValue;
        }

        protected virtual T GetRandomFromRangeMutableValue()
        {
            return this.customValue;
        }

        protected virtual T GetInheritedMutableValue()
        {
            return this.customValue;
        }

        protected virtual T GetDefaultCustomValue() { return default(T); }

        protected virtual T GetCopyValue(T value) { return value; }

        protected virtual bool RangeEquals(SerializedReference<T> other) { return true; }

        protected virtual bool InheritEquals(SerializedReference<T> other) { return true; }

        protected virtual int GetHashCodeFromRange() { return 0; }

        protected virtual int GetHashCodeFromInherited() { return 0; }
    }
}
