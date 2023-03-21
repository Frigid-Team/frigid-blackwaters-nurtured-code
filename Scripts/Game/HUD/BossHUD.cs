using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class BossHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private BarHUD healthBarHUDOriginal;

        private Dictionary<BossMob, (BarHUD barHUD, Action<int, int> onRemainingHealthChanged, Action<int, int> onMaxHealthChanged)> displayedBosses;
        private RecyclePool<BarHUD> barHUDPool;

        protected override void Awake()
        {
            base.Awake();
            this.displayedBosses = new Dictionary<BossMob, (BarHUD barHUD, Action<int, int> onRemainingHealthChanged, Action<int, int> onMaxHealthChanged)>();
            this.barHUDPool = new RecyclePool<BarHUD>(() => FrigidInstancing.CreateInstance<BarHUD>(this.healthBarHUDOriginal, this.transform, false), (BarHUD instance) => FrigidInstancing.DestroyInstance(instance));
            this.barHUDPool.Pool(this.healthBarHUDOriginal);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BossMob.OnCurrentBossAdded += AddHealthBar;
            BossMob.OnCurrentBossRemoved += RemoveHealthBar;
            foreach (BossMob bossMob in BossMob.CurrentBosses)
            {
                AddHealthBar(bossMob);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BossMob.OnCurrentBossAdded -= AddHealthBar;
            BossMob.OnCurrentBossRemoved -= RemoveHealthBar;
            foreach (BossMob bossMob in BossMob.CurrentBosses)
            {
                RemoveHealthBar(bossMob);
            }
        }

        private void AddHealthBar(BossMob boss)
        {
            BarHUD newBarHUD = this.barHUDPool.Retrieve();
            this.displayedBosses.Add(boss, (newBarHUD, (int prevRemainingHealth, int currRemainingHealth) => newBarHUD.SetCurrent(currRemainingHealth), (int prevMaxHealth, int currMaxHealth) => newBarHUD.SetMaximum(currMaxHealth)));
            boss.OnRemainingHealthChanged += this.displayedBosses[boss].onRemainingHealthChanged;
            boss.OnMaxHealthChanged += this.displayedBosses[boss].onMaxHealthChanged;
            newBarHUD.Transition(boss.RemainingHealth, boss.MaxHealth, 0f);
        }

        private void RemoveHealthBar(BossMob boss)
        {
            boss.OnRemainingHealthChanged -= this.displayedBosses[boss].onRemainingHealthChanged;
            boss.OnMaxHealthChanged -= this.displayedBosses[boss].onMaxHealthChanged;
            this.barHUDPool.Pool(this.displayedBosses[boss].barHUD);
            this.displayedBosses.Remove(boss);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
