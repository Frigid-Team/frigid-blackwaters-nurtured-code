using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class CooldownProgressConditional : MagnitudeConditional
    {
        [SerializeField]
        private Cooldown cooldown;

        protected override float GetComparisonValue()
        {
            return this.cooldown.GetProgress();
        }
    }
}
