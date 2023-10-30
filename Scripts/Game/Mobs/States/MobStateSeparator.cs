using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobStateSeparator : MobStateNode
    {
        [SerializeField]
        private MobStateNode stateNode;

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
                return new HashSet<MobStateNode> { this.stateNode };
            }
        }
    }
}
