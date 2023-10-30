using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class BossHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private BarHUD childHealthBarHUD;
        [SerializeField]
        private List<HealthBarAlignmentSetting> healthBarAlignmentSettings;

        private Dictionary<BossMob, (BarHUD barHUD, Action<int, int> onRemainingHealthChanged, Action<int, int> onMaxHealthChanged)> displayedBosses;
        private RecyclePool<BarHUD> barHUDPool;

        protected override void Awake()
        {
            base.Awake();
            this.displayedBosses = new Dictionary<BossMob, (BarHUD barHUD, Action<int, int> onRemainingHealthChanged, Action<int, int> onMaxHealthChanged)>();
            this.barHUDPool = new RecyclePool<BarHUD>(() => CreateInstance<BarHUD>(this.childHealthBarHUD, this.transform, false), (BarHUD instance) => DestroyInstance(instance));
            this.barHUDPool.Return(this.childHealthBarHUD);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BossMob.OnCurrentBossAdded += this.AddHealthBar;
            BossMob.OnCurrentBossRemoved += this.RemoveHealthBar;
            foreach (BossMob bossMob in BossMob.CurrentBosses)
            {
                this.AddHealthBar(bossMob);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BossMob.OnCurrentBossAdded -= this.AddHealthBar;
            BossMob.OnCurrentBossRemoved -= this.RemoveHealthBar;
            foreach (BossMob bossMob in BossMob.CurrentBosses)
            {
                this.RemoveHealthBar(bossMob);
            }
        }

        private void AddHealthBar(BossMob boss)
        {
            BarHUD newBarHUD = this.barHUDPool.Retrieve();
            this.displayedBosses.Add(boss, (newBarHUD, (int prevRemainingHealth, int currRemainingHealth) => newBarHUD.SetCurrent(currRemainingHealth), (int prevMaxHealth, int currMaxHealth) => newBarHUD.SetMaximum(currMaxHealth)));

            newBarHUD.MainBarColor = Color.clear;
            newBarHUD.BufferBarColor = Color.clear;
            foreach (HealthBarAlignmentSetting healthBarAlignmentSetting in this.healthBarAlignmentSettings)
            {
                if (healthBarAlignmentSetting.DamageAlignment == boss.Alignment)
                {
                    newBarHUD.MainBarColor = healthBarAlignmentSetting.MainBarColor;
                    newBarHUD.BufferBarColor = healthBarAlignmentSetting.BufferBarColor;
                    break;
                }
            }

            boss.OnRemainingHealthChanged += this.displayedBosses[boss].onRemainingHealthChanged;
            boss.OnMaxHealthChanged += this.displayedBosses[boss].onMaxHealthChanged;

            newBarHUD.Transition(boss.RemainingHealth, boss.MaxHealth, 0f);
        }

        private void RemoveHealthBar(BossMob boss)
        {
            boss.OnRemainingHealthChanged -= this.displayedBosses[boss].onRemainingHealthChanged;
            boss.OnMaxHealthChanged -= this.displayedBosses[boss].onMaxHealthChanged;

            this.barHUDPool.Return(this.displayedBosses[boss].barHUD);
            this.displayedBosses.Remove(boss);
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        [Serializable]
        private struct HealthBarAlignmentSetting
        {
            [SerializeField]
            private DamageAlignment damageAlignment;
            [SerializeField]
            private ColorSerializedReference mainBarColor;
            [SerializeField]
            private ColorSerializedReference bufferBarColor;

            public DamageAlignment DamageAlignment
            {
                get
                {
                    return this.damageAlignment;
                }
            }

            public Color MainBarColor
            {
                get
                {
                    return this.mainBarColor.ImmutableValue;
                }
            }

            public Color BufferBarColor
            {
                get
                {
                    return this.bufferBarColor.ImmutableValue;
                }
            }
        }
    }
}
