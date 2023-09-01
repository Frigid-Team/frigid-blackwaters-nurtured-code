using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ExpressionConditional : Conditional
    {
        [SerializeField]
        private List<Conjunction> conjunctions;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            bool evaluation = false;
            foreach (Conjunction conjunction in this.conjunctions)
            {
                evaluation |= conjunction.Evaluate(elapsedDuration, elapsedDurationDelta);
            }
            return evaluation;
        }

        [Serializable]
        private class Conjunction
        {
            [SerializeField]
            private List<Conditional> conditions;

            public bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
            {
                bool evaluation = true;
                foreach (Conditional condition in this.conditions)
                {
                    evaluation &= condition.Evaluate(elapsedDuration, elapsedDurationDelta);
                }
                return evaluation;
            }
        }
    }
}
