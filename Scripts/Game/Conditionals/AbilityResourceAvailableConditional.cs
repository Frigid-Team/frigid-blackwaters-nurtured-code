using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class AbilityResourceAvailableConditional : Conditional
    {
        [SerializeField]
        private AbilityResource abilityResource;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.abilityResource.Available;
        }
    }
}
