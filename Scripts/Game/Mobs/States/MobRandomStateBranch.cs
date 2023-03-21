using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobRandomStateBranch : MobStateNode
    {
        [SerializeField]
        private RelativeWeightPool<MobStateNode> stateNodes;
        [SerializeField]
        private bool autoEnter;
        [SerializeField]
        private bool autoExit;

        private MobStateNode chosenStateNode;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                HashSet<MobState> initialStates = new HashSet<MobState>();
                foreach (MobStateNode stateNode in this.stateNodes.Entries)
                {
                    if (stateNode != this) initialStates.UnionWith(stateNode.InitialStates);
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
                foreach (MobStateNode stateNode in this.stateNodes.Entries)
                {
                    if (stateNode != this) referencedStateNodes.Add(stateNode);
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
            foreach (MobStateNode stateNode in this.stateNodes.Entries)
            {
                this.chosenStateNode = stateNode;
                if (stateNode != this && stateNode.InitialStates.Contains(this.CurrentState))
                {
                    break;
                }
            }
        }

        public override void Enter()
        {
            SetChosenStateNode(this.stateNodes.Retrieve());
            base.Enter();
            this.chosenStateNode.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
            this.chosenStateNode.Enter();
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
            this.chosenStateNode.Refresh();
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
    }
}
