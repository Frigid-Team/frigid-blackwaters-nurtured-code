using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class LineExplosionAttack : ExplosionAttack
    {
        [SerializeField]
        private Explosion explosionPrefab;
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private IntSerializedReference numberLines;
        [Space]
        [SerializeField]
        private bool useOffsetSpeed;
        [SerializeField]
        [ShowIfBool("useOffsetSpeed", true)]
        private FloatSerializedReference offsetSpeed;
        [SerializeField]
        [ShowIfBool("useOffsetSpeed", false)]
        private FloatSerializedReference offsetLength;
        [SerializeField]
        private FloatSerializedReference lineLength;
        [SerializeField]
        private IntSerializedReference numberExplosions;
        [SerializeField]
        private FloatSerializedReference durationBetween;
        [SerializeField]
        private Direction lineDirection;

        protected override List<ExplosionSpawnParameters> GetExplosionSpawnParameters(TiledArea tiledArea, float elapsedDuration)
        {
            List<ExplosionSpawnParameters> explosionSpawnParameters = new List<ExplosionSpawnParameters>();

            int currNumberLines = this.numberLines.MutableValue;
            Vector2[] originPositions = this.originTargeter.Retrieve(new Vector2[currNumberLines], elapsedDuration, 0);
            foreach (Vector2 originPosition in originPositions)
            {
                float offsetLength = this.useOffsetSpeed ? (this.offsetSpeed.MutableValue * elapsedDuration) : this.offsetLength.MutableValue;
                int currentNumberExplosions = this.numberExplosions.MutableValue;
                float lengthBetweenExplosions = this.lineLength.MutableValue / Mathf.Clamp(currentNumberExplosions - 1, 1, int.MaxValue);
                Vector2 lineDirection = this.lineDirection.Retrieve(Vector2.zero, elapsedDuration, 0);

                float delayDuration = 0;
                for (int i = 0; i < currentNumberExplosions; i++)
                {
                    explosionSpawnParameters.Add(
                        new ExplosionSpawnParameters(
                            originPosition + lineDirection * (lengthBetweenExplosions * i + offsetLength),
                            lineDirection.ComponentAngle0To360(),
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
