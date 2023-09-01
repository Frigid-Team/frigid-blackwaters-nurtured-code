using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    [Serializable]
    public struct SerializedValueTuple<T1>
    {
        [SerializeField]
        private T1 item1;

        public SerializedValueTuple(T1 item1)
        {
            this.item1 = item1;
        }

        public T1 Item1
        {
            get
            {
                return this.item1;
            }
            set
            {
                this.item1 = value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SerializedValueTuple<T1> tuple && EqualityComparer<T1>.Default.Equals(this.item1, tuple.item1);
        }

        public override int GetHashCode()
        {
            return this.item1.GetHashCode();
        }

        public static bool operator ==(SerializedValueTuple<T1> lhs, SerializedValueTuple<T1> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SerializedValueTuple<T1> lhs, SerializedValueTuple<T1> rhs)
        {
            return !(lhs == rhs);
        }
    }

    [Serializable]
    public struct SerializedValueTuple<T1, T2>
    {
        [SerializeField]
        private T1 item1;
        [SerializeField]
        private T2 item2;

        public SerializedValueTuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public T1 Item1
        {
            get
            {
                return this.item1;
            }
            set
            {
                this.item1 = value;
            }
        }

        public T2 Item2
        {
            get
            {
                return this.item2;
            }
            set
            {
                this.item2 = value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SerializedValueTuple<T1, T2> tuple && EqualityComparer<T1>.Default.Equals(this.item1, tuple.item1) && EqualityComparer<T2>.Default.Equals(this.item2, tuple.item2);
        }

        public override int GetHashCode()
        {
            return this.item1.GetHashCode();
        }

        public static bool operator ==(SerializedValueTuple<T1, T2> lhs, SerializedValueTuple<T1, T2> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SerializedValueTuple<T1, T2> lhs, SerializedValueTuple<T1, T2> rhs)
        {
            return !(lhs == rhs);
        }
    }

    [Serializable]
    public struct SerializedValueTuple<T1, T2, T3>
    {
        [SerializeField]
        private T1 item1;
        [SerializeField]
        private T2 item2;
        [SerializeField]
        private T3 item3;

        public SerializedValueTuple(T1 item1, T2 item2, T3 item3)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
        }

        public T1 Item1
        {
            get
            {
                return this.item1;
            }
            set
            {
                this.item1 = value;
            }
        }

        public T2 Item2
        {
            get
            {
                return this.item2;
            }
            set
            {
                this.item2 = value;
            }
        }

        public T3 Item3
        {
            get
            {
                return this.item3;
            }
            set
            {
                this.item3 = value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SerializedValueTuple<T1, T2, T3> tuple && EqualityComparer<T1>.Default.Equals(this.item1, tuple.item1) && EqualityComparer<T2>.Default.Equals(this.item2, tuple.item2) && EqualityComparer<T3>.Default.Equals(this.item3, tuple.item3);
        }

        public override int GetHashCode()
        {
            return this.item1.GetHashCode();
        }

        public static bool operator ==(SerializedValueTuple<T1, T2, T3> lhs, SerializedValueTuple<T1, T2, T3> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SerializedValueTuple<T1, T2, T3> lhs, SerializedValueTuple<T1, T2, T3> rhs)
        {
            return !(lhs == rhs);
        }
    }

    [Serializable]
    public struct SerializedValueTuple<T1, T2, T3, T4>
    {
        [SerializeField]
        private T1 item1;
        [SerializeField]
        private T2 item2;
        [SerializeField]
        private T3 item3;
        [SerializeField]
        private T4 item4;

        public SerializedValueTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
        }

        public T1 Item1
        {
            get
            {
                return this.item1;
            }
            set
            {
                this.item1 = value;
            }
        }

        public T2 Item2
        {
            get
            {
                return this.item2;
            }
            set
            {
                this.item2 = value;
            }
        }

        public T3 Item3
        {
            get
            {
                return this.item3;
            }
            set
            {
                this.item3 = value;
            }
        }

        public T4 Item4
        {
            get
            {
                return this.item4;
            }
            set
            {
                this.item4 = value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SerializedValueTuple<T1, T2, T3, T4> tuple && EqualityComparer<T1>.Default.Equals(this.item1, tuple.item1) && EqualityComparer<T2>.Default.Equals(this.item2, tuple.item2) && EqualityComparer<T3>.Default.Equals(this.item3, tuple.item3) && EqualityComparer<T4>.Default.Equals(this.item4, tuple.item4);
        }

        public override int GetHashCode()
        {
            return this.item1.GetHashCode();
        }

        public static bool operator ==(SerializedValueTuple<T1, T2, T3, T4> lhs, SerializedValueTuple<T1, T2, T3, T4> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SerializedValueTuple<T1, T2, T3, T4> lhs, SerializedValueTuple<T1, T2, T3, T4> rhs)
        {
            return !(lhs == rhs);
        }
    }
}
