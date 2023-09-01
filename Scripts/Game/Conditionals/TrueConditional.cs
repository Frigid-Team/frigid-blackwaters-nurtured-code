namespace FrigidBlackwaters.Game
{
    public class TrueConditional : Conditional
    {
        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return true;
        }
    }
}
