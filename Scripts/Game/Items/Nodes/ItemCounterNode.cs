using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FrigidBlackwaters.Game
{
    public class ItemCounterNode : ItemNode
    {
        [SerializeField]
        private CallbackChannel countUpCallbacks;
        [SerializeField]
        private CallbackChannel countDownCallbacks;
        [SerializeField]
        private List<CounterChildBranch> counterChildBranches;
        [SerializeField]
        private bool absolute;
        [SerializeField]
        private int startingCount;

        private int currentCount;

        public override void Init()
        {
            base.Init();
            this.currentCount = this.startingCount;
        }

        public override void Enter()
        {
            base.Enter();
            this.countUpCallbacks.RegisterListener(CountUp);
            this.countDownCallbacks.RegisterListener(CountDown);
        }

        public override void Exit()
        {
            base.Exit();
            this.countUpCallbacks.ClearListener(CountUp);
            this.countDownCallbacks.ClearListener(CountDown);
        }

        protected override List<ChildBranch> AdditionalChildBranches
        {
            get
            {
                return this.counterChildBranches.ToList<ChildBranch>();
            }
        }

        private void CountUp() => ChangeCount(1);

        private void CountDown() => ChangeCount(-1);

        private void ChangeCount(int delta)
        {
            this.currentCount += delta;
            if (this.absolute) this.currentCount = Mathf.Max(0, this.currentCount);
            UpdateCurrentChildBranches();
        }

        [Serializable]
        private class CounterChildBranch : ChildBranch
        {
            [SerializeField]
            private int threshold;
            [SerializeField]
            private int countDeltaOnEnter;
            [SerializeField]
            private int countDeltaOnExit;

            public override void BranchEntered(ItemNode node)
            {
                base.BranchEntered(node);
                ((ItemCounterNode)node).ChangeCount(this.countDeltaOnEnter);
            }

            public override void BranchExited(ItemNode node)
            {
                base.BranchExited(node);
                ((ItemCounterNode)node).ChangeCount(this.countDeltaOnExit);
            }

            public override bool CanBranch(ItemNode node)
            {
                return base.CanBranch(node) && ((ItemCounterNode)node).currentCount >= this.threshold;
            }
        }
    }
}