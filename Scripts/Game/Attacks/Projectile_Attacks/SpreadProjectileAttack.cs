using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SpreadProjectileAttack : ProjectileAttack
    {
        [SerializeField]
        private Projectile projectilePrefab;
        [SerializeField]
        private IntSerializedReference numberProjectiles;
        [SerializeField]
        private FloatSerializedReference angleOffsetDegrees;
        [SerializeField]
        private FloatSerializedReference distanceFromSpawnPosition;

        protected override List<ProjectileSpawnParameters> GetProjectileSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration)
        {
            List<ProjectileSpawnParameters> spawnParameters = new List<ProjectileSpawnParameters>();
            int currNumberProjectiles = this.numberProjectiles.MutableValue;
            float angleBetweenRad = Mathf.PI * 2 / currNumberProjectiles;
            float angleOffsetRad = this.angleOffsetDegrees.MutableValue * Mathf.Deg2Rad;
            for (int i = 0; i < currNumberProjectiles; i++)
            {
                float launchAngleRad = angleBetweenRad * i + angleOffsetRad;
                Vector2 launchDirection = new Vector2(Mathf.Cos(launchAngleRad), Mathf.Sin(launchAngleRad));
                spawnParameters.Add(new ProjectileSpawnParameters(baseSpawnPosition + launchDirection * this.distanceFromSpawnPosition.ImmutableValue, launchDirection, this.projectilePrefab));
            }
            return spawnParameters;
        }
    }
}
