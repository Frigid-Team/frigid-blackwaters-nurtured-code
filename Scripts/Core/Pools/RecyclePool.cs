using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public class RecyclePool<T> where T : Component
    {
        private Queue<T> pooledObjects;
        private Func<T> toCreateInstance;
        private Action<T> toDestroyInstance;

        public RecyclePool(Func<T> toCreateInstance, Action<T> toDestroyInstance)
        {
            this.pooledObjects = new Queue<T>();
            this.toCreateInstance = toCreateInstance;
            this.toDestroyInstance = toDestroyInstance;
        }

        public RecyclePool(int numPreparedInAdvance, Func<T> toCreateInstance, Action<T> toDestroyInstance)
        {
            this.pooledObjects = new Queue<T>();
            this.toCreateInstance = toCreateInstance;
            this.toDestroyInstance = toDestroyInstance;
            for (int i = 0; i < numPreparedInAdvance; i++)
            {
                T instance = this.toCreateInstance?.Invoke();
                this.pooledObjects.Enqueue(instance);
                instance.gameObject.SetActive(false);
            }
        }

        ~RecyclePool()
        {
            foreach (T pooledObject in this.pooledObjects)
            {
                this.toDestroyInstance?.Invoke(pooledObject);
            }
        }

        public T Retrieve()
        {
            T nextInstance;
            if (this.pooledObjects.Count > 0)
            {
                nextInstance = this.pooledObjects.Dequeue();
            }
            else
            {
                nextInstance = this.toCreateInstance?.Invoke();
            }
            nextInstance.gameObject.SetActive(true);
            return nextInstance;
        }

        public List<T> Retrieve(int amount)
        {
            List<T> instances = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                instances.Add(this.Retrieve());
            }
            return instances;
        }

        public void Pool(T instanceToPool)
        {
            instanceToPool.gameObject.SetActive(false);
            if (!this.pooledObjects.Contains(instanceToPool))
            {
                this.pooledObjects.Enqueue(instanceToPool);
            }
        }

        public void Pool (List<T> instancesToPool)
        {
            foreach (T instance in instancesToPool)
            {
                this.Pool(instance);
            }
        }

        public void Cycle(List<T> unPooledInstances, int neededQuantity)
        {
            while (unPooledInstances.Count > neededQuantity)
            {
                this.Pool(unPooledInstances[unPooledInstances.Count - 1]);
                unPooledInstances.RemoveAt(unPooledInstances.Count - 1);
            }

            while (unPooledInstances.Count < neededQuantity)
            {
                unPooledInstances.Add(this.Retrieve());
            }
        }
    }
}
