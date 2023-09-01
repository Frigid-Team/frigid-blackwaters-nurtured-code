using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobTerrainTraversabilityConditional : Conditional
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private TraversableTerrain traversableTerrain;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mob.TryGetValue(out Mob mob) && mob.TraversableTerrain >= this.traversableTerrain;
        }
    }
}
