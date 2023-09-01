namespace FrigidBlackwaters.Game
{
    public class DurationPassedConditional : FloatComparisonConditional
    {
        protected override float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            return elapsedDuration;
        }
    }
}