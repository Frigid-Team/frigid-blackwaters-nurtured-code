using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class SaveFileData
    {
        [SerializeField]
        private DataCollection<string> stringData;
        [SerializeField]
        private DataCollection<Nested1DList<string>> stringArrayData;
        [SerializeField]
        private DataCollection<int> intData;
        [SerializeField]
        private DataCollection<Nested1DList<int>> intArrayData;
        [SerializeField]
        private DataCollection<float> floatData;
        [SerializeField]
        private DataCollection<Nested1DList<float>> floatArrayData;
        [SerializeField]
        private DataCollection<bool> boolData;
        [SerializeField]
        private DataCollection<Nested1DList<bool>> boolArrayData;
        [SerializeField]
        private DataCollection<string> assetIdData;
        [SerializeField]
        private DataCollection<Nested1DList<string>> assetIdArrayData;

        public SaveFileData()
        {
            this.stringData = new DataCollection<string>();
            this.stringArrayData = new DataCollection<Nested1DList<string>>();
            this.intData = new DataCollection<int>();
            this.intArrayData = new DataCollection<Nested1DList<int>>();
            this.floatData = new DataCollection<float>();
            this.floatArrayData = new DataCollection<Nested1DList<float>>();
            this.boolData = new DataCollection<bool>();
            this.boolArrayData = new DataCollection<Nested1DList<bool>>();
            this.assetIdData = new DataCollection<string>();
            this.assetIdArrayData = new DataCollection<Nested1DList<string>>();
        }

        public bool IsEmpty
        {
            get
            {
                return
                    this.stringData.IsEmpty && this.stringArrayData.IsEmpty &&
                    this.intData.IsEmpty && this.intArrayData.IsEmpty &&
                    this.floatData.IsEmpty && this.floatArrayData.IsEmpty &&
                    this.boolData.IsEmpty && this.boolArrayData.IsEmpty &&
                    this.assetIdData.IsEmpty && this.assetIdArrayData.IsEmpty;
            }
        }

        public string GetString(string key)
        {
            return this.stringData.Get(key);
        }

        public void SetString(string key, string value)
        {
            this.stringData.Set(key, value);
        }

        public string[] GetStringArray(string key)
        {
            Nested1DList<string> strings = this.stringArrayData.Get(key);
            if (strings == null)
            {
                return new string[0];
            }
            return strings.ToArray();
        }

        public void SetStringArray(string key, string[] value)
        {
            this.stringArrayData.Set(key, new Nested1DList<string>(value));
        }

        public int GetInt(string key)
        {
            return this.intData.Get(key);
        }

        public void SetInt(string key, int value)
        {
            this.intData.Set(key, value);
        }

        public int[] GetIntArray(string key)
        {
            Nested1DList<int> ints = this.intArrayData.Get(key);
            if (ints == null)
            {
                return new int[0];
            }
            return ints.ToArray();
        }

        public void SetIntArray(string key, int[] value)
        {
            this.intArrayData.Set(key, new Nested1DList<int>(value));
        }

        public float GetFloat(string key)
        {
            return this.floatData.Get(key);
        }

        public void SetFloat(string key, float value)
        {
            this.floatData.Set(key, value);
        }

        public float[] GetFloatArray(string key)
        {
            Nested1DList<float> floats = this.floatArrayData.Get(key);
            if (floats == null)
            {
                return new float[0];
            }
            return floats.ToArray();
        }

        public void SetFloatArray(string key, float[] value)
        {
            this.floatArrayData.Set(key, new Nested1DList<float>(value));
        }

        public bool GetBool(string key)
        {
            return this.boolData.Get(key);
        }

        public void SetBool(string key, bool value)
        {
            this.boolData.Set(key, value);
        }

        public bool[] GetBoolArray(string key)
        {
            Nested1DList<bool> bools = this.boolArrayData.Get(key);
            if (bools == null)
            {
                return new bool[0];
            }
            return bools.ToArray();
        }

        public void SetBoolArray(string key, bool[] value)
        {
            this.boolArrayData.Set(key, new Nested1DList<bool>(value));
        }

        public T GetAsset<T>(string key) where T : Object
        {
            return AssetLookupTable<T>.ForwardLookup(this.assetIdData.Get(key));
        }

        public void SetAsset<T>(string key, T asset) where T : Object
        {
            this.assetIdData.Set(key, AssetLookupTable<T>.BackwardLookup(asset));
        }

        public T[] GetAssetArray<T>(string key) where T : Object
        {
            Nested1DList<string> assetIds = this.assetIdArrayData.Get(key);
            if (assetIds == null)
            {
                return new T[0];
            }
            T[] assets = new T[assetIds.Count];
            for (int i = 0; i < assetIds.Count; i++)
            {
                assets[i] = AssetLookupTable<T>.ForwardLookup(assetIds[i]);
            }
            return assets;
        }
        
        public void SetAssetArray<T>(string key, T[] assets) where T : Object
        {
            string[] assetIds = new string[assets.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                assetIds[i] = AssetLookupTable<T>.BackwardLookup(assets[i]);
            }
            this.assetIdArrayData.Set(key, new Nested1DList<string>(assetIds));
        }

        [Serializable]
        private class DataCollection<T>
        {
            [SerializeField]
            private List<string> keys;
            [SerializeField]
            private List<T> values;

            public DataCollection()
            {
                this.keys = new List<string>();
                this.values = new List<T>();
            }

            public bool IsEmpty
            {
                get
                {
                    return this.keys.Count == 0;
                }
            }

            public void Set(string key, T value)
            {
                int index = this.keys.IndexOf(key);
                if (index == -1)
                {
                    this.keys.Add(key);
                    this.values.Add(value);
                    return;
                }
                this.values[index] = value;
            }

            public T Get(string key)
            {
                int index = this.keys.IndexOf(key);
                if (index == -1)
                {
                    return default;
                }
                return this.values[index];
            }
        }
    }
}
