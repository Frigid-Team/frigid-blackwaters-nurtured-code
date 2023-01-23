namespace FrigidBlackwaters.Utility
{
    public class ShowIfMethodAttribute : ShowIfAttribute
    {
        private string methodName;

        public ShowIfMethodAttribute(string methodName, bool evaluation, bool checkEnclosedObject = false) : base(evaluation, checkEnclosedObject) 
        {
            this.methodName = methodName;
        }
        
        public string MethodName
        {
            get
            {
                return this.methodName;
            }
        }
    }
}
