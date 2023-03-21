using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class ConditionalClause
    {
        [SerializeField]
        private List<Conjunction> conjunctions;

        public bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            bool valid = false;
            foreach (Conjunction conjunction in this.conjunctions)
            {
                valid |= conjunction.Evaluate(elapsedDuration, elapsedDurationDelta);
            }
            return valid;
        }

        [Serializable]
        private struct Conjunction
        {
            [SerializeField]
            private List<Conditional> conditionals;

            public bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
            {
                bool valid = true;
                foreach (Conditional conditional in this.conditionals)
                {
                    bool evaluation = conditional.Evaluate(elapsedDuration, elapsedDurationDelta);
                    valid &= (conditional.ValidateOnFalse && !evaluation) || (!conditional.ValidateOnFalse && evaluation);
                }
                return valid;
            }
        }
    }
}
