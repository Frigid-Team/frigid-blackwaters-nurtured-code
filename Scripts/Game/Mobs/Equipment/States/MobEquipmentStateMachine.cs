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
                return this.ChosenStateNode.AutoExit;
            }
        }

        public override void Equipped()
        {
            if (this.returnToStartingNodeBehaviour == ReturnToStartingNodeBehaviour.OnEquipped)
            {
                this.SetChosenStateNode(this.startingStateNode);
            }
            base.Equipped();
            this.CheckTransitions();
        }

        public override void Enter()
        {
            if (this.returnToStartingNodeBehaviour == ReturnToStartingNodeBehaviour.OnEnter)
            {
                this.SetChosenStateNode(this.startingStateNode);
            }
            base.Enter();
            this.CheckTransitions();
        }

        public override void Refresh()
        {
            base.Refresh();
            this.CheckTransitions();
        }

        protected override MobEquipmentStateNode SpawnStateNode
        {
            get
            {
                return this.startingStateNode;
            }
        }

        protected override HashSet<MobEquipmentStateNode> ChildStateNodes
        {
            get
            {
                HashSet<MobEquipmentStateNode> childStateNodes = new HashSet<MobEquipmentStateNode>() { this.startingStateNode };
                foreach (Transition transition in this.transitions)
                {
                    childStateNodes.Add(transition.FromStateNode);
                    childStateNodes.Add(transition.NextStateNode);
                }
                return childStateNodes;
            }
        }

        private void CheckTransitions()
        {
            if (!this.Owner.EquipPoint.Equipper.IsActingAndNotStunned) return;

            List<Transition> validTransitions = new List<Transition>();
            foreach (Transition transition in this.transitions)
            {
                if (transition.FromStateNode == this.ChosenStateNode && 
                    (transition.FromStateNode.AutoExit || transition.NextStateNode.AutoEnter || transition.TriggerCondition.Evaluate(this.EnterDuration, this.EnterDurationDelta)))
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

        [Serializable]
        private struct Transition
        {
            [SerializeField]
            private Conditional triggerCondition;
            [SerializeField]
            private MobEquipmentStateNode fromStateNode;
            [SerializeField]
            private MobEquipmentStateNode nextStateNode;

            public Conditional TriggerCondition
            {
                get
                {
                    return this.triggerCondition;
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
