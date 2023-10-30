using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobHitDealtConditional : MobDamageInfoConditional<HitInfo>
    {
        [SerializeField]
        private IntSerializedReference minimumDamage;

        protected override LinkedList<HitInfo> GetDamageInfos(Mob mob)
        {
            return mob.HitsDealt;
        }

        protected override int TallyDamageInfo(HitInfo hitInfo)
        {
            return hitInfo.Damage >= this.minimumDamage.ImmutableValue ? 1 : 0;
        }
    }
}
