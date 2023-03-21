using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobStackingBehaviour : MobBehaviour
    {
        [SerializeField]
        private ConditionalClause stackingConditions;
        [SerializeField]
        private ConditionalClause unstackingConditions;
        [SerializeField]
        private IntSerializedReference maxStacks;
        [SerializeField]
        private MobBehaviour behaviourOriginal;

        private List<MobBehaviour> stackedBehaviours;

        private float timeOfPreviousStackOrUnstack;

        public override void Enter()
        {
            base.Enter();
            this.stackedBehaviours = new List<MobBehaviour>();
            this.timeOfPreviousStackOrUnstack = this.EnterDuration;
        }

        public override void Refresh()
        {
            base.Refresh();

            if (this.stackingConditions.Evaluate(this.EnterDuration - this.timeOfPreviousStackOrUnstack, this.EnterDurationDelta) && this.stackedBehaviours.Count < this.maxStacks.ImmutableValue)
            {
                MobBehaviour newBehaviour = FrigidInstancing.CreateInstance<MobBehaviour>(behaviourOriginal);
                newBehaviour.transform.SetParent(this.Owner.transform);
                this.Owner.AddBehaviour(newBehaviour, this.Owner.GetIsIgnoringTimeScale(this));
                this.stackedBehaviours.Add(newBehaviour);
                this.timeOfPreviousStackOrUnstack = this.EnterDuration;
            }

            if (this.unstackingConditions.Evaluate(this.EnterDuration - this.timeOfPreviousStackOrUnstack, this.EnterDurationDelta) && this.stackedBehaviours.Count > 0)
            {
                MobBehaviour removedBehaviour = this.stackedBehaviours[this.stackedBehaviours.Count - 1];
                this.stackedBehaviours.Remove(removedBehaviour);
                this.Owner.RemoveBehaviour(removedBehaviour);
                FrigidInstancing.DestroyInstance(removedBehaviour);
                this.timeOfPreviousStackOrUnstack = this.EnterDuration;
            }
        }
    }
}
