using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AttackInputHeldConditional : Conditional
    {
        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.AttackHeld;
        }
    }
}
