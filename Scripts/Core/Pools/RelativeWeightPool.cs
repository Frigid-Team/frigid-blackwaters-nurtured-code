using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class RelativeWeightPool<T>
    {
        [SerializeField]
        private List<Weighting> weightings;

        public HashSet<T> Entries
        {
            get
            {
                HashSet<T> entries = new HashSet<T>();
                foreach (Weighting weighting in this.weightings)
                {
                    entries.Add(weighting.Entry);
                }
                return entries;
            }
        }

        public T Retrieve()
        {
            return this.Retrieve(1)[0];
        }

        public T[] Retrieve(int quantity)
        {
            List<T> retrievals = new List<T>();

            Dictionary<Weighting, int> retrievalCounts = new Dictionary<Weighting, int>();
            foreach (Weighting weighting in this.weightings)
            {
                if (weighting.RequireRetrievals)
                {
                    if (!weighting.LimitRetrievals || weighting.MaximumRetrievals > weighting.MinimumRetrievals)
                    {
                        retrievalCounts.TryAdd(weighting, weighting.MinimumRetrievals);
                    }
                    for (int i = 0; i < Mathf.Min(quantity - retrievals.Count, weighting.MinimumRetrievals); i++)
                    {
                        retrievals.Add(weighting.Entry);
                    }
                }
                else
                {
                    retrievalCounts.TryAdd(weighting, 0);
                }

                if (retrievals.Count == quantity) return retrievals.ToArray();
            }

            while (retrievals.Count < quantity && retrievalCounts.Count > 0)
            {
                float cumulativeWeight = 0;
                foreach (KeyValuePair<Weighting, int> retrievalCount in retrievalCounts)
                {
                    Weighting weighting = retrievalCount.Key;
                    cumulativeWeight += weighting.RelativeWeight;
                }

                float chosenWeight = UnityEngine.Random.Range(0, cumulativeWeight);

                float summedWeight = 0;
                foreach (KeyValuePair<Weighting, int> retrievalCount in retrievalCounts)
                {
                    Weighting weighting = retrievalCount.Key;
                    summedWeight += weighting.RelativeWeight;
                    if (chosenWeight <= summedWeight)
                    {
                        retrievals.Add(weighting.Entry);
                        retrievalCounts[weighting]++;
                        if (weighting.LimitRetrievals && retrievalCounts[weighting] >= weighting.MaximumRetrievals)
                        {
                            retrievalCounts.Remove(weighting);
                        }
                        break;
                    }
                }
            }
            
            for (int i = retrievals.Count; i < quantity; i++)
            {
                retrievals.Add(default(T));
            }

            return retrievals.OrderBy((T retrieval) => { return UnityEngine.Random.Range(0, int.MaxValue); }).ToArray();
        }

        [Serializable]
        private class Weighting
        {
            [SerializeField]
            private T entry;
            [SerializeField]
            private FloatSerializedReference relativeWeight;
            [SerializeField]
            private bool requireRetrievals;
            [SerializeField]
            [ShowIfBool("requireRetrievals", true)]
            private IntSerializedReference minimumRetrievals;
            [SerializeField] 
            private bool limitRetrievals;
            [SerializeField]
            [ShowIfBool("limitRetrievals", true)]
            private IntSerializedReference maximumRetrievals;

            public T Entry
            {
                get
                {
                    return this.entry;
                }
            }

            public float RelativeWeight
            {
                get
                {
                    return this.relativeWeight.ImmutableValue;
                }
            }

            public bool RequireRetrievals
            {
                get
                {
                    return this.requireRetrievals;
                }
            }

            public int MinimumRetrievals
            {
                get
                {
                    return this.minimumRetrievals.ImmutableValue;
                }
            }

            public bool LimitRetrievals
            {
                get
                {
                    return this.limitRetrievals;
                }
            }

            public int MaximumRetrievals
            {
                get
                {
                    return this.maximumRetrievals.ImmutableValue;
                }
            }
        }
    }
}
