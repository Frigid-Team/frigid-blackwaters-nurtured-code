namespace FrigidBlackwaters.Game
{
    public class DurationPassedConditional : MagnitudeConditional
    {
        protected override float GetComparisonValue(float elapsedDuration, float elapsedDurationDelta)
        {
            return elapsedDuration;
        }
    }
}