using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class EquipSecondPerformedThisFrameConditional : Conditional
    {
        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.EquipSecondPerformedThisFrame;
        }
    }
}
