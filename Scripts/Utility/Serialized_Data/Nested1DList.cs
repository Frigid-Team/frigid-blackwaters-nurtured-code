using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    [Serializable]
    public class Nested1DList<T> : IList<T>
    {
        [SerializeField]
        private List<T> items;

        public Nested1DList()
        {
            this.items = new List<T>();
        }

        public Nested1DList(IEnumerable<T> items)
        {
            this.items = new List<T>(items);
        }

        public List<T> Items
        {
            get
            {
                return this.items;
            }
        }
  
        public T this[int index] 
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

        public void Add(T item)
        {
            this.items.Add(item);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(T item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return this.items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.items.Insert(index, item);
        }

        public bool Remove(T item)
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
