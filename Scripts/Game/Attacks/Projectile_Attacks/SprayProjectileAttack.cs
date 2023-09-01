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
        private Targeter originTargeter;
        [SerializeField]
        private IntSerializedReference numberSprays;
        [Space]
        [SerializeField]
        private IntSerializedReference numberProjectiles;
        [SerializeField]
        private FloatSerializedReference angleBetween;
        [SerializeField]
        private FloatSerializedReference distanceFromSpawnPosition;
        [SerializeField]
        private FloatSerializedReference durationBetween;
        [SerializeField]
        private bool revolveClockwise;
        [SerializeField]
        private Direction summonDirection;
        [SerializeField]
        private Direction launchDirection;

        protected override List<ProjectileSpawnParameters> GetProjectileSpawnParameters(TiledArea tiledArea, float elapsedDuration)
        {
            List<ProjectileSpawnParameters> spawnParameters = new List<ProjectileSpawnParameters>();

            int currNumberSprays = this.numberSprays.MutableValue;
            Vector2[] originPositions = this.originTargeter.Retrieve(new Vector2[currNumberSprays], elapsedDuration, 0);
            foreach (Vector2 originPosition in originPositions)
            {
                int currNumberProjectiles = this.numberProjectiles.MutableValue;
                float currAngleBetweenProjectilesDegrees = this.angleBetween.MutableValue;

                float baseAngleDeg = this.launchDirection.Retrieve(Vector2.zero, elapsedDuration, 0).ComponentAngle0To360() + (currNumberProjectiles % 2 == 0 ? currAngleBetweenProjectilesDegrees / 2 : 0);
                float delayDuration = 0;
                for (int i = 0; i < currNumberProjectiles; i++)
                {
                    float projectileAngleDeg = baseAngleDeg + (this.revolveClockwise ? (currNumberProjectiles / 2 * currAngleBetweenProjectilesDegrees - i * currAngleBetweenProjectilesDegrees) : (-currNumberProjectiles / 2 * currAngleBetweenProjectilesDegrees + i * currAngleBetweenProjectilesDegrees));
                    float projectileAngleRad = projectileAngleDeg * Mathf.Deg2Rad;
                    Vector2 projectileSpawnDirection = new Vector2(Mathf.Cos(projectileAngleRad), Mathf.Sin(projectileAngleRad));
                    float summonAngleRad = this.summonDirection.Retrieve(Vector2.zero, elapsedDuration, 0).ComponentAngle0To360() * Mathf.Deg2Rad;
                    Vector2 projectileSpawnPosition = originPosition + new Vector2(Mathf.Cos(summonAngleRad), Mathf.Sin(summonAngleRad)) * this.distanceFromSpawnPosition.ImmutableValue;
                    spawnParameters.Add(new ProjectileSpawnParameters(projectileSpawnPosition, projectileSpawnDirection, delayDuration, this.projectilePrefab));
                    delayDuration += this.durationBetween.MutableValue;
                }
            }

            return spawnParameters;
        }
    }
}
