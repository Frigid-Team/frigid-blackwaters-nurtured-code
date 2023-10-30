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
        private Direction launchDirection;

        public override List<ProjectileSpawnSetting> GetSpawnSettings(TiledArea tiledArea, float elapsedDuration)
        {
            List<ProjectileSpawnSetting> spawnSettings = new List<ProjectileSpawnSetting>();

            int currNumberSprays = this.numberSprays.MutableValue;
            Vector2[] originPositions = this.originTargeter.Retrieve(new Vector2[currNumberSprays], elapsedDuration, 0);
            foreach (Vector2 originPosition in originPositions)
            {
                int currNumberProjectiles = this.numberProjectiles.MutableValue;
                float currAngleBetweenProjectiles = this.angleBetween.MutableValue;
                float baseAngle = this.launchDirection.Retrieve(Vector2.zero, elapsedDuration, 0).ComponentAngle0To360() + (currNumberProjectiles % 2 == 0 ? currAngleBetweenProjectiles / 2 : 0);

                float delayDuration = 0;
                for (int i = 0; i < currNumberProjectiles; i++)
                {
                    float projectileAngle = baseAngle + (this.revolveClockwise ? (currNumberProjectiles / 2 * currAngleBetweenProjectiles - i * currAngleBetweenProjectiles) : (-currNumberProjectiles / 2 * currAngleBetweenProjectiles + i * currAngleBetweenProjectiles));
                    float projectileAngleRad = projectileAngle * Mathf.Deg2Rad;
                    Vector2 launchDirection = new Vector2(Mathf.Cos(projectileAngleRad), Mathf.Sin(projectileAngleRad));
                    Vector2 spawnPosition = originPosition + launchDirection * this.distanceFromSpawnPosition.ImmutableValue;

                    spawnSettings.Add(new ProjectileSpawnSetting(spawnPosition, delayDuration, this.projectilePrefab, launchDirection));
                    delayDuration += this.durationBetween.MutableValue;
                }
            }

            return spawnSettings;
        }
    }
}
