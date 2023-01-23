using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobStateMachine : MobStateNode
    {
        [SerializeField]
        private MobState fallbackState;
        [SerializeField]
        private MobStateNode chosenStateNode;
        [SerializeField]
        private List<Transition> transitions;

        public override MobState InitialState
        {
            get
            {
                if (this.chosenStateNode == this)
                {
                    return this.fallbackState;
                }
                else
                {
                    return this.chosenStateNode.InitialState;
                }
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                HashSet<MobStateNode> referencedNodes = new HashSet<MobStateNode>();
                referencedNodes.Add(this.fallbackState);
                if (this.chosenStateNode != this) referencedNodes.Add(this.chosenStateNode);
                foreach (Transition transition in this.transitions)
                {
                    if (transition.FromStateNode != this) referencedNodes.Add(transition.FromStateNode);
                    if (transition.NextStateNode != this) referencedNodes.Add(transition.NextStateNode);
                }
                return referencedNodes;
            }
        }

        public override void Enter()
        {
            base.Enter();
            if (this.chosenStateNode == this)
            {
                this.fallbackState.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
                this.fallbackState.Enter();
            }
            else
            {
                this.chosenStateNode.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
                this.chosenStateNode.Enter();
            }
            SetCurrentStateFromChosenStateNode();
        }

        public override void Exit()
        {
            base.Exit();
            if (this.chosenStateNode == this)
            {
                this.fallbackState.OnCurrentStateChanged -= SetCurrentStateFromChosenStateNode;
                this.fallbackState.Exit();
            }
            else
            {
                this.chosenStateNode.OnCurrentStateChanged -= SetCurrentStateFromChosenStateNode;
                this.chosenStateNode.Exit();
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            List<Transition> validTransitions = new List<Transition>();
            foreach (Transition transition in this.transitions)
            {
                if (transition.FromStateNode == this.chosenStateNode && transition.TransitionConditions.Evaluate())
                {
                    validTransitions.Add(transition);
                }
            }

            if (validTransitions.Count > 0)
            {
                Transition chosenTransition = validTransitions[UnityEngine.Random.Range(0, validTransitions.Count)];

                if (this.chosenStateNode == this)
                {
                    this.fallbackState.OnCurrentStateChanged -= SetCurrentStateFromChosenStateNode;
                    this.fallbackState.Exit();
                }
                else
                {
                    this.chosenStateNode.OnCurrentStateChanged -= SetCurrentStateFromChosenStateNode;
                    this.chosenStateNode.Exit();
                }

                this.chosenStateNode = chosenTransition.NextStateNode;

                if (this.chosenStateNode == this)
                {
                    this.fallbackState.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
                    this.fallbackState.Enter();
                }
                else
                {
                    this.chosenStateNode.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
                    this.chosenStateNode.Enter();
                }

                SetCurrentStateFromChosenStateNode();
            }

            if (this.chosenStateNode == this) this.fallbackState.Refresh();
            else this.chosenStateNode.Refresh();
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
