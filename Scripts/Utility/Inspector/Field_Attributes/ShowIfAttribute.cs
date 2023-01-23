using System;

namespace FrigidBlackwaters.Utility
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ShowIfAttribute : InspectorFieldAttribute 
    {
        private bool evaluation;
        private bool checkEnclosedObject;

        public ShowIfAttribute(bool evaluation, bool checkEnclosedObject = false)
        {
            this.evaluation = evaluation;
            this.checkEnclosedObject = checkEnclosedObject;
        }

        public bool Evaluation
        {
            get
            {
                return this.evaluation;
            }
        }

        public bool CheckEnclosedObject
        {
            get
            {
                return this.checkEnclosedObject;
            }
        }
    }
}
