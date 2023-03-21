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
        private Mob mob;

        static TiledWorldExplorer()
        {
            exploredTiledAreas = new SceneVariable<HashSet<TiledArea>>(() => new HashSet<TiledArea>());
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

        protected override void Awake()
        {
            base.Awake();
            this.mob.OnTiledAreaChanged += ExploreTiledArea;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.mob.OnTiledAreaChanged -= ExploreTiledArea;
        }

        protected override void Start()
        {
            base.Start();
            ExploreTiledArea(this.mob.TiledArea);
        }

        private void ExploreTiledArea(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            ExploreTiledArea(currentTiledArea);
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
