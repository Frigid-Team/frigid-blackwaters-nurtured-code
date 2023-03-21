using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Conditional : FrigidMonoBehaviour
    {
        [SerializeField]
        private bool validateOnFalse;

        public bool ValidateOnFalse
        {
            get
            {
                return this.validateOnFalse;
            }
        }

        public abstract bool Evaluate(float elapsedDuration, float elapsedDurationDelta);
    }
}
