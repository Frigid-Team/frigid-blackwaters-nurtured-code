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
        private bool useOffsetSpeed;
        [SerializeField]
        private float offsetLength;
        [SerializeField]
        private float offsetSpeed;
        [SerializeField]
        private float lineLength;
        [SerializeField]
        private int numberExplosions;
        [SerializeField]
        private Direction lineDirection;

        protected override List<ExplosionSpawnParameters> GetExplosionSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration)
        {
            List<ExplosionSpawnParameters> explosionSpawnParameters = new List<ExplosionSpawnParameters>();
            float offsetLength = this.useOffsetSpeed ? (this.offsetSpeed * elapsedDuration) : this.offsetLength;
            float lengthBetweenExplosions = this.lineLength / Mathf.Clamp(this.numberExplosions - 1, 1, int.MaxValue);
            Vector2 lineDirection = this.lineDirection.Calculate(Vector2.zero, elapsedDuration, 0);
            for (int i = 0; i < this.numberExplosions; i++)
            {
                explosionSpawnParameters.Add(
                    new ExplosionSpawnParameters(
                        baseSpawnPosition + lineDirection * (lengthBetweenExplosions * i + offsetLength),
                        lineDirection.ComponentAngle0To2PI(),
                        this.explosionPrefab
                        )
                    );
            }
            return explosionSpawnParameters;
        }
    }
}
