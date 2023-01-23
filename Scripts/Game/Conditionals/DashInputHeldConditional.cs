using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DashInputHeldConditional : Conditional
    {
        protected override bool CustomValidate()
        {
            return CharacterInput.DashHeld;
        }
    }
}
