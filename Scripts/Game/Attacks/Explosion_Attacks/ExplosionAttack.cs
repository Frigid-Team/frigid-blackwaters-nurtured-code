using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class ExplosionAttack : Attack
    {
        private static SceneVariable<Dictionary<Explosion, RecyclePool<Explosion>>> explosionPools;

        static ExplosionAttack()
        {
            explosionPools = new SceneVariable<Dictionary<Explosion, RecyclePool<Explosion>>>(() => new Dictionary<Explosion, RecyclePool<Explosion>>());
        }

        public override void Perform(float elapsedDuration, Action onComplete)
        {
            if (!TiledArea.TryGetAreaAtPosition(this.transform.position, out TiledArea tiledArea))
            {
                return;
            }

            foreach (ExplosionSpawnParameters explosionSpawnParameters in this.GetExplosionSpawnParameters(tiledArea, elapsedDuration))
            {
                if (!AreaTiling.TilePositionWithinBounds(explosionSpawnParameters.SpawnPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions))
                {
                    continue;
                }

                FrigidCoroutine.Run(
                    Tween.Delay(
                        explosionSpawnParameters.DelayDuration, 
                        () => 
                        {
                            if (!explosionPools.Current.ContainsKey(explosionSpawnParameters.ExplosionPrefab))
                            {
                                explosionPools.Current.Add(
                                    explosionSpawnParameters.ExplosionPrefab,
                                    new RecyclePool<Explosion>(
                                        () => CreateInstance<Explosion>(explosionSpawnParameters.ExplosionPrefab),
                                        (Explosion explosion) => DestroyInstance(explosion)
                                        )
                                    );
                            }
                            RecyclePool<Explosion> explosionPool = explosionPools.Current[explosionSpawnParameters.ExplosionPrefab];
                            Explosion spawnedExplosion = explosionPool.Retrieve();
                            spawnedExplosion.SummonExplosion(
                                this.DamageBonus,
                                this.DamageAlignment,
                                () => explosionPool.Pool(spawnedExplosion),
                                explosionSpawnParameters.SpawnPosition,
                                explosionSpawnParameters.SummonRotationDeg,
                                this.OnHitDealt,
                                this.OnBreakDealt,
                                this.OnThreatDealt
                                );
                        }
                        ),
                    this.gameObject
                    );
            }
        }

        protected abstract List<ExplosionSpawnParameters> GetExplosionSpawnParameters(TiledArea tiledArea, float elapsedDuration);

        protected struct ExplosionSpawnParameters
        {
            private Vector2 spawnPosition;
            private float summonRotationDeg;
            private float delayDuration;
            private Explosion explosionPrefab;

            public ExplosionSpawnParameters(Vector2 spawnPosition, float summonRotationDeg, float delayDuration, Explosion explosionPrefab)
            {
                this.spawnPosition = spawnPosition;
                this.summonRotationDeg = summonRotationDeg;
                this.delayDuration = delayDuration;
                this.explosionPrefab = explosionPrefab;
            }

            public Vector2 SpawnPosition
            {
                get
                {
                    return this.spawnPosition;
                }
            }

            public float SummonRotationDeg
            {
                get
                {
                    return this.summonRotationDeg;
                }
            }

            public float DelayDuration
            {
                get
                {
                    return this.delayDuration;
                }
            }

            public Explosion ExplosionPrefab
            {
                get
                {
                    return this.explosionPrefab;
                }
            }
        }
    }
}
