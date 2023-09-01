using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobRandomStateNode : MobStateNode
    {
        [SerializeField]
        private RelativeWeightPool<MobStateNode> stateNodes;

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
                return false;
            }
        }

        public override bool AutoExit
        {
            get
            {
                 return this.chosenStateNode.AutoExit;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return true;
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
            this.SetChosenStateNode(this.stateNodes.Retrieve());
            base.Enter();
            this.chosenStateNode.OnCurrentStateChanged += this.SetCurrentStateFromChosenStateNode;
            this.chosenStateNode.Enter();
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
            this.chosenStateNode.Refresh();
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
    }
}
