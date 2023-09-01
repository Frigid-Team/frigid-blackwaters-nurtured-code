using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldDiscovery : FrigidMonoBehaviour
    {
        private static SceneVariable<Dictionary<TiledArea, HashSet<TiledWorldDiscovery>>> discoveriesPerAreas;
        private static Action<TiledWorldDiscovery> onDiscoveryRevealed;
        private static Action<TiledWorldDiscovery> onDiscoveryHidden;

        [SerializeField]
        private DiscoveryType discoveryType;
        [SerializeField]
        [ShowIfInt("discoveryType", 0, true)]
        private Mob mob;
        [SerializeField]
        [ShowIfInt("discoveryType", 0, true)]
        private bool showIfStatusTag;
        [SerializeField]
        [ShowIfInt("discoveryType", 0, true)]
        [ShowIfBool("showIfStatusTag", true)]
        private MobStatusTag mobStatusTag;
        [SerializeField]
        [ShowIfInt("discoveryType", 1, true)]
        private TerrainContent terrainContent;
        [SerializeField]
        private Sprite mapTokenSprite;

        static TiledWorldDiscovery()
        {
            discoveriesPerAreas = new SceneVariable<Dictionary<TiledArea, HashSet<TiledWorldDiscovery>>>(() => new Dictionary<TiledArea, HashSet<TiledWorldDiscovery>>());
        }

        public static Action<TiledWorldDiscovery> OnDiscoveryRevealed
        {
            get
            {
                return onDiscoveryRevealed;
            }
            set
            {
                onDiscoveryRevealed = value;
            }
        }

        public static Action<TiledWorldDiscovery> OnDiscoveryHidden
        {
            get
            {
                return onDiscoveryHidden;
            }
            set
            {
                onDiscoveryHidden = value;
            }
        }

        public Sprite MapTokenSprite
        {
            get
            {
                return this.mapTokenSprite;
            }
        }

        public static bool TryGetDiscoveriesInTiledArea(TiledArea tiledArea, out HashSet<TiledWorldDiscovery> discoveries)
        {
            return discoveriesPerAreas.Current.TryGetValue(tiledArea, out discoveries);
        }

        protected override void Awake()
        {
            base.Awake();
            if (this.discoveryType == DiscoveryType.Mob)
            {
                this.mob.OnTiledAreaChanged += this.UpdateDiscoveryOnMobTiledAreaChange;
                if (this.showIfStatusTag)
                {
                    this.mob.OnStatusTagAdded += this.UpdateDiscoveryOnMobStatusTagChange;
                    this.mob.OnStatusTagRemoved += this.UpdateDiscoveryOnMobStatusTagChange;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.discoveryType == DiscoveryType.Mob)
            {
                this.mob.OnTiledAreaChanged -= this.UpdateDiscoveryOnMobTiledAreaChange;
                if (this.showIfStatusTag)
                {
                    this.mob.OnStatusTagAdded -= this.UpdateDiscoveryOnMobStatusTagChange;
                    this.mob.OnStatusTagRemoved -= this.UpdateDiscoveryOnMobStatusTagChange;
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            switch (this.discoveryType)
            {
                case DiscoveryType.Mob:
                    if (!this.showIfStatusTag || this.mob.HasStatusTag(this.mobStatusTag))
                    {
                        this.AddDiscovery(this.mob.TiledArea);
                    }
                    break;
                case DiscoveryType.TerrainContent:
                    if (TiledArea.TryGetAreaAtPosition(this.terrainContent.transform.position, out TiledArea area))
                    {
                        this.AddDiscovery(area);
                    }
                    break;
            }
        }

        private void UpdateDiscoveryOnMobStatusTagChange(MobStatusTag changedMobStatusTag)
        {
            if (this.mob.HasStatusTag(this.mobStatusTag))
            {
                this.AddDiscovery(this.mob.TiledArea);
            }
            else
            {
                this.RemoveDiscovery(this.mob.TiledArea);
            }
        }

        private void UpdateDiscoveryOnMobTiledAreaChange(TiledArea previousArea, TiledArea currentArea)
        {
            if (!this.showIfStatusTag || this.mob.HasStatusTag(this.mobStatusTag))
            {
                this.RemoveDiscovery(previousArea);
                this.AddDiscovery(currentArea);
            }
        }

        private void AddDiscovery(TiledArea area)
        {
            if (!discoveriesPerAreas.Current.ContainsKey(area))
            {
                discoveriesPerAreas.Current.Add(area, new HashSet<TiledWorldDiscovery>());
            }
            if (discoveriesPerAreas.Current[area].Add(this))
            {
                onDiscoveryRevealed?.Invoke(this);
            }
        }

        private void RemoveDiscovery(TiledArea area)
        {
            if (discoveriesPerAreas.Current.ContainsKey(area) && discoveriesPerAreas.Current[area].Remove(this))
            {
                onDiscoveryHidden?.Invoke(this);
                if (discoveriesPerAreas.Current[area].Count == 0)
                {
                    discoveriesPerAreas.Current.Remove(area);
                }
            }
        }

        public enum DiscoveryType
        {
            Mob,
            TerrainContent
        }
    }
}
