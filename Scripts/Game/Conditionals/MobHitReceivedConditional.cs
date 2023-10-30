using UnityEngine;

using FrigidBlackwaters.Core;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobHitReceivedConditional : MobDamageInfoConditional<HitInfo>
    {
        [SerializeField]
        private IntSerializedReference minimumDamage;

        protected override LinkedList<HitInfo> GetDamageInfos(Mob mob)
        {
            return mob.HitsReceived;
        }

        protected override int TallyDamageInfo(HitInfo hitInfo)
        {
            return hitInfo.Damage >= this.minimumDamage.ImmutableValue ? 1 : 0;
        }
    }
}
