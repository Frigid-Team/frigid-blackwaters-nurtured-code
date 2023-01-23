using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class RingExplosionAttack : ExplosionAttack
    {
        [SerializeField]
        private Explosion explosionPrefab;
        [SerializeField]
        private float ringRadius;
        [SerializeField]
        private int numberExplosions;

        protected override List<ExplosionSpawnParameters> GetExplosionSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration)
        {
            List<ExplosionSpawnParameters> explosionSpawnParameters = new List<ExplosionSpawnParameters>();
            for (int i = 0; i < this.numberExplosions; i++)
            {
                float angleRad = Mathf.PI * 2 / this.numberExplosions * i;
                explosionSpawnParameters.Add(
                    new ExplosionSpawnParameters(
                        baseSpawnPosition + new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * this.ringRadius,
                        angleRad * Mathf.Rad2Deg,
                        this.explosionPrefab
                        )
                    );
            }
            return explosionSpawnParameters;
        }
    }
}
