using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class ExplosionAttack : Attack
    {
        private static SceneVariable<Dictionary<Explosion, RecyclePool<Explosion>>> explosionPools;

        [SerializeField]
        private IntSerializedReference numberSpawns;
        [SerializeField]
        private FloatSerializedReference durationBetweenSpawns;
        [SerializeField]
        private Targeter baseSpawnTargeter;

        static ExplosionAttack()
        {
            explosionPools = new SceneVariable<Dictionary<Explosion, RecyclePool<Explosion>>>(() => new Dictionary<Explosion, RecyclePool<Explosion>>());
        }

        public override void Perform(float elapsedDuration)
        {
            IEnumerator<FrigidCoroutine.Delay> Perform()
            {
                Vector2[] baseSpawnPositions = this.baseSpawnTargeter.Calculate(new Vector2[this.numberSpawns.ImmutableValue], elapsedDuration, 0);
                foreach (Vector2 baseSpawnPosition in baseSpawnPositions)
                {
                    if (!TiledArea.TryGetTiledAreaAtPosition(baseSpawnPosition, out TiledArea tiledArea))
                    {
                        continue;
                    }

                    foreach (ExplosionSpawnParameters explosionSpawnParameters in GetExplosionSpawnParameters(baseSpawnPosition, elapsedDuration))
                    {
                        if (!TilePositioning.TilePositionWithinBounds(explosionSpawnParameters.SpawnPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions))
                        {
                            continue;
                        }

                        if (!explosionPools.Current.ContainsKey(explosionSpawnParameters.ExplosionPrefab))
                        {
                            explosionPools.Current.Add(
                                explosionSpawnParameters.ExplosionPrefab,
                                new RecyclePool<Explosion>(
                                    () => FrigidInstancing.CreateInstance<Explosion>(explosionSpawnParameters.ExplosionPrefab),
                                    (Explosion explosion) => FrigidInstancing.DestroyInstance(explosion)
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

                    yield return new FrigidCoroutine.DelayForSeconds(this.durationBetweenSpawns.MutableValue);
                }
            }
            FrigidCoroutine.Run(Perform(), this.gameObject);
        }

        protected abstract List<ExplosionSpawnParameters> GetExplosionSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration);

        protected struct ExplosionSpawnParameters
        {
            private Vector2 spawnPosition;
            private float summonRotationDeg;
            private Explosion explosionPrefab;

            public ExplosionSpawnParameters(Vector2 spawnPosition, float summonRotationDeg, Explosion explosionPrefab)
            {
                this.spawnPosition = spawnPosition;
                this.summonRotationDeg = summonRotationDeg;
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
