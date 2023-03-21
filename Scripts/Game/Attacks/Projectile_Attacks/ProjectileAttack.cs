using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class ProjectileAttack : Attack
    {
        private static SceneVariable<Dictionary<Projectile, RecyclePool<Projectile>>> projectilePools;

        [SerializeField]
        private IntSerializedReference numberSpawns;
        [SerializeField]
        private FloatSerializedReference durationBetweenSpawns;
        [SerializeField]
        private Targeter baseSpawnTargeter;

        static ProjectileAttack()
        {
            projectilePools = new SceneVariable<Dictionary<Projectile, RecyclePool<Projectile>>>(() => new Dictionary<Projectile, RecyclePool<Projectile>>());
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

                    foreach (ProjectileSpawnParameters projectileSpawnParameters in GetProjectileSpawnParameters(baseSpawnPosition, elapsedDuration))
                    {
                        if (!TilePositioning.TilePositionWithinBounds(projectileSpawnParameters.SpawnPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions))
                        {
                            continue;
                        }

                        if (!projectilePools.Current.ContainsKey(projectileSpawnParameters.ProjectilePrefab))
                        {
                            projectilePools.Current.Add(
                                projectileSpawnParameters.ProjectilePrefab,
                                new RecyclePool<Projectile>(
                                    () => FrigidInstancing.CreateInstance<Projectile>(projectileSpawnParameters.ProjectilePrefab),
                                    (Projectile projectile) => FrigidInstancing.DestroyInstance(projectile)
                                    )
                                );
                        }
                        RecyclePool<Projectile> projectilePool = projectilePools.Current[projectileSpawnParameters.ProjectilePrefab];
                        Projectile spawnedProjectile = projectilePool.Retrieve();
                        spawnedProjectile.LaunchProjectile(
                            Mathf.RoundToInt(this.DamageBonus * projectileSpawnParameters.DamageBonusMultiplier),
                            this.DamageAlignment,
                            () => projectilePool.Pool(spawnedProjectile),
                            projectileSpawnParameters.SpawnPosition,
                            projectileSpawnParameters.LaunchDirection,
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

        protected abstract List<ProjectileSpawnParameters> GetProjectileSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration);

        protected struct ProjectileSpawnParameters
        {
            private float damageBonusMultiplier;
            private Vector2 spawnPosition;
            private Vector2 launchDirection;
            private Projectile projectilePrefab;

            public ProjectileSpawnParameters(float damageBonusMultiplier, Vector2 spawnPosition, Vector2 launchDirection, Projectile projectilePrefab)
            {
                this.damageBonusMultiplier = damageBonusMultiplier;
                this.spawnPosition = spawnPosition;
                this.launchDirection = launchDirection;
                this.projectilePrefab = projectilePrefab;
            }

            public float DamageBonusMultiplier
            {
                get
                {
                    return this.damageBonusMultiplier;
                }
            }

            public Vector2 SpawnPosition
            {
                get
                {
                    return this.spawnPosition;
                }
            }

            public Vector2 LaunchDirection
            {
                get
                {
                    return this.launchDirection;
                }
            }

            public Projectile ProjectilePrefab
            {
                get
                {
                    return this.projectilePrefab;
                }
            }
        }
    }
}
