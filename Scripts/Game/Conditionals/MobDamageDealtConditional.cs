using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobDamageDealtConditional : MobDamageInfoConditional<HitInfo>
    {
        protected override LinkedList<HitInfo> GetDamageInfos(Mob mob)
        {
            return mob.HitsDealt;
        }

        protected override int TallyDamageInfo(HitInfo hitInfo)
        {
            return hitInfo.Damage;
        }
    }
}
