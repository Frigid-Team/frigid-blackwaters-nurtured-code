using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class OnCooldownConditional : Conditional
    {
        [SerializeField]
        private Cooldown cooldown;

        protected override bool CustomValidate()
        {
            return this.cooldown.OnCooldown();
        }
    }
}
