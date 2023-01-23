using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DockInputHeldConditional : Conditional
    {
        protected override bool CustomValidate()
        {
            return CharacterInput.DockHeld;
        }
    }
}
