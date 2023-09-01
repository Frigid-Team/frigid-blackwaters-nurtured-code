using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldExplorer : FrigidMonoBehaviour
    {
        private static SceneVariable<HashSet<TiledArea>> exploredAreas;
        private static Action<TiledArea> onExploredArea;

        [SerializeField]
        private Mob mob;

        static TiledWorldExplorer()
        {
            exploredAreas = new SceneVariable<HashSet<TiledArea>>(() => new HashSet<TiledArea>());
        }

        public static HashSet<TiledArea> ExploredAreas
        {
            get
            {
                return exploredAreas.Current;
            }
        }

        public static Action<TiledArea> OnExploredArea
        {
            get
            {
                return onExploredArea;
            }
            set
            {
                onExploredArea = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.mob.OnTiledAreaChanged += this.ExploreArea;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.mob.OnTiledAreaChanged -= this.ExploreArea;
        }

        protected override void Start()
        {
            base.Start();
            this.ExploreArea(this.mob.TiledArea);
        }

        private void ExploreArea(TiledArea previousArea, TiledArea currentArea)
        {
            this.ExploreArea(currentArea);
        }

        private void ExploreArea(TiledArea area)
        {
            if (exploredAreas.Current.Add(area))
            {
                onExploredArea?.Invoke(area);
            }
        }
    }
}
