using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Utility
{
    [Serializable]
    public class Nested2DList<T> : IList<Nested1DList<T>>
    {
        [SerializeField]
        private List<Nested1DList<T>> items;

        public Nested2DList()
        {
            this.items = new List<Nested1DList<T>>();
        }

        public Nested2DList(IEnumerable<Nested1DList<T>> items)
        {
            this.items = new List<Nested1DList<T>>(items);
        }

        public List<Nested1DList<T>> Items
        {
            get
            {
                return this.items;
            }
        }

        public Nested1DList<T> this[int index]
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

        public void Add(Nested1DList<T> item)
        {
            this.items.Add(item);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(Nested1DList<T> item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(Nested1DList<T>[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Nested1DList<T>> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        public int IndexOf(Nested1DList<T> item)
        {
            return this.items.IndexOf(item);
        }

        public void Insert(int index, Nested1DList<T> item)
        {
            this.items.Insert(index, item);
        }

        public bool Remove(Nested1DList<T> item)
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
