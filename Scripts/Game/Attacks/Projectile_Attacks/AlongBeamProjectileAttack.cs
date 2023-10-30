using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AlongBeamProjectileAttack : ProjectileAttack
    {
        [SerializeField]
        private Projectile projectilePrefab;
        [SerializeField]
        private Beam beam;
        [SerializeField]
        private IntSerializedReference numberTimesAlongBeam;
        [Space]
        [SerializeField]
        private FloatSerializedReference interval;
        [SerializeField]
        private FloatSerializedReference launchAngleOffset;
        [SerializeField]
        private FloatSerializedReference durationBetween;

        public override List<ProjectileSpawnSetting> GetSpawnSettings(TiledArea tiledArea, float elapsedDuration)
        {
            List<ProjectileSpawnSetting> spawnSettings = new List<ProjectileSpawnSetting>();

            Vector2[] beamPositions = this.beam.Positions;
            float beamDistance = 0f;
            for (int j = 1; j < beamPositions.Length; j++)
            {
                beamDistance += Vector2.Distance(beamPositions[j], beamPositions[j - 1]);
            }

            int currNumberTimesAlongBeam = this.numberTimesAlongBeam.MutableValue;
            for (int i = 0; i < currNumberTimesAlongBeam; i++)
            {
                float interval = this.interval.MutableValue;

                float d = beamDistance % interval;
                float delayDuration = 0f;
                for (int j = 1; j < beamPositions.Length; j++)
                {
                    Vector2 startPosition = beamPositions[j - 1];
                    Vector2 endPosition = beamPositions[j];
                    Vector2 displacement = endPosition - startPosition;
                    Vector2 direction = displacement.normalized;
                    float distance = displacement.magnitude;
                    for (; d <= distance; d += interval)
                    {
                        Vector2 spawnPosition = startPosition + direction * d;

                        spawnSettings.Add(new ProjectileSpawnSetting(spawnPosition, delayDuration, this.projectilePrefab, direction.RotateAround(this.launchAngleOffset.MutableValue)));
                        delayDuration += this.durationBetween.MutableValue;
                    }
                    d %= interval;
                }
            }

            return spawnSettings;
        }
    }
}
