using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldDiscovery : FrigidMonoBehaviour
    {
        private static SceneVariable<Dictionary<TiledArea, HashSet<TiledWorldDiscovery>>> tiledWorldDiscoveriesPerTiledAreas;
        private static Action<TiledWorldDiscovery> onTiledWorldDiscoveryRevealed;
        private static Action<TiledWorldDiscovery> onTiledWorldDiscoveryHidden;

        [SerializeField]
        private DiscoveryType discoveryType;
        [SerializeField]
        [ShowIfInt("discoveryType", 0, true)]
        private Mob mob;
        [SerializeField]
        [ShowIfInt("discoveryType", 0, true)]
        private bool showForMobClassification;
        [SerializeField]
        [ShowIfInt("discoveryType", 0, true)]
        [ShowIfBool("showForMobClassification", true)]
        private MobClassification mobClassification;
        [SerializeField]
        [ShowIfInt("discoveryType", 1, true)]
        private TerrainContent terrainContent;
        [SerializeField]
        private Sprite mapTokenSprite;

        static TiledWorldDiscovery()
        {
            tiledWorldDiscoveriesPerTiledAreas = new SceneVariable<Dictionary<TiledArea, HashSet<TiledWorldDiscovery>>>(() => new Dictionary<TiledArea, HashSet<TiledWorldDiscovery>>());
        }

        public static Action<TiledWorldDiscovery> OnTiledWorldDiscoveryRevealed
        {
            get
            {
                return onTiledWorldDiscoveryRevealed;
            }
            set
            {
                onTiledWorldDiscoveryRevealed = value;
            }
        }

        public static Action<TiledWorldDiscovery> OnTiledWorldDiscoveryHidden
        {
            get
            {
                return onTiledWorldDiscoveryHidden;
            }
            set
            {
                onTiledWorldDiscoveryHidden = value;
            }
        }

        public Sprite MapTokenSprite
        {
            get
            {
                return this.mapTokenSprite;
            }
        }

        public static bool TryGetDiscoveriesInTiledArea(TiledArea tiledArea, out HashSet<TiledWorldDiscovery> tiledWorldDiscoveries)
        {
            return tiledWorldDiscoveriesPerTiledAreas.Current.TryGetValue(tiledArea, out tiledWorldDiscoveries);
        }

        protected override void Awake()
        {
            base.Awake();
            if (this.discoveryType == DiscoveryType.Mob)
            {
                this.mob.OnTiledAreaChanged += UpdateDiscoveryOnMobTiledAreaChange;
                if (this.showForMobClassification) this.mob.OnClassificationChanged += UpdateDiscoveryOnMobClassificationChange;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.discoveryType == DiscoveryType.Mob)
            {
                this.mob.OnTiledAreaChanged -= UpdateDiscoveryOnMobTiledAreaChange;
                if (this.showForMobClassification) this.mob.OnClassificationChanged -= UpdateDiscoveryOnMobClassificationChange;
            }
        }

        protected override void Start()
        {
            base.Start();
            switch (this.discoveryType)
            {
                case DiscoveryType.Mob:
                    if (!this.showForMobClassification || this.mobClassification == this.mob.Classification)
                    {
                        AddDiscovery(this.mob.TiledArea);
                    }
                    break;
                case DiscoveryType.TerrainContent:
                    if (TiledArea.TryGetTiledAreaAtPosition(this.terrainContent.transform.position, out TiledArea tiledArea))
                    {
                        AddDiscovery(tiledArea);
                    }
                    break;
            }
        }

        private void UpdateDiscoveryOnMobClassificationChange()
        {
            if (this.mobClassification == this.mob.Classification)
            {
                AddDiscovery(this.mob.TiledArea);
            }
            else
            {
                RemoveDiscovery(this.mob.TiledArea);
            }
        }

        private void UpdateDiscoveryOnMobTiledAreaChange(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            if (!this.showForMobClassification || this.mobClassification == this.mob.Classification)
            {
                RemoveDiscovery(previousTiledArea);
                AddDiscovery(currentTiledArea);
            }
        }

        private void AddDiscovery(TiledArea tiledArea)
        {
            if (!tiledWorldDiscoveriesPerTiledAreas.Current.ContainsKey(tiledArea))
            {
                tiledWorldDiscoveriesPerTiledAreas.Current.Add(tiledArea, new HashSet<TiledWorldDiscovery>());
            }
            if (tiledWorldDiscoveriesPerTiledAreas.Current[tiledArea].Add(this))
            {
                onTiledWorldDiscoveryRevealed?.Invoke(this);
            }
        }

        private void RemoveDiscovery(TiledArea tiledArea)
        {
            if (tiledWorldDiscoveriesPerTiledAreas.Current.ContainsKey(tiledArea) && tiledWorldDiscoveriesPerTiledAreas.Current[tiledArea].Remove(this))
            {
                onTiledWorldDiscoveryHidden?.Invoke(this);
                if (tiledWorldDiscoveriesPerTiledAreas.Current[tiledArea].Count == 0)
                {
                    tiledWorldDiscoveriesPerTiledAreas.Current.Remove(tiledArea);
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
