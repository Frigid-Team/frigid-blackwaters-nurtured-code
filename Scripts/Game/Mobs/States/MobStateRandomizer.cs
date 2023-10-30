using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobStateRandomizer : MobStateNode
    {
        [SerializeField]
        private RelativeWeightPool<MobStateNode> stateNodes;

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
                 return this.ChosenStateNode.AutoExit;
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
                return this.ChosenStateNode.ShouldExit;
            }
        }

        public override void Enter()
        {
            MobStateNode randomStateNode = this.stateNodes.Retrieve();
            if (this.CanSetChosenStateNode(randomStateNode))
            {
                this.SetChosenStateNode(randomStateNode);
            }
            else
            {
                Debug.LogWarning("MobRandomStateNode " + this.name + " cannot enter its retrieved node.");
            }
            base.Enter();
        }

        protected override HashSet<MobStateNode> SpawnStateNodes
        {
            get
            {
                return this.ChildStateNodes;
            }
        }

        protected override HashSet<MobStateNode> MoveStateNodes
        {
            get
            {
                return new HashSet<MobStateNode> { this.ChosenStateNode };
            }
        }

        protected override HashSet<MobStateNode> ChildStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>(this.stateNodes.Entries);
            }
        }
    }
}
