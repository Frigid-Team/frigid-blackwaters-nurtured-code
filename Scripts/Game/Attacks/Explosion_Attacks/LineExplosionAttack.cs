using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class LineExplosionAttack : ExplosionAttack
    {
        [SerializeField]
        private Explosion explosionPrefab;
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private Targeter destinationTargeter;
        [SerializeField]
        private IntSerializedReference numberLines;
        [Space]
        [SerializeField]
        private FloatSerializedReference interval;
        [SerializeField]
        private FloatSerializedReference durationBetween;

        public override List<ExplosionSpawnSetting> GetSpawnSettings(TiledArea tiledArea, float elapsedDuration)
        {
            List<ExplosionSpawnSetting> spawnSettings = new List<ExplosionSpawnSetting>();

            int currNumberLines = this.numberLines.MutableValue;
            Vector2[] originPositions = this.originTargeter.Retrieve(new Vector2[currNumberLines], elapsedDuration, 0f);
            Vector2[] destinationPositions = this.destinationTargeter.Retrieve(originPositions, elapsedDuration, 0f);
            for (int i = 0; i < currNumberLines; i++)
            {
                Vector2 originPosition = originPositions[i];
                Vector2 destinationPosition = destinationPositions[i];
                Vector2 displacement = destinationPosition - originPosition;
                Vector2 direction = displacement.normalized;
                float interval = this.interval.MutableValue;
                float distance = displacement.magnitude;

                float delayDuration = 0;
                for (float d = distance % interval; d <= distance; d += interval)
                {
                    Vector2 spawnPosition = originPosition + direction * d;

                    spawnSettings.Add(new ExplosionSpawnSetting(spawnPosition, delayDuration, this.explosionPrefab, direction.ComponentAngle0To360()));
                    delayDuration += this.durationBetween.MutableValue;
                }
            }

            return spawnSettings;
        }
    }
}
