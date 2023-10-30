using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class ExplosionAttack : AttackBodyAttack<ExplosionAttack.ExplosionSpawnSetting, Explosion>
    {
        public class ExplosionSpawnSetting : SpawnSetting
        {
            private float summonAngle;

            public ExplosionSpawnSetting(Vector2 spawnPosition, float delayDuration, Explosion explosionPrefab, float summonAngle) : base(spawnPosition, delayDuration, explosionPrefab)
            {
                this.summonAngle = summonAngle;
                this.OnBodyInitialized += (Explosion explosion) => explosion.SummonAngle = summonAngle;
            }

            public float SummonAngle
            {
                get
                {
                    return this.summonAngle;
                }
            }
        }
    }
}
