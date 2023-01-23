using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldExplorer : FrigidMonoBehaviour
    {
        private static SceneVariable<HashSet<TiledArea>> exploredTiledAreas;
        private static Action<TiledArea> onExploredNewTiledArea;

        [SerializeField]
        private List<TiledAreaOccupier> tiledAreaOccupiers;

        static TiledWorldExplorer()
        {
            exploredTiledAreas = new SceneVariable<HashSet<TiledArea>>(() => { return new HashSet<TiledArea>(); });
        }

        public static HashSet<TiledArea> ExploredTiledAreas
        {
            get
            {
                return exploredTiledAreas.Current;
            }
        }

        public static Action<TiledArea> OnExploredNewTiledArea
        {
            get
            {
                return onExploredNewTiledArea;
            }
            set
            {
                onExploredNewTiledArea = value;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (TiledAreaOccupier tiledAreaOccupier in this.tiledAreaOccupiers)
            {
                tiledAreaOccupier.OnCurrentTiledAreaChanged += ExploreTiledArea;
                if (tiledAreaOccupier.TryGetCurrentTiledArea(out TiledArea tiledArea)) ExploreTiledArea(tiledArea);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (TiledAreaOccupier tiledAreaOccupier in this.tiledAreaOccupiers)
            {
                tiledAreaOccupier.OnCurrentTiledAreaChanged -= ExploreTiledArea;
            }
        }

        private void ExploreTiledArea(bool hasOldTiledArea, TiledArea oldTiledArea, bool hasNewTiledArea, TiledArea newTiledArea)
        {
            if (hasNewTiledArea) ExploreTiledArea(newTiledArea);
        }

        private void ExploreTiledArea(TiledArea tiledArea)
        {
            if (exploredTiledAreas.Current.Add(tiledArea))
            {
                onExploredNewTiledArea?.Invoke(tiledArea);
            }
        }
    }
}
