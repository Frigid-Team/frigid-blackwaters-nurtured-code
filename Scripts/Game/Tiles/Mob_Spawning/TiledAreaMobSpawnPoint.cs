using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class TiledAreaMobSpawnPoint : IEquatable<TiledAreaMobSpawnPoint>
    {
        [SerializeField]
        private Vector2 localPosition;
        [SerializeField]
        private bool isExclusive;
        [SerializeField]
        private List<MobSpawnable> filteredMobSpawnables;
        [SerializeField]
        private List<MobSpawnTag> filteredMobSpawnTags;
        [SerializeField]
        private List<Core.Span<int>> filteredTierSpans;

        public Vector2 LocalPosition
        {
            get
            {
                return this.localPosition;
            }
        }

        public bool IsExclusive
        {
            get
            {
                return this.isExclusive;
            }
        }

        public IReadOnlyList<MobSpawnable> FilteredMobSpawnables
        {
            get
            {
                return this.filteredMobSpawnables;
            }
        }

        public IReadOnlyList<MobSpawnTag> FilteredMobSpawnTags
        {
            get
            {
                return this.filteredMobSpawnTags;
            }
        }

        public IReadOnlyList<Core.Span<int>> FilteredTierSpans
        {
            get
            {
                return this.filteredTierSpans;
            }
        }

        public TiledAreaMobSpawnPoint(Vector2 localPosition) : this(localPosition, false, new MobSpawnable[0], new MobSpawnTag[0], new Core.Span<int>[0]) { }

        public TiledAreaMobSpawnPoint(Vector2 localPosition, bool isExclusive, IEnumerable<MobSpawnable> filteredMobSpawnables, IEnumerable<MobSpawnTag> filteredMobSpawnTags, IEnumerable<Core.Span<int>> filteredTierSpans)
        {
            this.localPosition = localPosition;
            this.isExclusive = isExclusive;
            this.filteredMobSpawnables = new List<MobSpawnable>(filteredMobSpawnables);
            this.filteredMobSpawnTags = new List<MobSpawnTag>(filteredMobSpawnTags);
            this.filteredTierSpans = new List<Core.Span<int>>(filteredTierSpans);
        }

        public Vector2 GetSpawnPosition(TiledArea tiledArea)
        {
            return this.localPosition + tiledArea.CenterPosition;
        }

        public bool CanSpawnHere(MobSpawnable mobSpawnable, TiledArea tiledArea)
        {
            if (this.isExclusive)
            {
                foreach (Core.Span<int> filteredTierSpan in this.filteredTierSpans)
                {
                    if (!filteredTierSpan.Within(mobSpawnable.Tier))
                    {
                        return false;
                    }
                }
                if (this.filteredMobSpawnables.Count > 0 && !this.filteredMobSpawnables.Contains(mobSpawnable))
                {
                    return false;
                }
                if (this.filteredMobSpawnTags.Count > 0 && this.filteredMobSpawnTags.Intersect(mobSpawnable.SpawnTags).Count() == 0)
                {
                    return false;
                }
            }
            else
            {
                foreach (Core.Span<int> filteredTierSpan in this.filteredTierSpans)
                {
                    if (filteredTierSpan.Within(mobSpawnable.Tier))
                    {
                        return false;
                    }
                }
                if (this.filteredMobSpawnables.Count > 0 && this.filteredMobSpawnables.Contains(mobSpawnable) || 
                    this.filteredMobSpawnTags.Count > 0 && this.filteredMobSpawnTags.Intersect(mobSpawnable.SpawnTags).Count() > 0)
                {
                    return false;
                }
            }
            return mobSpawnable.CanSpawnMobAt(this.localPosition + tiledArea.CenterPosition);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TiledAreaMobSpawnPoint);
        }

        public bool Equals(TiledAreaMobSpawnPoint other)
        {
            if (other is null)
            {
                return false;
            }
            return
                other.localPosition == this.localPosition && other.isExclusive == this.isExclusive &&
                other.filteredMobSpawnables.SequenceEqual(this.filteredMobSpawnables) && other.filteredMobSpawnTags.SequenceEqual(this.filteredMobSpawnTags) &&
                other.filteredTierSpans.SequenceEqual(this.filteredTierSpans);
        }

        public override int GetHashCode()
        {
            return this.localPosition.GetHashCode();
        }

        public static bool operator ==(TiledAreaMobSpawnPoint lhs, TiledAreaMobSpawnPoint rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(TiledAreaMobSpawnPoint lhs, TiledAreaMobSpawnPoint rhs)
        {
            return !(lhs == rhs);
        }
    }
}
