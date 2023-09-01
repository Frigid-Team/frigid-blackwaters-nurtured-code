using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobSeparatorStateNode : MobStateNode
    {
        [SerializeField]
        private MobStateNode stateNode;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                return this.stateNode.InitialStates;
            }
        }

        public override HashSet<MobState> MoveStates
        {
            get
            {
                return this.stateNode.MoveStates;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                return new HashSet<MobStateNode> { this.stateNode };
            }
        }

        public override bool AutoEnter
        {
            get
            {
                return this.stateNode.AutoEnter;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return this.stateNode.AutoExit;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return this.stateNode.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.stateNode.ShouldExit;
            }
        }

        public override void Enter()
        {
            base.Enter();
            this.stateNode.OnCurrentStateChanged += this.SetCurrentStateFromStateNode;
            this.stateNode.Enter();
            this.SetCurrentStateFromStateNode();
        }

        public override void Exit()
        {
            base.Exit();
            this.stateNode.Exit();
            this.stateNode.OnCurrentStateChanged -= this.SetCurrentStateFromStateNode;
        }

        public override void Refresh()
        {
            base.Refresh();
            this.stateNode.Refresh();
        }

        private void SetCurrentStateFromStateNode(MobState previousState, MobState currentState)
        {
            this.SetCurrentStateFromStateNode();
        }

        private void SetCurrentStateFromStateNode()
        {
            this.SetCurrentState(this.stateNode.CurrentState);
        }
    }
}
