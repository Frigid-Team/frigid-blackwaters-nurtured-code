using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobSequenceStateNode : MobStateNode
    {
        [SerializeField]
        private Step startingStep;
        [SerializeField]
        private List<Step> remainingSteps;
        [SerializeField]
        private bool restartOnEnter;

        private MobStateNode currentStateNode;
        private Queue<MobStateNode> stateNodeQueue;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                return this.startingStep.StateNode.InitialStates;
            }
        }

        public override HashSet<MobState> MoveStates
        {
            get
            {
                return this.currentStateNode.MoveStates;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                HashSet<MobStateNode> referencedStateNodes = new HashSet<MobStateNode>();
                referencedStateNodes.Add(this.startingStep.StateNode);
                foreach (Step remainingStep in this.remainingSteps)
                {
                    referencedStateNodes.Add(remainingStep.StateNode);
                }
                return referencedStateNodes;
            }
        }

        public override bool AutoEnter
        {
            get
            {
                return this.currentStateNode.AutoEnter;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return this.currentStateNode.AutoExit && this.stateNodeQueue.Count == 0;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return this.currentStateNode.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.currentStateNode.ShouldExit;
            }
        }

        public override void Init()
        {
            this.stateNodeQueue = new Queue<MobStateNode>();
            this.FillStateNodeQueue();
            base.Init();
            this.currentStateNode = this.startingStep.StateNode;
        }

        public override void Enter()
        {
            if (this.restartOnEnter)
            {
                this.FillStateNodeQueue();
                if (this.stateNodeQueue.Count > 0)
                {
                    this.SetCurrentStateNode(this.stateNodeQueue.Dequeue());
                }
            }

            base.Enter();
            this.currentStateNode.OnCurrentStateChanged += this.SetCurrentStateFromCurrentStateNode;
            this.currentStateNode.Enter();
            this.SetCurrentStateFromCurrentStateNode();
        }

        public override void Exit()
        {
            base.Exit();
            this.currentStateNode.Exit();
            this.currentStateNode.OnCurrentStateChanged -= this.SetCurrentStateFromCurrentStateNode;
        }

        public override void Refresh()
        {
            base.Refresh();
            if (this.Owner.IsActingAndNotStunned && this.currentStateNode.ShouldExit && this.currentStateNode.ShouldEnter && (this.currentStateNode.AutoExit || this.currentStateNode.AutoEnter))
            {
                if (this.stateNodeQueue.Count > 0 && this.CanSetCurrentStateNode(this.stateNodeQueue.Peek()))
                {
                    this.SetCurrentStateNode(this.stateNodeQueue.Dequeue());
                }
            }
            this.currentStateNode.Refresh();
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

        private bool CanSetCurrentStateNode(MobStateNode chosenStateNode)
        {
            return this.CanSetCurrentState(chosenStateNode.CurrentState);
        }

        private void SetCurrentStateNode(MobStateNode chosenStateNode)
        {
            if (this.CanSetCurrentStateNode(chosenStateNode))
            {
                if (this.Entered)
                {
                    this.currentStateNode.Exit();
                    this.currentStateNode.OnCurrentStateChanged -= this.SetCurrentStateFromCurrentStateNode;
                }

                this.currentStateNode = chosenStateNode;
                this.SetCurrentStateFromCurrentStateNode();

                if (this.Entered)
                {
                    this.currentStateNode.OnCurrentStateChanged += this.SetCurrentStateFromCurrentStateNode;
                    this.currentStateNode.Enter();
                }
            }
        }

        private void SetCurrentStateFromCurrentStateNode(MobState previousState, MobState currentState)
        {
            this.SetCurrentStateFromCurrentStateNode();
        }

        private void SetCurrentStateFromCurrentStateNode()
        {
            this.SetCurrentState(this.currentStateNode.CurrentState);
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
