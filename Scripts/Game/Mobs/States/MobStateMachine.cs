using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobStateMachine : MobStateNode
    {
        [SerializeField]
        private List<MobStateNode> startingStateNodes;
        [SerializeField]
        private List<Transition> transitions;
        [SerializeField]
        private bool returnToStartingNodeOnEnter;

        private MobStateNode chosenStartingStateNode;
        private MobStateNode chosenStateNode;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                HashSet<MobState> initialStates = new HashSet<MobState>();
                foreach (MobStateNode startingStateNode in this.startingStateNodes)
                {
                    initialStates.UnionWith(startingStateNode.InitialStates);
                }
                return initialStates;
            }
        }

        public override HashSet<MobState> MoveStates
        {
            get
            {
                return this.chosenStateNode.MoveStates;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                HashSet<MobStateNode> referencedStateNodes = new HashSet<MobStateNode>();
                foreach (MobStateNode startingStateNode in this.startingStateNodes)
                {
                    referencedStateNodes.Add(startingStateNode);
                }
                foreach (Transition transition in this.transitions)
                {
                    referencedStateNodes.Add(transition.FromStateNode);
                    referencedStateNodes.Add(transition.NextStateNode);
                }
                return referencedStateNodes;
            }
        }

        public override bool AutoEnter
        {
            get
            {
                return false;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return false;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return this.chosenStateNode.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.chosenStateNode.ShouldExit;
            }
        }

        public override void Init()
        {
            base.Init();
            foreach (MobStateNode startingStateNode in this.startingStateNodes)
            {
                this.chosenStartingStateNode = startingStateNode;
                this.chosenStateNode = startingStateNode;
                if (startingStateNode.InitialStates.Contains(this.CurrentState))
                {
                    break;
                }
            }
        }

        public override void Enter()
        {
            if (this.returnToStartingNodeOnEnter)
            {
                this.SetChosenStateNode(this.chosenStartingStateNode);
            }
            base.Enter();
            this.chosenStateNode.OnCurrentStateChanged += this.SetCurrentStateFromChosenStateNode;
            this.chosenStateNode.Enter();

            this.CheckTransitions();
        }

        public override void Exit()
        {
            base.Exit();
            this.chosenStateNode.Exit();
            this.chosenStateNode.OnCurrentStateChanged -= this.SetCurrentStateFromChosenStateNode;
        }

        public override void Refresh()
        {
            base.Refresh();

            this.CheckTransitions();
            this.chosenStateNode.Refresh();
        }

        private void CheckTransitions()
        {
            if (!this.Owner.IsActingAndNotStunned) return;
            if (this.chosenStateNode.ShouldExit)
            {
                List<Transition> validTransitions = new List<Transition>();
                foreach (Transition transition in this.transitions)
                {
                    if (transition.FromStateNode == this.chosenStateNode &&
                        transition.NextStateNode.ShouldEnter &&
                        (transition.FromStateNode.AutoExit || transition.NextStateNode.AutoEnter || transition.TransitionCondition.Evaluate(this.EnterDuration, this.EnterDurationDelta)))
                    {
                        validTransitions.Add(transition);
                    }
                }

                if (validTransitions.Count > 0)
                {
                    Transition chosenTransition = validTransitions[0];
                    this.SetChosenStateNode(chosenTransition.NextStateNode);
                }
            }
        }

        private bool CanSetChosenStateNode(MobStateNode chosenStateNode)
        {
            return this.CanSetCurrentState(chosenStateNode.CurrentState);
        }

        private void SetChosenStateNode(MobStateNode chosenStateNode)
        {
            if (this.CanSetChosenStateNode(chosenStateNode))
            {
                if (this.Entered)
                {
                    this.chosenStateNode.Exit();
                    this.chosenStateNode.OnCurrentStateChanged -= this.SetCurrentStateFromChosenStateNode;
                }

                this.chosenStateNode = chosenStateNode;
                this.SetCurrentStateFromChosenStateNode();

                if (this.Entered)
                {
                    this.chosenStateNode.OnCurrentStateChanged += this.SetCurrentStateFromChosenStateNode;
                    this.chosenStateNode.Enter();
                }
            }
        }

        private void SetCurrentStateFromChosenStateNode(MobState previousState, MobState currentState)
        {
            this.SetCurrentStateFromChosenStateNode();
        }

        private void SetCurrentStateFromChosenStateNode()
        {
            this.SetCurrentState(this.chosenStateNode.CurrentState);
        }

        [Serializable]
        private struct Transition
        {
            [SerializeField]
            private Conditional transitionCondition;
            [SerializeField]
            private MobStateNode fromStateNode;
            [SerializeField]
            private MobStateNode nextStateNode;

            public Conditional TransitionCondition
            {
                get
                {
                    return this.transitionCondition;
                }
            }

            public MobStateNode FromStateNode
            {
                get
                {
                    return this.fromStateNode;
                }
            }

            public MobStateNode NextStateNode
            {
                get
                {
                    return this.nextStateNode;
                }
            }
        }
    }
}
