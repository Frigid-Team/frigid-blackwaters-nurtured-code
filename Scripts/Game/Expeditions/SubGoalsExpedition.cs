using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class SubGoalsExpedition<G> : Expedition where G : SubGoalsExpedition<G>.SubGoal
    {
        [SerializeField]
        private List<G> subGoals;

        private bool completed;
        private Action onComplete;

        protected override void Departed(Action onComplete)
        {
            this.completed = false;
            this.onComplete = onComplete;
            foreach (G subGoal in this.subGoals)
            {
                subGoal.Start(this.CheckCompletion);
            }
        }

        protected override void Returned()
        {
            this.onComplete = null;
            foreach (G subGoal in this.subGoals)
            {
                subGoal.End();
            }
        }

        private void CheckCompletion()
        {
            if (this.completed) return;

            bool allComplete = true;
            foreach (G subGoal in this.subGoals)
            {
                allComplete &= subGoal.IsComplete;
            }
            if (allComplete)
            {
                this.completed = true;
                this.onComplete?.Invoke();
            }
        }

        public abstract class SubGoal
        {
            public abstract bool IsComplete { get; }

            public virtual void Start(Action onComplete) { }

            public virtual void End() { }
        }
    }
}
