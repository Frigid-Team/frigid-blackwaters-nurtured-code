using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipmentStateMachine : MobEquipmentStateNode
    {
        [SerializeField]
        private MobEquipmentStateNode startingStateNode;
        [SerializeField]
        private List<Transition> transitions;
        [SerializeField]
        private ReturnToStartingNodeBehaviour returnToStartingNodeBehaviour;

        private MobEquipmentStateNode chosenStateNode;

        public override MobEquipmentState InitialState
        {
            get
            {
                return this.startingStateNode.InitialState;
            }
        }

        public override HashSet<MobEquipmentStateNode> ReferencedStateNodes
        {
            get
            {
                HashSet<MobEquipmentStateNode> referencedStateNodes = new HashSet<MobEquipmentStateNode>();
                referencedStateNodes.Add(this.startingStateNode);
                foreach (Transition transition in this.transitions)
                {
                    referencedStateNodes.Add(transition.FromStateNode);
                    referencedStateNodes.Add(transition.NextStateNode);
                }
                return referencedStateNodes;
            }
        }

        public override void Init()
        {
            base.Init();
            this.chosenStateNode = this.startingStateNode;
        }

        public override void Equipped()
        {
            base.Equipped();
            if (this.returnToStartingNodeBehaviour == ReturnToStartingNodeBehaviour.OnEquipped)
            {
                SetChosenStateNode(this.startingStateNode);
            }
        }

        public override void Enter()
        {
            if (this.returnToStartingNodeBehaviour == ReturnToStartingNodeBehaviour.OnEnter)
            {
                SetChosenStateNode(this.startingStateNode);
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
            List<Transition> validTransitions = new List<Transition>();
            foreach (Transition transition in this.transitions)
            {
                if (transition.FromStateNode == this.chosenStateNode && 
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

        private void SetChosenStateNode(MobEquipmentStateNode chosenStateNode)
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

        private void SetCurrentStateFromChosenStateNode(MobEquipmentState previousState, MobEquipmentState currentState)
        {
            SetCurrentStateFromChosenStateNode();
        }

        private void SetCurrentStateFromChosenStateNode()
        {
            this.CurrentState = this.chosenStateNode.CurrentState;
        }

        [Serializable]
        private struct Transition
        {
            [SerializeField]
            private ConditionalClause transitionConditions;
            [SerializeField]
            private MobEquipmentStateNode nextStateNode;
            [SerializeField]
            private MobEquipmentStateNode fromStateNode;

            public ConditionalClause TransitionConditions
            {
                get
                {
                    return this.transitionConditions;
                }
            }

            public MobEquipmentStateNode NextStateNode
            {
                get
                {
                    return this.nextStateNode;
                }
            }

            public MobEquipmentStateNode FromStateNode
            {
                get
                {
                    return this.fromStateNode;
                }
            }
        }

        private enum ReturnToStartingNodeBehaviour
        {
            None,
            OnEnter,
            OnEquipped
        }
    }
}
