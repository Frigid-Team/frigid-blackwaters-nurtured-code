using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class BossMob : Mob
    {
        private static SceneVariable<HashSet<BossMob>> currentBosses;
        private static Action<BossMob> onCurrentBossAdded;
        private static Action<BossMob> onCurrentBossRemoved;

        static BossMob()
        {
            currentBosses = new SceneVariable<HashSet<BossMob>>(() => new HashSet<BossMob>());
        }

        public static HashSet<BossMob> CurrentBosses
        {
            get
            {
                return currentBosses.Current;
            }
        }

        public static Action<BossMob> OnCurrentBossAdded
        {
            get
            {
                return onCurrentBossAdded;
            }
            set
            {
                onCurrentBossAdded = value;
            }
        }

        public static Action<BossMob> OnCurrentBossRemoved
        {
            get
            {
                return onCurrentBossRemoved;
            }
            set
            {
                onCurrentBossRemoved = value;
            }
        }

        protected override void OnActive()
        {
            base.OnActive();
            TiledArea.OnFocusedAreaChanged += this.AddOrRemoveAsCurrentBossMob;
            this.OnTiledAreaChanged += this.AddOrRemoveAsCurrentBossMob;
            if (TiledArea.TryGetFocusedArea(out TiledArea focusedTiledArea) && focusedTiledArea == this.TiledArea)
            {
                if (currentBosses.Current.Add(this)) onCurrentBossAdded?.Invoke(this);
            }
        }

        protected override void OnInactive()
        {
            base.OnInactive();
            TiledArea.OnFocusedAreaChanged -= this.AddOrRemoveAsCurrentBossMob;
            this.OnTiledAreaChanged -= this.AddOrRemoveAsCurrentBossMob;
            if (TiledArea.TryGetFocusedArea(out TiledArea focusedTiledArea) && focusedTiledArea == this.TiledArea)
            {
                if (currentBosses.Current.Remove(this)) onCurrentBossRemoved?.Invoke(this);
            }
        }

        private void AddOrRemoveAsCurrentBossMob(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            this.AddOrRemoveAsCurrentBossMob();
        }

        private void AddOrRemoveAsCurrentBossMob()
        {
            if (TiledArea.TryGetFocusedArea(out TiledArea focusedTiledArea) && focusedTiledArea == this.TiledArea)
            {
                if (currentBosses.Current.Add(this)) onCurrentBossAdded?.Invoke(this);
            }
            else
            {
                if (currentBosses.Current.Remove(this)) onCurrentBossRemoved?.Invoke(this);
            }
        }
    }
}
