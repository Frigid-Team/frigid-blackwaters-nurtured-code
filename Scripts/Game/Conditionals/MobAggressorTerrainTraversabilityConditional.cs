using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobAggressorTerrainTraversabilityConditional : Conditional
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private TraversableTerrain traversableTerrain;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.ImmutableValue.TryGetAggressor(out Mob aggressor) && aggressor.TraversableTerrain == this.traversableTerrain;
        }
    }
}
