using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobStaggerState : MobState
    {
        public override bool Dead
        {
            get
            {
                return false;
            }
        }

        public override MobState InitialState
        {
            get
            {
                return this;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>();
            }
        }
    }
}
