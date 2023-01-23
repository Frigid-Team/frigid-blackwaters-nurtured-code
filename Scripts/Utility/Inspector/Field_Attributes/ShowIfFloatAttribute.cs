namespace FrigidBlackwaters.Utility
{
    public class ShowIfFloatAttribute : ShowIfAttribute
    {
        private string floatPropertyPath;
        private float minValue;
        private float maxValue;

        public ShowIfFloatAttribute(string floatPropertyPath, float value, bool evaluation, bool checkEnclosedObject = false) : this(floatPropertyPath, value, value, evaluation, checkEnclosedObject) { }

        public ShowIfFloatAttribute(string floatPropertyPath, float minValue, float maxValue, bool evaluation, bool checkEnclosedObject = false) : base(evaluation, checkEnclosedObject)
        {
            this.floatPropertyPath = floatPropertyPath;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public string FloatPropertyPath
        {
            get
            {
                return this.floatPropertyPath;
            }
        }

        public float MinValue
        {
            get
            {
                return this.minValue;
            }
        }

        public float MaxValue
        {
            get
            {
                return this.maxValue;
            }
        }
    }
}
