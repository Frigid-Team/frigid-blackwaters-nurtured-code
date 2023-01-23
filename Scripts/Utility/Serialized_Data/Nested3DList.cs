using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Utility 
{ 
    [Serializable]
    public class Nested3DList<T> : IList<Nested2DList<T>>
    {
        [SerializeField]
        private List<Nested2DList<T>> items;

        public Nested3DList()
        {
            this.items = new List<Nested2DList<T>>();
        }

        public Nested2DList<T> this[int index]
        {
            get
            {
                return this.items[index];
            }
            set
            {
                this.items[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(Nested2DList<T> item)
        {
            this.items.Add(item);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(Nested2DList<T> item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(Nested2DList<T>[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Nested2DList<T>> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        public int IndexOf(Nested2DList<T> item)
        {
            return this.items.IndexOf(item);
        }

        public void Insert(int index, Nested2DList<T> item)
        {
            this.items.Insert(index, item);
        }

        public bool Remove(Nested2DList<T> item)
        {
            return this.items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
