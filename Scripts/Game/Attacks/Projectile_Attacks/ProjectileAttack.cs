using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class ProjectileAttack : Attack
    {
        private static SceneVariable<Dictionary<Projectile, RecyclePool<Projectile>>> projectilePools;

        static ProjectileAttack()
        {
            projectilePools = new SceneVariable<Dictionary<Projectile, RecyclePool<Projectile>>>(() => new Dictionary<Projectile, RecyclePool<Projectile>>());
        }

        public override void Perform(float elapsedDuration, Action onComplete)
        {
            if (!TiledArea.TryGetAreaAtPosition(this.transform.position, out TiledArea tiledArea))
            {
                return;
            } 

            foreach (ProjectileSpawnParameters projectileSpawnParameters in this.GetProjectileSpawnParameters(tiledArea, elapsedDuration))
            {
                if (!AreaTiling.TilePositionWithinBounds(projectileSpawnParameters.SpawnPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions))
                {
                    continue;
                }

                FrigidCoroutine.Run(
                    Tween.Delay(
                        projectileSpawnParameters.DelayDuration,
                        () =>
                        {
                            if (!projectilePools.Current.ContainsKey(projectileSpawnParameters.ProjectilePrefab))
                            {
                                projectilePools.Current.Add(
                                    projectileSpawnParameters.ProjectilePrefab,
                                    new RecyclePool<Projectile>(
                                        () => CreateInstance<Projectile>(projectileSpawnParameters.ProjectilePrefab),
                                        (Projectile projectile) => DestroyInstance(projectile)
                                        )
                                    );
                            }
                            RecyclePool<Projectile> projectilePool = projectilePools.Current[projectileSpawnParameters.ProjectilePrefab];
                            Projectile spawnedProjectile = projectilePool.Retrieve();
                            spawnedProjectile.LaunchProjectile(
                                this.DamageBonus,
                                this.DamageAlignment,
                                () => projectilePool.Pool(spawnedProjectile),
                                projectileSpawnParameters.SpawnPosition,
                                projectileSpawnParameters.LaunchDirection,
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

        protected abstract List<ProjectileSpawnParameters> GetProjectileSpawnParameters(TiledArea tiledArea, float elapsedDuration);

        protected struct ProjectileSpawnParameters
        {
            private Vector2 spawnPosition;
            private Vector2 launchDirection;
            private float delayDuration;
            private Projectile projectilePrefab;

            public ProjectileSpawnParameters(Vector2 spawnPosition, Vector2 launchDirection, float delayDuration, Projectile projectilePrefab)
            {
                this.spawnPosition = spawnPosition;
                this.launchDirection = launchDirection;
                this.delayDuration = delayDuration;
                this.projectilePrefab = projectilePrefab;
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

            public float DelayDuration
            {
                get
                {
                    return this.delayDuration;
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
