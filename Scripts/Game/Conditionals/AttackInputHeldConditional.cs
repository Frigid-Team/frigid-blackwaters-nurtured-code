using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AttackInputHeldConditional : Conditional
    {
        protected override bool CustomValidate()
        {
            return CharacterInput.AttackHeld;
        }
    }
}
