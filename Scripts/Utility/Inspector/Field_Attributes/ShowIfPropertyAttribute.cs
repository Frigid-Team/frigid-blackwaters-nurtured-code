namespace FrigidBlackwaters.Utility
{
    public class ShowIfPropertyAttribute : ShowIfAttribute
    {
        private string propertyName;

        public ShowIfPropertyAttribute(string propertyName, bool evaluation, bool checkEnclosedObject = false) : base(evaluation, checkEnclosedObject)
        {
            this.propertyName = propertyName;
        }
        
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }
    }
}
