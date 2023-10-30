using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobStateSequence : MobStateNode
    {
        [SerializeField]
        private Step startingStep;
        [SerializeField]
        private List<Step> remainingSteps;
        [SerializeField]
        private bool restartOnEnter;

        private Queue<MobStateNode> stateNodeQueue;

        public override bool AutoEnter
        {
            get
            {
                return this.ChosenStateNode.AutoEnter;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return this.ChosenStateNode.AutoExit && this.stateNodeQueue.Count == 0;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return this.ChosenStateNode.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.ChosenStateNode.ShouldExit;
            }
        }

        public override void Spawn()
        {
            this.stateNodeQueue = new Queue<MobStateNode>();
            this.FillStateNodeQueue();
            base.Spawn();
        }

        public override void Enter()
        {
            if (this.restartOnEnter)
            {
                this.FillStateNodeQueue();
                if (this.stateNodeQueue.Count > 0)
                {
                    if (this.CanSetChosenStateNode(this.stateNodeQueue.Peek()))
                    {
                        this.SetChosenStateNode(this.stateNodeQueue.Dequeue());
                    }
                    else
                    {
                        Debug.LogWarning("MobSequenceStateNode " + this.name + " could not enter its starting node.");
                    }
                }
            }
            base.Enter();
        }

        public override void Refresh()
        {
            base.Refresh();
            if (this.Owner.IsActingAndNotStunned && this.ChosenStateNode.ShouldExit && this.ChosenStateNode.ShouldEnter && (this.ChosenStateNode.AutoExit || this.ChosenStateNode.AutoEnter))
            {
                if (this.stateNodeQueue.Count > 0 && this.CanSetChosenStateNode(this.stateNodeQueue.Peek()))
                {
                    this.SetChosenStateNode(this.stateNodeQueue.Dequeue());
                }
            }
        }

        protected override HashSet<MobStateNode> SpawnStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>() { this.startingStep.StateNode };
            }
        }

        protected override HashSet<MobStateNode> MoveStateNodes
        {
            get
            {
                return new HashSet<MobStateNode> { this.ChosenStateNode };
            }
        }

        protected override HashSet<MobStateNode> ChildStateNodes
        {
            get
            {
                HashSet<MobStateNode> childStateNodes = new HashSet<MobStateNode>() { this.startingStep.StateNode };
                foreach (Step remainingStep in this.remainingSteps)
                {
                    childStateNodes.Add(remainingStep.StateNode);
                }
                return childStateNodes;
            }
        }

        private void FillStateNodeQueue()
        {
            this.stateNodeQueue.Clear();
            int startingStepRepetitions = Mathf.Max(1, 1 + this.startingStep.NumberRepetitionsByReference.MutableValue);
            for (int i = 0; i < startingStepRepetitions; i++)
            {
                this.stateNodeQueue.Enqueue(this.startingStep.StateNode);
            }
            foreach (Step remainingStep in this.remainingSteps)
            {
                int currNumberRepetitions = Mathf.Max(1, 1 + remainingStep.NumberRepetitionsByReference.MutableValue);
                for (int i = 0; i < currNumberRepetitions; i++)
                {
                    this.stateNodeQueue.Enqueue(remainingStep.StateNode);
                }
            }
        }

        [Serializable]
        private struct Step
        {
            [SerializeField]
            private MobStateNode stateNode;
            [SerializeField]
            private IntSerializedReference numberRepetitions;

            public MobStateNode StateNode
            {
                get
                {
                    return this.stateNode;
                }
            }

            public IntSerializedReference NumberRepetitionsByReference
            {
                get
                {
                    return this.numberRepetitions;
                }
            }
        }
    }
}
