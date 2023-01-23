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

        // Inspector_TODO: Discovery type hiding attributes
        [SerializeField]
        private DiscoveryType discoveryType;
        [SerializeField]
        [ShowIfInt("discoveryType", 0, true)]
        private Mob mob;
        [SerializeField]
        [ShowIfInt("discoveryType", 1, true)]
        private TerrainContent terrainContent;
        [SerializeField]
        private Sprite mapTokenSprite;

        static TiledWorldDiscovery()
        {
            tiledWorldDiscoveriesPerTiledAreas = new SceneVariable<Dictionary<TiledArea, HashSet<TiledWorldDiscovery>>>(() => { return new Dictionary<TiledArea, HashSet<TiledWorldDiscovery>>(); });
            TiledWorldExplorer.OnExploredNewTiledArea += (TiledArea exploredTiledArea) => { tiledWorldDiscoveriesPerTiledAreas.Current.Add(exploredTiledArea, new HashSet<TiledWorldDiscovery>()); };
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
            TiledWorldExplorer.OnExploredNewTiledArea += EvaluateDiscovery;
            if (this.discoveryType == DiscoveryType.Mob)
            {
                this.mob.OnTiledAreaChanged += EvaluateDiscovery;
                this.mob.Active.OnSet += EvaluateDiscovery;
                this.mob.Active.OnUnset += EvaluateDiscovery;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TiledWorldExplorer.OnExploredNewTiledArea -= EvaluateDiscovery;
            if (this.discoveryType == DiscoveryType.Mob)
            {
                this.mob.OnTiledAreaChanged -= EvaluateDiscovery;
                this.mob.Active.OnSet -= EvaluateDiscovery;
                this.mob.Active.OnUnset -= EvaluateDiscovery;
            }
        }

        private void EvaluateDiscovery(bool hasOldTiledArea, TiledArea oldTiledArea, bool hasNewTiledArea, TiledArea newTiledArea)
        {
            if (hasOldTiledArea) HideDiscovery(oldTiledArea);
            EvaluateDiscovery();
        }

        private void EvaluateDiscovery(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            EvaluateDiscovery();
        }

        private void EvaluateDiscovery(TiledArea exploredTiledArea)
        {
            EvaluateDiscovery();
        }

        private void EvaluateDiscovery()
        {
            switch (this.discoveryType)
            {
                case DiscoveryType.Mob:
                    if (this.mob.Active) RevealDiscovery(this.mob.TiledArea);
                    else HideDiscovery(this.mob.TiledArea);
                    break;
                case DiscoveryType.TerrainContent:
                    if (TiledArea.TryGetTiledAreaAtPosition(this.terrainContent.transform.position, out TiledArea tileContentTiledArea))
                    {
                        RevealDiscovery(tileContentTiledArea);
                    }
                    break;
            }
        }

        private void RevealDiscovery(TiledArea tiledArea)
        {
            if (tiledWorldDiscoveriesPerTiledAreas.Current.ContainsKey(tiledArea) &&
                tiledWorldDiscoveriesPerTiledAreas.Current[tiledArea].Add(this))
            {
                onTiledWorldDiscoveryRevealed?.Invoke(this);
            }
        }

        private void HideDiscovery(TiledArea tiledArea)
        {
            if (tiledWorldDiscoveriesPerTiledAreas.Current.ContainsKey(tiledArea) &&
                tiledWorldDiscoveriesPerTiledAreas.Current[tiledArea].Remove(this))
            {
                onTiledWorldDiscoveryHidden?.Invoke(this);
            }
        }

        public enum DiscoveryType
        {
            Mob,
            TerrainContent
        }
    }
}
