namespace FrigidBlackwaters.Utility
{
    public class ShowIfBoolAttribute : ShowIfAttribute
    {
        private string boolPropertyPath;

        public ShowIfBoolAttribute(string boolPropertyPath, bool evaluation, bool checkEnclosedObject = false) : base(evaluation, checkEnclosedObject)
        {
            this.boolPropertyPath = boolPropertyPath;
        }

        public string BoolPropertyPath
        {
            get
            {
                return this.boolPropertyPath;
            }
        }
    }
}
