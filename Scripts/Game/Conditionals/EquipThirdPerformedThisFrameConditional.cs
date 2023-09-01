using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class EquipThirdPerformedThisFrameConditional : Conditional
    {
        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return CharacterInput.EquipThirdPerformedThisFrame;
        }
    }
}
