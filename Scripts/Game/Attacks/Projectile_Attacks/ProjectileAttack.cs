using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class ProjectileAttack : Attack
    {
        private static SceneVariable<Dictionary<Projectile, RecyclePool<Projectile>>> projectilePools;

        [SerializeField]
        private Targeter baseSpawnTargeter;

        static ProjectileAttack()
        {
            projectilePools = new SceneVariable<Dictionary<Projectile, RecyclePool<Projectile>>>(() => { return new Dictionary<Projectile, RecyclePool<Projectile>>(); });
        }

        public override void Perform(float elapsedDuration)
        {
            Vector2 baseSpawnPosition = this.baseSpawnTargeter.Calculate(this.transform.position, elapsedDuration, 0);

            if (!TiledArea.TryGetTiledAreaAtPosition(baseSpawnPosition, out TiledArea tiledArea))
            {
                return;
            }

            foreach (ProjectileSpawnParameters projectileSpawnParameters in GetProjectileSpawnParameters(baseSpawnPosition, elapsedDuration))
            {
                if (!TilePositioning.TileAbsolutePositionWithinBounds(projectileSpawnParameters.AbsoluteSpawnPosition, tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions))
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
                    this.DamageBonus,
                    this.DamageAlignment, 
                    () => projectilePool.Pool(spawnedProjectile),
                    projectileSpawnParameters.AbsoluteSpawnPosition, 
                    projectileSpawnParameters.LaunchDirection,
                    this.OnHitDealt,
                    this.OnBreakDealt,
                    this.OnThreatDealt
                    );
            }
        }

        protected abstract List<ProjectileSpawnParameters> GetProjectileSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration);

        protected struct ProjectileSpawnParameters
        {
            private Vector2 absoluteSpawnPosition;
            private Vector2 launchDirection;
            private Projectile projectilePrefab;

            public ProjectileSpawnParameters(Vector2 absoluteSpawnPosition, Vector2 launchDirection, Projectile projectilePrefab)
            {
                this.absoluteSpawnPosition = absoluteSpawnPosition;
                this.launchDirection = launchDirection;
                this.projectilePrefab = projectilePrefab;
            }

            public Vector2 AbsoluteSpawnPosition
            {
                get
                {
                    return this.absoluteSpawnPosition;
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
