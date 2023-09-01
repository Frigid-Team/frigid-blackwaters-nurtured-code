using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AttackInputHeldConditional : Conditional
    {
        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.AttackHeld;
        }
    }
}
