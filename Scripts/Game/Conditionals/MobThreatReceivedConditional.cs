using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobThreatReceivedConditional : MobDamageInfoConditional<ThreatInfo>
    {
        protected override LinkedList<ThreatInfo> GetDamageInfos(Mob mob)
        {
            return mob.ThreatsReceived;
        }

        protected override int TallyDamageInfo(ThreatInfo threatInfo)
        {
            return 1;
        }
    }
}
