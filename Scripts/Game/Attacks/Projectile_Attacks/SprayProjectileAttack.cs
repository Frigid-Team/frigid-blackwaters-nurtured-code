using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SprayProjectileAttack : ProjectileAttack
    {
        [SerializeField]
        private Projectile projectilePrefab;
        [SerializeField]
        private IntSerializedReference numberProjectiles;
        [SerializeField]
        private FloatSerializedReference angleBetweenProjectilesDegrees;
        [SerializeField]
        private FloatSerializedReference distanceFromSpawnPosition;
        [SerializeField]
        private Direction summonDirection;
        [SerializeField]
        private Direction launchDirection;

        protected override List<ProjectileSpawnParameters> GetProjectileSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration)
        {
            List<ProjectileSpawnParameters> spawnParameters = new List<ProjectileSpawnParameters>();

            int currNumberProjectiles = this.numberProjectiles.MutableValue;
            float currAngleBetweenProjectilesDegrees = this.angleBetweenProjectilesDegrees.MutableValue;

            float baseAngleDeg = this.launchDirection.Calculate(Vector2.zero, elapsedDuration, 0).ComponentAngle0To360() * Mathf.Rad2Deg + (currNumberProjectiles % 2 == 0 ? currAngleBetweenProjectilesDegrees/ 2 : 0);
            for (int i = 0; i < currNumberProjectiles; i++)
            {
                float projectileAngleDeg = baseAngleDeg - currNumberProjectiles / 2 * currAngleBetweenProjectilesDegrees + i * currAngleBetweenProjectilesDegrees;
                float projectileAngleRad = projectileAngleDeg * Mathf.Deg2Rad;
                Vector2 projectileSpawnDirection = new Vector2(Mathf.Cos(projectileAngleRad), Mathf.Sin(projectileAngleRad));
                float summonAngleRad = this.summonDirection.Calculate(Vector2.zero, elapsedDuration, 0).ComponentAngle0To360();
                Vector2 projectileSpawnPosition = baseSpawnPosition + new Vector2(Mathf.Cos(summonAngleRad), Mathf.Sin(summonAngleRad)) * this.distanceFromSpawnPosition.ImmutableValue;

                spawnParameters.Add(new ProjectileSpawnParameters(projectileSpawnPosition, projectileSpawnDirection, this.projectilePrefab));
            }

            return spawnParameters;
        }
    }
}
