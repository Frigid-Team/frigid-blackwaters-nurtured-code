using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class ExplosionAttack : Attack
    {
        private static SceneVariable<Dictionary<Explosion, RecyclePool<Explosion>>> explosionPools;

        [SerializeField]
        private Targeter baseSpawnTargeter;

        static ExplosionAttack()
        {
            explosionPools = new SceneVariable<Dictionary<Explosion, RecyclePool<Explosion>>>(() => { return new Dictionary<Explosion, RecyclePool<Explosion>>(); });
        }

        public override void Perform(float elapsedDuration)
        {
            Vector2 baseSpawnPosition = this.baseSpawnTargeter.Calculate(this.transform.position, elapsedDuration, 0);

            if (!TiledArea.TryGetTiledAreaAtPosition(baseSpawnPosition, out TiledArea tiledArea))
            {
                return;
            }

            foreach (ExplosionSpawnParameters explosionSpawnParameters in GetExplosionSpawnParameters(baseSpawnPosition, elapsedDuration))
            {
                if (!TilePositioning.TileAbsolutePositionWithinBounds(explosionSpawnParameters.AbsoluteSpawnPosition, tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions))
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
                    explosionSpawnParameters.AbsoluteSpawnPosition, 
                    explosionSpawnParameters.SummonRotationDeg, 
                    this.OnHitDealt,
                    this.OnBreakDealt,
                    this.OnThreatDealt
                    );
            }
        }

        protected abstract List<ExplosionSpawnParameters> GetExplosionSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration);

        protected struct ExplosionSpawnParameters
        {
            private Vector2 absoluteSpawnPosition;
            private float summonRotationDeg;
            private Explosion explosionPrefab;

            public ExplosionSpawnParameters(Vector2 absoluteSpawnPosition, float summonRotationDeg, Explosion explosionPrefab)
            {
                this.absoluteSpawnPosition = absoluteSpawnPosition;
                this.summonRotationDeg = summonRotationDeg;
                this.explosionPrefab = explosionPrefab;
            }

            public Vector2 AbsoluteSpawnPosition
            {
                get
                {
                    return this.absoluteSpawnPosition;
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
