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

        public override bool AutoEnter
        {
            get
            {
                return this.chosenStateNode.AutoEnter;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return this.chosenStateNode.AutoExit;
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
                this.SetChosenStateNode(this.startingStateNode);
            }
        }

        public override void Enter()
        {
            if (this.returnToStartingNodeBehaviour == ReturnToStartingNodeBehaviour.OnEnter)
            {
                this.SetChosenStateNode(this.startingStateNode);
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
            if (!this.Owner.EquipPoint.Equipper.IsActingAndNotStunned) return;

            List<Transition> validTransitions = new List<Transition>();
            foreach (Transition transition in this.transitions)
            {
                if (transition.FromStateNode == this.chosenStateNode && 
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

        private void SetChosenStateNode(MobEquipmentStateNode chosenStateNode)
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

        private void SetCurrentStateFromChosenStateNode(MobEquipmentState previousState, MobEquipmentState currentState)
        {
            this.SetCurrentStateFromChosenStateNode();
        }

        private void SetCurrentStateFromChosenStateNode()
        {
            this.CurrentState = this.chosenStateNode.CurrentState;
        }

        [Serializable]
        private struct Transition
        {
            [SerializeField]
            private Conditional transitionCondition;
            [SerializeField]
            private MobEquipmentStateNode fromStateNode;
            [SerializeField]
            private MobEquipmentStateNode nextStateNode;

            public Conditional TransitionCondition
            {
                get
                {
                    return this.transitionCondition;
                }
            }

            public MobEquipmentStateNode FromStateNode
            {
                get
                {
                    return this.fromStateNode;
                }
            }

            public MobEquipmentStateNode NextStateNode
            {
                get
                {
                    return this.nextStateNode;
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
