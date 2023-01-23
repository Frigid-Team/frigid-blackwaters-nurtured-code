using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class TraversableTerrain : IEquatable<TraversableTerrain>
    {
        [SerializeField]
        private List<TileTerrain> terrains;

        public TraversableTerrain()
        {
            this.terrains = new List<TileTerrain>();
        }

        public static TraversableTerrain All
        {
            get
            {
                TraversableTerrain traversableTerrain = new TraversableTerrain();
                for (int i = 0; i < (int)TileTerrain.Count; i++)
                {
                    traversableTerrain.terrains.Add((TileTerrain)i);
                }
                return traversableTerrain;
            }
        }

        public int TerrainCount
        {
            get
            {
                return this.terrains.Count;
            }
        }

        public bool Includes(TileTerrain terrain)
        {
            return this.terrains.Contains(terrain);
        }

        public void VisitTerrains(Action<TileTerrain> onVisited)
        {
            foreach (TileTerrain terrain in this.terrains)
            {
                onVisited?.Invoke(terrain);
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TraversableTerrain);
        }

        public bool Equals(TraversableTerrain other)
        {
            if (other is null)
            {
                return false;
            }
            int intersectCount = this.terrains.Intersect(other.terrains).Count();
            return intersectCount == this.terrains.Count && intersectCount == other.terrains.Count;
        }

        public override int GetHashCode()
        {
            int total = 0;
            foreach (TileTerrain terrain in this.terrains)
            {
                total += Mathf.RoundToInt(Mathf.Pow((int)terrain, 2));
            }
            return total.GetHashCode();
        }

        public static bool operator ==(TraversableTerrain t1, TraversableTerrain t2)
        {
            if (t1 is null)
            {
                if (t2 is null)
                {
                    return true;
                }

                return false;
            }

            return t1.Equals(t2);
        }

        public static bool operator !=(TraversableTerrain t1, TraversableTerrain t2)
        {
            return !(t1 == t2);
        }

        public static bool operator >=(TraversableTerrain t1, TraversableTerrain t2)
        {
            foreach (TileTerrain terrain in t2.terrains)
            {
                if (!t1.terrains.Contains(terrain))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator <=(TraversableTerrain t1, TraversableTerrain t2)
        {
            return !(t1 >= t2);
        }

        public static bool operator >(TraversableTerrain t1, TraversableTerrain t2)
        {
            return t1 >= t2 && t1 != t2;
        }

        public static bool operator <(TraversableTerrain t1, TraversableTerrain t2)
        {
            return !(t1 > t2);
        }
    }
}
