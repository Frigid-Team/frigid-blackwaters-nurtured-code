using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class DockInputHeldConditional : Conditional
    {
        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.DockHeld;
        }
    }
}
