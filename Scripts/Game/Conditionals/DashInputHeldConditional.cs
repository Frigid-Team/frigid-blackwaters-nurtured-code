using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DashInputHeldConditional : Conditional
    {
        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.DashHeld;
        }
    }
}
