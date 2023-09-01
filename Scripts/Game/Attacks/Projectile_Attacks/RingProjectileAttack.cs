using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RingProjectileAttack : ProjectileAttack
    {
        [SerializeField]
        private Projectile projectilePrefab;
        [SerializeField]
        private Targeter centerTargeter;
        [SerializeField]
        private IntSerializedReference numberRings;
        [Space]
        [SerializeField]
        private IntSerializedReference numberProjectiles;
        [SerializeField]
        private FloatSerializedReference startingAngle;
        [SerializeField]
        private FloatSerializedReference distanceFromSpawnPosition;
        [SerializeField]
        private FloatSerializedReference durationBetween;
        [SerializeField]
        private bool revolveClockwise;

        protected override List<ProjectileSpawnParameters> GetProjectileSpawnParameters(TiledArea tiledArea, float elapsedDuration)
        {
            List<ProjectileSpawnParameters> spawnParameters = new List<ProjectileSpawnParameters>();

            int currNumberRings = this.numberRings.MutableValue;
            Vector2[] centerPositions = this.centerTargeter.Retrieve(new Vector2[currNumberRings], elapsedDuration, 0);
            foreach (Vector2 centerPosition in centerPositions)
            {
                int currNumberProjectiles = this.numberProjectiles.MutableValue;
                float angleBetweenRad = Mathf.PI * 2 / currNumberProjectiles;
                float angleOffsetRad = this.startingAngle.MutableValue * Mathf.Deg2Rad;

                float delayDuration = 0;
                for (int i = 0; i < currNumberProjectiles; i++)
                {
                    float launchAngleRad = angleBetweenRad * i * (this.revolveClockwise ? -1 : 1) + angleOffsetRad;
                    Vector2 launchDirection = new Vector2(Mathf.Cos(launchAngleRad), Mathf.Sin(launchAngleRad));
                    Vector2 projectileSpawnPosition = centerPosition + launchDirection * this.distanceFromSpawnPosition.ImmutableValue;
                    spawnParameters.Add(new ProjectileSpawnParameters(projectileSpawnPosition, launchDirection, delayDuration, this.projectilePrefab));
                    delayDuration += this.durationBetween.MutableValue;
                }
            }

            return spawnParameters;
        }
    }
}
