using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RingExplosionAttack : ExplosionAttack
    {
        [SerializeField]
        private Explosion explosionPrefab;
        [SerializeField]
        private Targeter centerTargeter;
        [SerializeField]
        private IntSerializedReference numberRings;
        [Space]
        [SerializeField]
        private FloatSerializedReference ringRadius;
        [SerializeField]
        private IntSerializedReference numberExplosions;
        [SerializeField]
        private FloatSerializedReference durationBetween;
        [SerializeField]
        private FloatSerializedReference startingAngle;
        [SerializeField]
        private bool revolveClockwise;

        protected override List<ExplosionSpawnParameters> GetExplosionSpawnParameters(TiledArea tiledArea, float elapsedDuration)
        {
            List<ExplosionSpawnParameters> explosionSpawnParameters = new List<ExplosionSpawnParameters>();

            int currNumberRings = this.numberRings.MutableValue;
            Vector2[] centerPositions = this.centerTargeter.Retrieve(new Vector2[currNumberRings], elapsedDuration, 0);

            foreach (Vector2 centerPosition in centerPositions)
            {
                int currNumberExplosions = this.numberExplosions.MutableValue;
                float currRingRadius = this.ringRadius.MutableValue;
                float currStartingAngleRad = this.startingAngle.MutableValue * Mathf.Deg2Rad;

                float delayDuration = 0;
                for (int i = 0; i < currNumberExplosions; i++)
                {
                    float angleRad = Mathf.PI * 2 / currNumberExplosions * i * (this.revolveClockwise ? -1f : 1f) + currStartingAngleRad;
                    explosionSpawnParameters.Add(
                        new ExplosionSpawnParameters(
                            centerPosition + new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * currRingRadius,
                            angleRad * Mathf.Rad2Deg,
                            delayDuration,
                            this.explosionPrefab
                            )
                        );
                    delayDuration += this.durationBetween.MutableValue;
                }
            }

            return explosionSpawnParameters;
        }
    }
}
