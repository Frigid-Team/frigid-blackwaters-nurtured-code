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

        public List<T> GetRange(int index, int count)
        {
            return this.items.GetRange(index, count);
        }

        public void Add(T item)
        {
            this.items.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            this.items.AddRange(collection);
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

        public T[] ToArray()
        {
            return this.items.ToArray();
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

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            this.items.InsertRange(index, collection);
        }

        public bool Remove(T item)
        {
            return this.items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            this.items.RemoveRange(index, count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

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

        public List<Nested1DList<T>> GetRange(int index, int count)
        {
            return this.items.GetRange(index, count);
        }

        public void Add(Nested1DList<T> item)
        {
            this.items.Add(item);
        }

        public void AddRange(IEnumerable<Nested1DList<T>> collection)
        {
            this.items.AddRange(collection);
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

        public Nested1DList<T>[] ToArray()
        {
            return this.items.ToArray();
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

        public void InsertRange(int index, IEnumerable<Nested1DList<T>> collection)
        {
            this.items.InsertRange(index, collection);
        }

        public bool Remove(Nested1DList<T> item)
        {
            return this.items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            this.items.RemoveRange(index, count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    [Serializable]
    public class Nested3DList<T> : IList<Nested2DList<T>>
    {
        [SerializeField]
        private List<Nested2DList<T>> items;

        public Nested3DList()
        {
            this.items = new List<Nested2DList<T>>();
        }

        public Nested3DList(IEnumerable<Nested2DList<T>> items)
        {
            this.items = new List<Nested2DList<T>>(items);
        }

        public List<Nested2DList<T>> Items
        {
            get
            {
                return this.items;
            }
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

        public List<Nested2DList<T>> GetRange(int index, int count)
        {
            return this.items.GetRange(index, count);
        }

        public void Add(Nested2DList<T> item)
        {
            this.items.Add(item);
        }

        public void AddRange(IEnumerable<Nested2DList<T>> collection)
        {
            this.items.AddRange(collection);
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

        public Nested2DList<T>[] ToArray()
        {
            return this.items.ToArray();
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

        public void InsertRange(int index, IEnumerable<Nested2DList<T>> collection)
        {
            this.items.InsertRange(index, collection);
        }

        public bool Remove(Nested2DList<T> item)
        {
            return this.items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            this.items.RemoveRange(index, count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
