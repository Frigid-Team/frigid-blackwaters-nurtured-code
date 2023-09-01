using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobStackingBehaviour : MobBehaviour
    {
        [SerializeField]
        private Conditional stackingCondition;
        [SerializeField]
        private Conditional unstackingCondition;
        [SerializeField]
        private IntSerializedReference maxStacks;
        [SerializeField]
        private MobBehaviour childBehaviour;

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

            int numberStackingOccurrences = this.stackingCondition.Tally(this.EnterDuration - this.timeOfPreviousStackOrUnstack, this.EnterDurationDelta);
            for (int i = 0; i < numberStackingOccurrences && this.stackedBehaviours.Count < this.maxStacks.ImmutableValue; i++)
            {
                MobBehaviour newBehaviour = CreateInstance<MobBehaviour>(this.childBehaviour);
                newBehaviour.transform.SetParent(this.Owner.transform);
                newBehaviour.transform.localPosition = Vector3.zero;
                this.Owner.AddBehaviour(newBehaviour, this.Owner.GetIsIgnoringTimeScale(this));
                this.stackedBehaviours.Add(newBehaviour);
                this.timeOfPreviousStackOrUnstack = this.EnterDuration;
            }

            int numberUnStackingOccurrences = this.unstackingCondition.Tally(this.EnterDuration - this.timeOfPreviousStackOrUnstack, this.EnterDurationDelta);
            for (int i = 0; i < numberUnStackingOccurrences && this.stackedBehaviours.Count > 0; i++)
            {
                MobBehaviour removedBehaviour = this.stackedBehaviours[this.stackedBehaviours.Count - 1];
                this.stackedBehaviours.Remove(removedBehaviour);
                this.Owner.RemoveBehaviour(removedBehaviour);
                removedBehaviour.transform.SetParent(this.transform);
                DestroyInstance(removedBehaviour);
                this.timeOfPreviousStackOrUnstack = this.EnterDuration;
            }
        }
    }
}
