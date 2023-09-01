using System;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public struct Span<T> : IEquatable<Span<T>> where T : IComparable, IComparable<T>
    {
        [SerializeField]
        private T first;
        [SerializeField]
        private T second;

        public Span(T first, T second)
        {
            this.first = first;
            this.second = second;
        }

        public T First
        {
            get
            {
                return this.first;
            }
        }

        public T Second
        {
            get
            {
                return this.second;
            }
        }

        public T Min
        {
            get
            {
                return this.first.CompareTo(this.second) > 0 ? this.second : this.first;
            }
        }

        public T Max
        {
            get
            {
                return this.first.CompareTo(this.second) > 0 ? this.first : this.second;
            }
        }

        public bool Within(T value)
        {
            return value.CompareTo(this.Min) >= 0 && value.CompareTo(this.Max) <= 0;
        }

        public override bool Equals(object obj)
        {
            return this.Equals((Span<T>)obj);
        }

        public bool Equals(Span<T> other)
        {
            return other.first.CompareTo(this.first) == 0 && other.second.CompareTo(this.second) == 0;
        }

        public override int GetHashCode()
        {
            return (this.first, this.second).GetHashCode();
        }

        public static bool operator ==(Span<T> lhs, Span<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Span<T> lhs, Span<T> rhs)
        {
            return !(lhs == rhs);
        }
    }
}
