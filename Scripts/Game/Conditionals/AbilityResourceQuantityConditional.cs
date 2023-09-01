using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class AbilityResourceQuantityConditional : IntComparisonConditional
    {
        [SerializeField]
        private AbilityResource abilityResource;

        protected override int GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.abilityResource.Quantity;
        }
    }
}
