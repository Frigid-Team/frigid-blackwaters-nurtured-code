using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DashInputHeldConditional : Conditional
    {
        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.DashHeld;
        }
    }
}
