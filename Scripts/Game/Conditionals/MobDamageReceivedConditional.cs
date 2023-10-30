using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobDamageReceivedConditional : MobDamageInfoConditional<HitInfo>
    {
        protected override LinkedList<HitInfo> GetDamageInfos(Mob mob)
        {
            return mob.HitsReceived;
        }

        protected override int TallyDamageInfo(HitInfo hitInfo)
        {
            return hitInfo.Damage;
        }
    }
}
