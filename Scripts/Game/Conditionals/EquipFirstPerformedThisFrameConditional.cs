using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class EquipFirstPerformedThisFrameConditional : Conditional
    {
        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.EquipFirstPerformedThisFrame;
        }
    }
}
