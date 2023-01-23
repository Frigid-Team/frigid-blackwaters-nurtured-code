namespace FrigidBlackwaters.Utility
{
    public class ShowIfPreviouslyShownAttribute : ShowIfAttribute
    {
        public ShowIfPreviouslyShownAttribute(bool evaluation) : base(evaluation, false) { }
    }
}
