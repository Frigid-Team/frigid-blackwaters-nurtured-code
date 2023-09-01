using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class AbilityResourceProgressConditional : FloatComparisonConditional
    {
        [SerializeField]
        private AbilityResource abilityResource;

        protected override float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.abilityResource.Progress;
        }
    }
}
