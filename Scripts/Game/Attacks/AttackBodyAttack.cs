using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class AttackBodyAttack<SS, AB> : Attack where SS : AttackBodyAttack<SS, AB>.SpawnSetting where AB : AttackBody
    {
        private static SceneVariable<Dictionary<AB, RecyclePool<AB>>> attackBodyPools;

        static AttackBodyAttack()
        {
            attackBodyPools = new SceneVariable<Dictionary<AB, RecyclePool<AB>>>(() => new Dictionary<AB, RecyclePool<AB>>());
        }

        public override void Perform(float elapsedDuration, ref Action toForceComplete, Action onComplete)
        {
            if (!TiledArea.TryGetAreaAtPosition(this.transform.position, out TiledArea tiledArea))
            {
                return;
            }

            List<SS> spawnSettings = this.GetSpawnSettings(tiledArea, elapsedDuration);
            int numberSpawnSettings = spawnSettings.Count;
            if (numberSpawnSettings == 0)
            {
                onComplete?.Invoke();
                return;
            }
            int numberSpawnSettingsCompleted = 0;

            foreach (SS spawnSetting in spawnSettings)
            {
                if (!AreaTiling.TilePositionWithinBounds(spawnSetting.SpawnPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions))
                {
                    continue;
                }

                Action toForceBodyComplete = null;
                toForceComplete += () => toForceBodyComplete?.Invoke();
                FrigidCoroutine spawnRoutine = FrigidCoroutine.Run(
                    Tween.Delay(
                        spawnSetting.DelayDuration,
                        () =>
                        {
                            if (!attackBodyPools.Current.ContainsKey(spawnSetting.Prefab))
                            {
                                attackBodyPools.Current.Add(spawnSetting.Prefab, new RecyclePool<AB>(() => CreateInstance<AB>(spawnSetting.Prefab), (AB instance) => DestroyInstance(instance)));
                            }

                            RecyclePool<AB> attackBodyPool = attackBodyPools.Current[spawnSetting.Prefab];
                            AB attackBody = attackBodyPool.Retrieve();

                            spawnSetting.OnBodyInitialized?.Invoke(attackBody);

                            attackBody.DoLifetime(
                                this.DamageAlignment,
                                this.IsIgnoringDamage,
                                this.DamageBonus,
                                tiledArea,
                                spawnSetting.SpawnPosition, 
                                this.OnHitDealt,
                                this.OnBreakDealt, 
                                this.OnThreatDealt,
                                ref toForceBodyComplete,
                                () =>
                                {
                                    spawnSetting.OnBodyDeinitialized?.Invoke(attackBody);

                                    attackBodyPool.Return(attackBody);
                                    numberSpawnSettingsCompleted++;
                                    if (numberSpawnSettingsCompleted == numberSpawnSettings)
                                    {
                                        onComplete?.Invoke();
                                    }
                                }
                                );
                        }
                        ),
                    this.gameObject
                    );
                toForceBodyComplete += () => FrigidCoroutine.Kill(spawnRoutine);
            }
        }

        public abstract List<SS> GetSpawnSettings(TiledArea tiledArea, float elapsedDuration);

        public abstract class SpawnSetting
        {
            private Vector2 spawnPosition;
            private float delayDuration;
            private AB prefab;
            private Action<AB> onBodyInitialized;
            private Action<AB> onBodyDeinitialized;

            public SpawnSetting(Vector2 spawnPosition, float delayDuration, AB prefab)
            {
                this.spawnPosition = spawnPosition;
                this.delayDuration = delayDuration;
                this.prefab = prefab;
            }

            public Vector2 SpawnPosition
            {
                get
                {
                    return this.spawnPosition;
                }
            }

            public float DelayDuration
            {
                get
                {
                    return this.delayDuration;
                }
            }

            public AB Prefab
            {
                get
                {
                    return this.prefab;
                }
            }

            public Action<AB> OnBodyInitialized
            {
                get
                {
                    return this.onBodyInitialized;
                }
                set
                {
                    this.onBodyInitialized = value;
                }
            }

            public Action<AB> OnBodyDeinitialized
            {
                get
                {
                    return this.onBodyDeinitialized;
                }
                set
                {
                    this.onBodyDeinitialized = value;
                }
            }
        }
    }
}
