using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DockInputHeldConditional : Conditional
    {
        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.DockHeld;
        }
    }
}
