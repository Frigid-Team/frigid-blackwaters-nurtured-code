using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class ProjectileAttack : AttackBodyAttack<ProjectileAttack.ProjectileSpawnSetting, Projectile>
    {
        public class ProjectileSpawnSetting : SpawnSetting
        {
            private Func<float, float, Vector2, Vector2, Vector2> toGetLaunchDirection;

            public ProjectileSpawnSetting(Vector2 spawnPosition, float delayDuration, Projectile projectilePrefab, Vector2 launchDirection) : this(spawnPosition, delayDuration, projectilePrefab, (float travelDuration, float travelDurationDelta, Vector2 position, Vector2 velocity) => launchDirection) { }

            public ProjectileSpawnSetting(Vector2 spawnPosition, float delayDuration, Projectile projectilePrefab, Func<float, float, Vector2, Vector2, Vector2> toGetLaunchDirection) : base(spawnPosition, delayDuration, projectilePrefab)
            {
                this.toGetLaunchDirection = toGetLaunchDirection;
                this.OnBodyInitialized += (Projectile projectile) => projectile.ToGetLaunchDirection = toGetLaunchDirection;
            }

            public Func<float, float, Vector2, Vector2, Vector2> ToGetLaunchDirection
            {
                get
                {
                    return this.toGetLaunchDirection;
                }
            }
        }
    }
}
