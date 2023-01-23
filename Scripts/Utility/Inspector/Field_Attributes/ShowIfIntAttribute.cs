namespace FrigidBlackwaters.Utility
{
    public class ShowIfIntAttribute : ShowIfAttribute
    {
        private string intPropertyPath;
        private int minValue;
        private int maxValue;

        public ShowIfIntAttribute(string intPropertyPath, int value, bool evaluation, bool checkEnclosedObject = false) : this(intPropertyPath, value, value, evaluation, checkEnclosedObject) { }

        public ShowIfIntAttribute(string intPropertyPath, int minValue, int maxValue, bool evaluation, bool checkEnclosedObject = false) : base(evaluation, checkEnclosedObject)
        {
            this.intPropertyPath = intPropertyPath;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public string IntPropertyPath
        {
            get
            {
                return this.intPropertyPath;
            }
        }

        public int MinValue
        {
            get
            {
                return this.minValue;
            }
        }

        public int MaxValue
        {
            get
            {
                return this.maxValue;
            }
        }
    }
}
