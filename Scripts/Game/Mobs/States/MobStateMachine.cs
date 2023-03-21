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
        [SerializeField]
        private bool autoEnter;
        [SerializeField]
        private bool autoExit;

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

        public override HashSet<MobState> SwitchableStates
        {
            get
            {
                return this.chosenStateNode.SwitchableStates;
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
                return this.autoEnter && this.chosenStateNode.AutoEnter;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return this.autoExit && this.chosenStateNode.AutoExit;
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
                SetChosenStateNode(this.chosenStartingStateNode);
            }
            base.Enter();
            this.chosenStateNode.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
            this.chosenStateNode.Enter();

            CheckTransitions();
        }

        public override void Exit()
        {
            base.Exit();
            this.chosenStateNode.Exit();
            this.chosenStateNode.OnCurrentStateChanged -= SetCurrentStateFromChosenStateNode;
        }

        public override void Refresh()
        {
            base.Refresh();

            CheckTransitions();
            this.chosenStateNode.Refresh();
        }

        private void CheckTransitions()
        {
            if (!this.Owner.CanAct) return;
            if (this.chosenStateNode.ShouldExit)
            {
                List<Transition> validTransitions = new List<Transition>();
                foreach (Transition transition in this.transitions)
                {
                    if (transition.FromStateNode == this.chosenStateNode &&
                        transition.NextStateNode.ShouldEnter &&
                        (transition.FromStateNode.AutoExit || transition.NextStateNode.AutoEnter || transition.TransitionConditions.Evaluate(this.EnterDuration, this.EnterDurationDelta)))
                    {
                        validTransitions.Add(transition);
                    }
                }

                if (validTransitions.Count > 0)
                {
                    Transition chosenTransition = validTransitions[0];
                    SetChosenStateNode(chosenTransition.NextStateNode);
                }
            }
        }

        private bool CanSetChosenStateNode(MobStateNode chosenStateNode)
        {
            return CanSetCurrentState(chosenStateNode.CurrentState);
        }

        private void SetChosenStateNode(MobStateNode chosenStateNode)
        {
            if (CanSetChosenStateNode(chosenStateNode))
            {
                if (this.Entered)
                {
                    this.chosenStateNode.Exit();
                    this.chosenStateNode.OnCurrentStateChanged -= SetCurrentStateFromChosenStateNode;
                }

                this.chosenStateNode = chosenStateNode;
                SetCurrentStateFromChosenStateNode();

                if (this.Entered)
                {
                    this.chosenStateNode.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
                    this.chosenStateNode.Enter();
                }
            }
        }

        private void SetCurrentStateFromChosenStateNode(MobState previousState, MobState currentState)
        {
            SetCurrentStateFromChosenStateNode();
        }

        private void SetCurrentStateFromChosenStateNode()
        {
            SetCurrentState(this.chosenStateNode.CurrentState);
        }

        [Serializable]
        private struct Transition
        {
            [SerializeField]
            private ConditionalClause transitionConditions;
            [SerializeField]
            private MobStateNode nextStateNode;
            [SerializeField]
            private MobStateNode fromStateNode;

            public ConditionalClause TransitionConditions
            {
                get
                {
                    return this.transitionConditions;
                }
            }

            public MobStateNode NextStateNode
            {
                get
                {
                    return this.nextStateNode;
                }
            }

            public MobStateNode FromStateNode
            {
                get
                {
                    return this.fromStateNode;
                }
            }
        }
    }
}
