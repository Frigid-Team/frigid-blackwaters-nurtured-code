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

        public bool Evaluate()
        {
            bool valid = false;
            foreach (Conjunction conjunction in this.conjunctions)
            {
                if (conjunction.Evaluate())
                {
                    valid = true;
                    break;
                }
            }

            return valid || this.conjunctions.Count == 0;
        }

        [Serializable]
        private struct Conjunction
        {
            [SerializeField]
            private List<Conditional> conditionals;

            public bool Evaluate()
            {
                bool valid = true;
                foreach (Conditional conditional in this.conditionals)
                {
                    if (!conditional.Validate())
                    {
                        valid = false;
                    }
                }
                return valid;
            }
        }
    }
}
