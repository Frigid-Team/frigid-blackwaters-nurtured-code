using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Conditional : FrigidMonoBehaviour
    {
        [SerializeField]
        private bool validateOnFalse;

        public bool Validate()
        {
            bool evaluation = CustomValidate();
            return evaluation && !this.validateOnFalse || !evaluation && this.validateOnFalse;
        }

        protected abstract bool CustomValidate();
    }
}
