using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobDeathState : MobState
    {
        public override bool Dead
        {
            get
            {
                return true;
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
