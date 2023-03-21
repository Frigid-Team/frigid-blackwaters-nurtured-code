using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobRepeatStateBranch : MobStateNode
    {
        [SerializeField]
        private MobStateNode stateNode;
        [SerializeField]
        private IntSerializedReference numberRepetitions;

        private int repetitionMax;
        private int repetitionCount;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                return this.stateNode.InitialStates;
            }
        }

        public override HashSet<MobState> SwitchableStates
        {
            get
            {
                return this.stateNode.SwitchableStates;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                return new HashSet<MobStateNode> { this.stateNode };
            }
        }

        public override bool AutoExit
        {
            get
            {
                return this.repetitionCount == this.repetitionMax;
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
            this.repetitionMax = this.numberRepetitions.MutableValue;
            this.repetitionCount = 0;
            this.stateNode.OnCurrentStateChanged += SetCurrentStateFromStateNode;
            this.stateNode.Enter();
            SetCurrentStateFromStateNode();
        }

        public override void Exit()
        {
            base.Exit();
            this.stateNode.Exit();
            this.stateNode.OnCurrentStateChanged -= SetCurrentStateFromStateNode;
        }

        public override void Refresh()
        {
            base.Refresh();
            if (this.Owner.CanAct && 
                this.repetitionCount < this.repetitionMax && 
                this.stateNode.ShouldExit && this.stateNode.ShouldEnter && (this.stateNode.AutoExit || this.stateNode.AutoEnter))
            {
                this.repetitionCount++;
                this.stateNode.Exit();
                this.stateNode.Enter();
            }
            this.stateNode.Refresh();
        }

        private void SetCurrentStateFromStateNode(MobState previousState, MobState currentState)
        {
            SetCurrentStateFromStateNode();
        }

        private void SetCurrentStateFromStateNode()
        {
            SetCurrentState(this.stateNode.CurrentState);
        }
    }
}
