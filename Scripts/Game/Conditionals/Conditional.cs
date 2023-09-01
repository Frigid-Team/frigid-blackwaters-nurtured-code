using UnityEngine;
using System;

namespace FrigidBlackwaters.Game
{
    public abstract class Conditional : FrigidMonoBehaviour
    {
        [SerializeField]
        private bool validateOnFalse;

        public bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            bool customEvaluation = this.CustomEvaluate(elapsedDuration, elapsedDurationDelta);
            return customEvaluation && !this.validateOnFalse || !customEvaluation && this.validateOnFalse;
        }

        public int Tally(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.CustomTally(elapsedDuration, elapsedDurationDelta);
        }

        protected abstract bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta);

        protected virtual int CustomTally(float elapsedDuration, float elapsedDurationDelta)
        {
            return Convert.ToInt32(this.Evaluate(elapsedDuration, elapsedDurationDelta));
        }
    }
}
